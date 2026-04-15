using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject tablePrefab;
    public GameObject chairPrefab;
    public GameObject decoPrefab;
    public float spawnCheckRadius = 0.45f;
    public float spawnForwardDistance = 2f;
    public float groundRaycastHeight = 100f;
    public float groundRaycastDistance = 300f;
    public float spawnHeightOffset = 0.5f;

    readonly Collider[] overlapResults = new Collider[8];

    bool TryGetSpawnPosition(out Vector3 spawnPosition, out Collider groundCollider)
    {
        spawnPosition = Vector3.zero;
        groundCollider = null;

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("Impossible de poser : aucune caméra MainCamera détectée.");
            return false;
        }

        Vector3 forward = cam.transform.forward;
        forward.y = 0;
        forward.Normalize();

        float[] tryDistances = { spawnForwardDistance, 1.5f, 1f, 0.5f };
        foreach (float dist in tryDistances)
        {
            Vector3 target = cam.transform.position + forward * dist;
            Vector3 rayOrigin = new Vector3(target.x, cam.transform.position.y + groundRaycastHeight, target.z);
            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, groundRaycastDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                spawnPosition = hit.point + Vector3.up * spawnHeightOffset;
                groundCollider = hit.collider;
                return true;
            }
        }
        return false;
    }

    bool CanSpawnAtPosition(Vector3 spawnPosition, Collider groundCollider)
    {
        int hitCount = Physics.OverlapSphereNonAlloc(
            spawnPosition,
            spawnCheckRadius,
            overlapResults,
            ~0,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < hitCount; i++)
        {
            Collider hitCollider = overlapResults[i];
            if (hitCollider == null)
            {
                continue;
            }

            if (hitCollider == groundCollider)
            {
                continue;
            }

            return false;
        }

        return true;
    }

    public void SpawnTable()
    {
        if (!TryGetSpawnPosition(out Vector3 spawnPosition, out Collider groundCollider))
        {
            Debug.Log("Impossible de poser ici : pas de sol détecté.");
            return;
        }

        if (!CanSpawnAtPosition(spawnPosition, groundCollider))
        {
            Debug.Log("Impossible de poser ici : emplacement occupé.");
            return;
        }

        float rightAngleRotation = Random.Range(0, 4) * 90f;
        Vector3 tablePosition = new Vector3(spawnPosition.x, 0.1f, spawnPosition.z);
        GameObject table = Instantiate(tablePrefab, tablePosition, Quaternion.Euler(270, 0, rightAngleRotation));
        table.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
    }

    public void SpawnChair()
    {
        if (!TryGetSpawnPosition(out Vector3 spawnPosition, out Collider groundCollider))
        {
            Debug.Log("Impossible de poser ici : pas de sol détecté.");
            return;
        }

        if (!CanSpawnAtPosition(spawnPosition, groundCollider))
        {
            Debug.Log("Impossible de poser ici : emplacement occupé.");
            return;
        }

        Instantiate(chairPrefab, spawnPosition, Quaternion.identity);
    }

    public void SpawnDeco()
    {
        if (!TryGetSpawnPosition(out Vector3 spawnPosition, out Collider groundCollider))
        {
            Debug.Log("Impossible de poser ici : pas de sol détecté.");
            return;
        }

        if (!CanSpawnAtPosition(spawnPosition, groundCollider))
        {
            Debug.Log("Impossible de poser ici : emplacement occupé.");
            return;
        }

        Instantiate(decoPrefab, spawnPosition, Quaternion.identity);
    }
}