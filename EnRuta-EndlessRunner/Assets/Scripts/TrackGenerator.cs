using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    [Header("Módulo de Pista")]
    [Tooltip("Arrastra aquí tu Prefab completo del módulo de carretera (PF_RoadModule).")]
    public GameObject roadModulePrefab;

    [Tooltip("Longitud exacta del módulo prefab (Medida en RoadModule.cs).")]
    public float moduleLength = 60f;

    private Vector3 nextSpawnPoint;

    void Start()
    {
        // Inicializar la posición de aparición en el origen.
        nextSpawnPoint = Vector3.zero;

        // Genera unos cuantos módulos iniciales (visibles) para que el Player empiece a correr.
        for (int i = 0; i < 4; i++)
        {
            SpawnModule();
        }
    }

    // Método llamado por el Player cuando llega al SpawnTrigger
    public void SpawnModule()
    {
        // Instancia el nuevo módulo en el punto de aparición calculado.
        GameObject newModule = Instantiate(roadModulePrefab, nextSpawnPoint, Quaternion.identity);

        // Actualiza el punto de aparición para el siguiente módulo.
        nextSpawnPoint.z += moduleLength;

        // Opcional: Ajustar la longitud del módulo en el nuevo script RoadModule
        if (newModule.GetComponent<RoadModule>() != null)
        {
            newModule.GetComponent<RoadModule>().moduleLength = moduleLength;
        }
    }
}