using UnityEngine;
using System.Collections.Generic;

public class EnvironmentManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public List<EnvironmentSegment> segmentPrefabs; // prefabs must have EnvironmentSegment component

    [Header("Spawn control")]
    public int initialSegments = 3;
    public float initialSpawnAhead = 20f;    // first segment start position relative to player.z
    public float safeSpawnDistance = 30f;    // ensure spawned segment is at least this far ahead of player
    public float extraSpacing = 0.1f;        // tiny extra gap if needed

    private Queue<EnvironmentSegment> active = new Queue<EnvironmentSegment>();
    private float nextSpawnZ;
    private bool initialized = false;

    void Start()
    {
        if (player == null) Debug.LogError("[EnvironmentManager] Asigna Player.");
        if (segmentPrefabs == null || segmentPrefabs.Count == 0) Debug.LogError("[EnvironmentManager] Asigna segmentPrefabs.");

        nextSpawnZ = player != null ? player.position.z + initialSpawnAhead : 0f;

        for (int i = 0; i < initialSegments; i++) SpawnSegment(initial: true);

        initialized = true;
    }

    void Update()
    {
        if (!initialized || player == null) return;

        // Ensure there is always next segment far enough ahead
        EnvironmentSegment last = active.Count > 0 ? active.ToArray()[active.Count - 1] : null;
        if (last != null)
        {
            float lastEndZ = last.transform.position.z + last.GetSegmentLength();
            if (lastEndZ < player.position.z + safeSpawnDistance)
                SpawnSegment();
        }

        // Destroy first only when player passed half of second
        if (active.Count >= 2)
        {
            EnvironmentSegment[] arr = active.ToArray();
            EnvironmentSegment first = arr[0];
            EnvironmentSegment second = arr[1];

            if (first != null && second != null)
            {
                float secondMid = second.transform.position.z + second.GetSegmentLength() * 0.5f;
                if (player.position.z > secondMid)
                {
                    // clean spawned children (safe) and destroy
                    first.ClearSpawned();
                    Destroy(first.gameObject);
                    active.Dequeue();
                }
            }
        }
    }

    void SpawnSegment(bool initial = false)
    {
        EnvironmentSegment prefab = segmentPrefabs[Random.Range(0, segmentPrefabs.Count)];
        if (prefab == null) return;

        float spawnZ = nextSpawnZ;
        if (!initial && player != null)
            spawnZ = Mathf.Max(nextSpawnZ, player.position.z + safeSpawnDistance);

        Vector3 spawnPos = new Vector3(0f, 0f, spawnZ);
        EnvironmentSegment inst = Instantiate(prefab, spawnPos, Quaternion.identity);

        // Add MoveBack to entire segment so children move automatically with it
        MoveBack mb = inst.GetComponent<MoveBack>();
        if (mb == null)
        {
            mb = inst.gameObject.AddComponent<MoveBack>();
            mb.useGlobalSpeed = true;
        }

        float len = inst.GetSegmentLength();
        if (len <= 0f) len = inst.GetSegmentLength(); // ensure cached

        nextSpawnZ = spawnZ + len + extraSpacing;
        active.Enqueue(inst);
    }
}
