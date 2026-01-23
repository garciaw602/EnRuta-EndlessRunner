using UnityEngine;
using System.Collections.Generic;

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
    public float obstacleY = 1.2f;
    
    [Header("Alturas")]
    public float obstacleHeight = 2.0f; // Altura típica de los obstáculos
    public float collectibleAboveObstacle = 1.2f; // Distancia encima del techo del obstáculo

    [Header("Distancia entre Obstáculos")]
    public float minDistanceBetweenObstacles = 6f; // Distancia mínima entre obstáculos (en unidades Z)

    [Header("Rotación de Objetos")]
    public Vector3 obstacleRotation = new Vector3(0, 0, 0); // Rotación para obstáculos (carros)
    public Vector3 collectibleRotation = new Vector3(0, 0, 0); // Rotación para coleccionables
    public Vector3 powerUpRotation = new Vector3(0, 0, 0); // Rotación para power-ups

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

        // Lista para almacenar TODAS las posiciones Z donde hay obstáculos (cualquier carril)
        List<float> allObstaclePositions = new List<float>();
        
        // Diccionario para almacenar obstáculos por carril
        Dictionary<int, HashSet<float>> obstaclesByLane = new Dictionary<int, HashSet<float>>();
        for (int i = 0; i < segment.lanePoints.Length; i++)
        {
            obstaclesByLane[i] = new HashSet<float>();
        }

        // PRIMERA PASADA: Spawnear obstáculos y power-ups (para mantener registro)
        for (float z = zStart; z <= zEnd; z += rowSpacing)
        {
            if (z < minZAllowed)
                continue;

            float roll = Random.value;

            if (roll < collectibleChance)
            {
                // Saltamos esta posición Z para coleccionables (se harán después)
                continue;
            }

            for (int i = 0; i < segment.lanePoints.Length; i++)
            {
                float r = Random.value;

                if (r < obstacleChance)
                {
                    // Verificar si hay un obstáculo demasiado cerca en otras posiciones
                    if (CanSpawnObstacle(z, allObstaclePositions))
                    {
                        Vector3 pos = new Vector3(segment.lanePoints[i].position.x, obstacleY, z);
                        SpawnOne(segment, obstaclePrefabs, pos, obstacleRotation);
                        obstaclesByLane[i].Add(z); // Registrar que hay obstáculo en esta posición
                        allObstaclePositions.Add(z); // Registrar posición global
                    }
                }
                else if (r < obstacleChance + powerUpChance)
                {
                    Vector3 pos = new Vector3(segment.lanePoints[i].position.x, collectibleY, z);
                    SpawnOne(segment, powerUpPrefabs, pos, powerUpRotation);
                }
            }
        }

        // SEGUNDA PASADA: Spawnear coleccionables (secuencias)
        for (float z = zStart; z <= zEnd; z += rowSpacing)
        {
            if (z < minZAllowed)
                continue;

            float roll = Random.value;

            if (roll < collectibleChance)
            {
                int lane = Random.Range(0, segment.lanePoints.Length);
                SpawnCollectibleSequence(segment, lane, z, obstaclesByLane[lane]);
                z += rowSpacing;
                continue;
            }
        }
    }

    bool CanSpawnObstacle(float zPosition, List<float> existingObstacles)
    {
        foreach (float existingZ in existingObstacles)
        {
            if (Mathf.Abs(zPosition - existingZ) < minDistanceBetweenObstacles)
            {
                return false; // Hay un obstáculo demasiado cerca
            }
        }
        return true; // Puedo spawnear
    }

    void SpawnCollectibleSequence(EnvironmentSegment s, int lane, float startZ, HashSet<float> obstaclesInLane)
    {
        int count = Random.Range(minCollectibles, maxCollectibles + 1);
        GameObject prefab = collectiblePrefabs[Random.Range(0, collectiblePrefabs.Length)];

        for (int i = 0; i < count; i++)
        {
            float z = startZ + i * collectibleSpacing;
            
            // Determinar altura del coleccionable
            float collectibleHeight = collectibleY; // Altura normal por defecto
            
            // Si hay un obstáculo cerca en esta posición Z, elevar el coleccionable encima
            if (obstaclesInLane.Contains(z))
            {
                collectibleHeight = obstacleY + obstacleHeight + collectibleAboveObstacle;
            }
            
            Vector3 pos = new Vector3(s.lanePoints[lane].position.x, collectibleHeight, z);
            SpawnOne(s, new GameObject[] { prefab }, pos, collectibleRotation);
        }
    }

    void SpawnOne(EnvironmentSegment s, GameObject[] pool, Vector3 pos, Vector3 rotation)
    {
        if (pool == null || pool.Length == 0) return;

        GameObject prefab = pool[Random.Range(0, pool.Length)];
        Quaternion finalRotation = Quaternion.Euler(rotation);
        Instantiate(prefab, pos, finalRotation, s.objectsRoot);
    }
}
