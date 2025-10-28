using UnityEngine;
using System.Collections.Generic;

public class EnvironmentManager : MonoBehaviour
{
    public Transform player;                 // Referencia al jugador
    public GameObject[] segmentPrefabs;      // Prefabs de los segmentos
    public float segmentLength = 30f;        // Longitud de cada segmento
    public int segmentsVisible = 5;          // Cuántos segmentos mantener activos

    private List<GameObject> activeSegments = new List<GameObject>();
    private float spawnZ = 0f;

    void Start()
    {
        // Generar segmentos iniciales
        for (int i = 0; i < segmentsVisible; i++)
        {
            SpawnSegment();
        }
    }

    void Update()
    {
        // Si el jugador ha avanzado más allá del primer segmento
        if (player.position.z - 35f > (spawnZ - segmentsVisible * segmentLength))
        {
            SpawnSegment();
            DeleteOldestSegment();
        }
    }

    void SpawnSegment()
    {
        if (segmentPrefabs.Length == 0)
        {
            Debug.LogError("No hay prefabs asignados en EnvironmentManager!");
            return;
        }

        int randomIndex = Random.Range(0, segmentPrefabs.Length);
        GameObject newSegment = Instantiate(segmentPrefabs[randomIndex], Vector3.forward * spawnZ, Quaternion.identity);
        activeSegments.Add(newSegment);
        spawnZ += segmentLength;
    }

    void DeleteOldestSegment()
    {
        Destroy(activeSegments[0]);
        activeSegments.RemoveAt(0);
    }
}
