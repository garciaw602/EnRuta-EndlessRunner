using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    [Header("M�dulo de Pista")]
    [Tooltip("Arrastra aqu� tu Prefab completo del m�dulo de carretera (PF_RoadModule).")]
    public GameObject roadModulePrefab;

    [Tooltip("Longitud exacta del m�dulo prefab (Medida en RoadModule.cs).")]
    public float moduleLength = 60f;

    private Vector3 nextSpawnPoint;

    void Start()
    {
        // Inicializar la posici�n de aparici�n en el origen.
        nextSpawnPoint = Vector3.zero;

        // Genera unos cuantos m�dulos iniciales (visibles) para que el Player empiece a correr.
        for (int i = 0; i < 4; i++)
        {
            SpawnModule();
        }
    }

    // M�todo llamado por el Player cuando llega al SpawnTrigger
    public void SpawnModule()
    {
        // Instancia el nuevo m�dulo en el punto de aparici�n calculado.
        GameObject newModule = Instantiate(roadModulePrefab, nextSpawnPoint, Quaternion.identity);

        // Actualiza el punto de aparici�n para el siguiente m�dulo.
        nextSpawnPoint.z += moduleLength;

        // Opcional: Ajustar la longitud del m�dulo en el nuevo script RoadModule
        if (newModule.GetComponent<RoadModule>() != null)
        {
            newModule.GetComponent<RoadModule>().moduleLength = moduleLength;
        }
    }
}