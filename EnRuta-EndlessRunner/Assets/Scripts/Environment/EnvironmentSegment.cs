using UnityEngine;
using System.Collections.Generic;

public class EnvironmentSegment : MonoBehaviour
{
    [Header("Lane points (child transforms inside the prefab)")]
    public Transform[] lanePoints; // e.g. 3 child empties with local X = -2,0,2

    [Header("Optional length helpers (only used if present)")]
    public Transform startPoint; // optional, used only for visual editing
    public Transform endPoint;   // optional, used only for visual editing

    [Header("Prefabs (individual prefabs)")]
    public GameObject[] collectiblePrefabs;
    public GameObject[] obstaclePrefabs;
    public GameObject[] powerUpPrefabs;

    [Header("Spawn config")]
    [Range(0f,1f)] public float collectibleChance = 0.6f;
    [Range(0f,1f)] public float obstacleChance = 0.3f;
    [Range(0f,1f)] public float powerUpChance = 0.1f;

    public float safeStartOffset = 8f;    // inside-segment margin before first spawn
    public float safeEndOffset = 6f;      // margin near end where we don't spawn
    public float rowSpacing = 4f;         // Z spacing between rows
    public int minCollectibles = 4;
    public int maxCollectibles = 8;
    public float collectibleSpacing = 1.0f; // spacing between coins in sequence (Z)
    public float collectibleY = 1.2f;     // height of collectables

    // cached spawned list for cleanup
    private List<GameObject> spawned = new List<GameObject>();
    private float cachedLength = -1f;

    void Start()
    {
        GenerateObjects();
    }

    // Public getter used by manager
    public float GetSegmentLength()
    {
        if (cachedLength > 0f) return cachedLength;

        // Prefer explicit start/end if provided
        if (startPoint != null && endPoint != null)
        {
            cachedLength = Mathf.Abs(endPoint.position.z - startPoint.position.z);
            if (cachedLength > 0f) return cachedLength;
        }

        // Otherwise compute bounds of renderers
        Renderer[] rs = GetComponentsInChildren<Renderer>();
        if (rs != null && rs.Length > 0)
        {
            Bounds b = rs[0].bounds;
            for (int i = 1; i < rs.Length; i++) b.Encapsulate(rs[i].bounds);
            cachedLength = b.size.z;
            if (cachedLength > 0f) return cachedLength;
        }

        // fallback
        cachedLength = 40f;
        return cachedLength;
    }

    void GenerateObjects()
    {
        if (lanePoints == null || lanePoints.Length == 0)
        {
            Debug.LogWarning($"[EnvironmentSegment] {name} no tiene lanePoints asignados.");
            return;
        }

        float length = GetSegmentLength();
        float zStart = transform.position.z + safeStartOffset;
        float zEnd = transform.position.z + length - safeEndOffset;

        // iterate rows along Z (world space)
        for (float z = zStart; z <= zEnd; z += rowSpacing)
        {
            float roll = Random.value;

            if (roll < collectibleChance)
            {
                int lane = Random.Range(0, lanePoints.Length);
                SpawnCollectableSequence(lane, z);
                // advance a bit to avoid overlapped sequences
                z += rowSpacing;
                continue;
            }

            // try obstacles and powerups (per-lane)
            for (int lane = 0; lane < lanePoints.Length; lane++)
            {
                float r = Random.value;

                if (r < obstacleChance)
                {
                    Vector3 pos = new Vector3(lanePoints[lane].position.x, 0f, z);
                    SpawnOne(prefabPool: obstaclePrefabs, worldPos: pos);
                }
                else if (r < obstacleChance + powerUpChance)
                {
                    Vector3 pos = new Vector3(lanePoints[lane].position.x, 1.2f, z);
                    SpawnOne(prefabPool: powerUpPrefabs, worldPos: pos);
                }
                // else empty
            }
        }
    }

    void SpawnCollectableSequence(int laneIndex, float startZ)
    {
        if (collectiblePrefabs == null || collectiblePrefabs.Length == 0) return;

        int count = Random.Range(minCollectibles, maxCollectibles + 1);
        GameObject prefab = collectiblePrefabs[Random.Range(0, collectiblePrefabs.Length)];

        for (int i = 0; i < count; i++)
        {
            float z = startZ + i * collectibleSpacing;
            Vector3 pos = new Vector3(lanePoints[laneIndex].position.x, collectibleY, z);
            SpawnOne(new GameObject[] { prefab }, pos);
        }
    }

    void SpawnOne(GameObject[] prefabPool, Vector3 worldPos)
    {
        if (prefabPool == null || prefabPool.Length == 0) return;
        GameObject prefab = prefabPool[Random.Range(0, prefabPool.Length)];
        GameObject go = Instantiate(prefab, worldPos, Quaternion.identity, transform);
        // DO NOT add MoveBack to children — segment will have MoveBack from manager
        spawned.Add(go);
    }

    // optional cleanup called by manager before destroying
    public void ClearSpawned()
    {
        for (int i = spawned.Count - 1; i >= 0; i--)
            if (spawned[i] != null) Destroy(spawned[i]);
        spawned.Clear();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (lanePoints != null)
        {
            Gizmos.color = Color.yellow;
            foreach (var l in lanePoints)
            {
                if (l != null) Gizmos.DrawSphere(l.position, 0.2f);
            }
        }

        if (startPoint != null && endPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(startPoint.position, endPoint.position);
        }
    }
#endif
}
