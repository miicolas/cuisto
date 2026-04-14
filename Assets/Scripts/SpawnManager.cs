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

        Vector3 horizontalTarget = new Vector3(
            cam.transform.position.x + forward.x * spawnForwardDistance,
            0f,
            cam.transform.position.z + forward.z * spawnForwardDistance
        );

        Vector3 rayOrigin = new Vector3(horizontalTarget.x, cam.transform.position.y + 0.5f, horizontalTarget.z);
        if (!Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, groundRaycastDistance, ~0, QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        spawnPosition = hit.point + Vector3.up * spawnHeightOffset;
        groundCollider = hit.collider;
        return true;
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
        Instantiate(tablePrefab, spawnPosition, Quaternion.Euler(270, 0, rightAngleRotation));
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