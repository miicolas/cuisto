using UnityEngine;

public class VRSpawnController : MonoBehaviour
{
    public SpawnManager spawnManager;

    void Awake()
    {
        if (spawnManager == null) spawnManager = GetComponent<SpawnManager>();
        if (spawnManager == null) spawnManager = FindAnyObjectByType<SpawnManager>();
    }
}
