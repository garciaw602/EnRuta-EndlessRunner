using UnityEngine;

public class EnvironmentSegment : MonoBehaviour
{
    [Header("Spawnable Prefabs")]
    public GameObject[] powerUpPrefabs;
    public GameObject[] trashPrefabs;
    public GameObject[] obstaclePrefabs;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("Parent Containers")]
    public Transform powerUpParent;
    public Transform trashParent;
    public Transform obstacleParent;

    [Header("Probabilidades (0-1)")]
    [Range(0, 1f)] public float powerUpChance = 0.1f;
    [Range(0, 1f)] public float trashChance = 0.4f;
    [Range(0, 1f)] public float obstacleChance = 0.3f;

    void Start()
    {
        foreach (Transform point in spawnPoints)
        {
            float roll = Random.value;

            if (roll < powerUpChance)
                SpawnRandom(point, powerUpPrefabs, powerUpParent);
            else if (roll < powerUpChance + trashChance)
                SpawnRandom(point, trashPrefabs, trashParent);
            else if (roll < powerUpChance + trashChance + obstacleChance)
                SpawnRandom(point, obstaclePrefabs, obstacleParent);
        }
    }

    void SpawnRandom(Transform point, GameObject[] pool, Transform parent)
    {
        if (pool.Length == 0) return;

        int index = Random.Range(0, pool.Length);
        GameObject obj = Instantiate(pool[index], point.position, Quaternion.identity, parent);

        // Asegura que se mueva con el entorno
        Moveback mover = obj.GetComponent<Moveback>();
        if (mover == null)
        {
            mover = obj.AddComponent<Moveback>();
            mover.speed = 10f;
        }
    }
}
