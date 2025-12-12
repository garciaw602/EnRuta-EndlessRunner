using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject[] collectiblePrefabs;
    public GameObject[] obstaclePrefabs;
    public GameObject[] powerUpPrefabs;

    [Header("Probabilidades")]
    [Range(0f,1f)] public float collectibleChance = 0.6f;
    [Range(0f,1f)] public float obstacleChance = 0.3f;
    [Range(0f,1f)] public float powerUpChance = 0.1f;

    [Header("Spawn Limits")]
    public float minSafeDistanceFromPlayer = 12f; 
    public Transform player;

    public float safeStartOffset = 8f;
    public float safeEndOffset = 6f;
    public float rowSpacing = 4f;

    public int minCollectibles = 4;
    public int maxCollectibles = 8;
    public float collectibleSpacing = 1f;
    public float collectibleY = 1.2f;

    public void PopulateSegment(EnvironmentSegment segment)
    {
        segment.ClearObjects();

        if (segment.lanePoints == null || segment.lanePoints.Length == 0)
            return;

        float len = segment.GetSegmentLength();

        float zStart = segment.transform.position.z + safeStartOffset;
        float zEnd = segment.transform.position.z + len - safeEndOffset;

        // Nueva regla: solo bloquea objetos demasiado cerca del jugador
        float minZAllowed = player.position.z + minSafeDistanceFromPlayer;

        for (float z = zStart; z <= zEnd; z += rowSpacing)
        {
            if (z < minZAllowed)
                continue; // Evita objetos pegados al jugador, pero permite los demás

            float roll = Random.value;

            if (roll < collectibleChance)
            {
                int lane = Random.Range(0, segment.lanePoints.Length);
                SpawnCollectibleSequence(segment, lane, z);
                z += rowSpacing;
                continue;
            }

            for (int i = 0; i < segment.lanePoints.Length; i++)
            {
                float r = Random.value;

                if (r < obstacleChance)
                {
                    Vector3 pos = new Vector3(segment.lanePoints[i].position.x, 0f, z);
                    SpawnOne(segment, obstaclePrefabs, pos);
                }
                else if (r < obstacleChance + powerUpChance)
                {
                    Vector3 pos = new Vector3(segment.lanePoints[i].position.x, collectibleY, z);
                    SpawnOne(segment, powerUpPrefabs, pos);
                }
            }
        }
    }

    void SpawnCollectibleSequence(EnvironmentSegment s, int lane, float startZ)
    {
        int count = Random.Range(minCollectibles, maxCollectibles + 1);
        GameObject prefab = collectiblePrefabs[Random.Range(0, collectiblePrefabs.Length)];

        for (int i = 0; i < count; i++)
        {
            float z = startZ + i * collectibleSpacing;
            Vector3 pos = new Vector3(s.lanePoints[lane].position.x, collectibleY, z);
            SpawnOne(s, new GameObject[] { prefab }, pos);
        }
    }

    void SpawnOne(EnvironmentSegment s, GameObject[] pool, Vector3 pos)
    {
        if (pool == null || pool.Length == 0) return;

        GameObject prefab = pool[Random.Range(0, pool.Length)];
        Instantiate(prefab, pos, Quaternion.identity, s.objectsRoot);
    }
}
