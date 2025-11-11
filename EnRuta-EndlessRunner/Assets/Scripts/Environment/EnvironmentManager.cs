using UnityEngine;
using System.Collections.Generic;

public class EnvironmentManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public List<EnvironmentSegment> segmentPrefabs;
    public ObjectSpawner objectSpawner;

    [Header("Spawn control")]
    public int initialSegments = 3;
    public float safeSpawnDistance = 30f;
    public float extraSpacing = 0.1f;

    private Queue<EnvironmentSegment> active = new Queue<EnvironmentSegment>();
    private float nextSpawnZ;
    private bool initialized = false;

    void Start()
    {
        if (player == null) { Debug.LogError("[EnvironmentManager] Falta referencia al jugador."); return; }
        if (segmentPrefabs == null || segmentPrefabs.Count == 0) { Debug.LogError("[EnvironmentManager] No hay prefabs de segmentos."); return; }
        if (objectSpawner == null) { Debug.LogError("[EnvironmentManager] Falta ObjectSpawner."); return; }

        // Inicio justo donde está el jugador
        nextSpawnZ = player.position.z;

        for (int i = 0; i < initialSegments; i++)
            SpawnSegment(initial: true);

        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        EnvironmentSegment last = active.Count > 0 ? active.ToArray()[active.Count - 1] : null;

        if (last != null)
        {
            if (player.position.z + safeSpawnDistance > last.transform.position.z)
                SpawnSegment();
        }

        if (active.Count >= 2)
        {
            EnvironmentSegment[] arr = active.ToArray();
            EnvironmentSegment first = arr[0];
            EnvironmentSegment second = arr[1];

            float secondMidZ = second.transform.position.z + second.GetSegmentLength() * 0.5f;

            if (player.position.z > secondMidZ)
            {
                first.ClearObjects();
                Destroy(first.gameObject);
                active.Dequeue();
            }
        }
    }

    void SpawnSegment(bool initial = false)
    {
        EnvironmentSegment prefab = segmentPrefabs[Random.Range(0, segmentPrefabs.Count)];

        float spawnZ = nextSpawnZ;

        Vector3 pos = new Vector3(0f, 0f, spawnZ);
        EnvironmentSegment inst = Instantiate(prefab, pos, Quaternion.identity);

        objectSpawner.PopulateSegment(inst);

        float len = inst.GetSegmentLength();
        nextSpawnZ = spawnZ + len + extraSpacing;

        active.Enqueue(inst);
    }
}
