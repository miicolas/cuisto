using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SpawnManager : MonoBehaviour
{
    [Serializable]
    public class SpawnableEntry
    {
        public string displayName;
        public GameObject prefab;
        public Sprite icon;
        public Vector3 eulerRotation;
        public Vector3 scale = Vector3.one;
        public float yOverride = float.NaN;
        public bool randomYaw;
    }

    public List<SpawnableEntry> spawnables = new();
    public float spawnForwardDistance = 2f;
    public float groundRaycastDistance = 50f;
    public float spawnHeightOffset = 0.1f;

    static void EnsureGrabbable(GameObject go)
    {
        if (go == null) return;
        var rb = go.GetComponent<Rigidbody>();
        if (rb == null) rb = go.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints.None;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        if (go.GetComponent<Collider>() == null)
        {
            foreach (var mf in go.GetComponentsInChildren<MeshFilter>())
                if (mf.GetComponent<Collider>() == null)
                    mf.gameObject.AddComponent<MeshCollider>().convex = true;
        }
        var grab = go.GetComponent<XRGrabInteractable>();
        if (grab == null) grab = go.AddComponent<XRGrabInteractable>();
        grab.forceGravityOnDetach = true;
    }

    Vector3 ComputeSpawnPosition()
    {
        Camera cam = Camera.main;
        Vector3 basePos = cam != null ? cam.transform.position : transform.position;
        Vector3 forward = cam != null ? cam.transform.forward : transform.forward;
        forward.y = 0;
        forward.Normalize();
        Vector3 right = Vector3.Cross(Vector3.up, forward);

        Vector2 jitter = UnityEngine.Random.insideUnitCircle * 0.6f;
        Vector3 target = basePos + forward * spawnForwardDistance + right * jitter.x;
        Vector3 rayOrigin = new Vector3(target.x, basePos.y, target.z);
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, groundRaycastDistance, ~0, QueryTriggerInteraction.Ignore))
            return hit.point + Vector3.up * spawnHeightOffset;
        return new Vector3(target.x, basePos.y - 1f, target.z);
    }

    GameObject SpawnEntry(SpawnableEntry entry)
    {
        if (entry == null || entry.prefab == null) return null;

        Vector3 pos = ComputeSpawnPosition();
        if (!float.IsNaN(entry.yOverride)) pos.y = entry.yOverride;

        Vector3 euler = entry.eulerRotation;
        if (entry.randomYaw) euler.y += UnityEngine.Random.Range(0, 4) * 90f;

        GameObject go = Instantiate(entry.prefab, pos, Quaternion.Euler(euler));
        if (entry.scale != Vector3.zero && entry.scale != Vector3.one)
            go.transform.localScale = entry.scale;
        EnsureGrabbable(go);
        return go;
    }

    public GameObject Spawn(int index)
    {
        if (index < 0 || index >= spawnables.Count)
        {
            Debug.LogWarning($"SpawnManager.Spawn: index {index} out of range (count={spawnables.Count})");
            return null;
        }
        return SpawnEntry(spawnables[index]);
    }

    public GameObject Spawn(string displayName)
    {
        SpawnableEntry entry = spawnables.Find(e => e != null && e.displayName == displayName);
        if (entry == null)
        {
            Debug.LogWarning($"SpawnManager.Spawn: no spawnable named '{displayName}'");
            return null;
        }
        return SpawnEntry(entry);
    }

    public void SpawnTable() => Spawn("Table");
    public void SpawnChair() => Spawn("Chair");
    public void SpawnDeco() => Spawn("Deco");
}
