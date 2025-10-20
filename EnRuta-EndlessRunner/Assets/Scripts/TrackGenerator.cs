using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    [Header("Referencias de Pista")]
    [Tooltip("Prefab del módulo de carretera a instanciar. Debe ser RoadModule_01.")]
    public GameObject roadModulePrefab;

    [Header("Configuración de Generación")]
    [Tooltip("Longitud exacta del módulo (10m). Asegúrate de que este valor es 10f en el Inspector.")]
    public float moduleLength = 50f;

    [Tooltip("Número de módulos iniciales para crear el buffer (ej. 4).")]
    public int initialModules = 4;

    // Posición donde aparecerá el siguiente módulo (la parte trasera Z=0 del prefab).
    private Vector3 nextSpawnPoint;


    void Start()
    {
        // 1. Inicializar nextSpawnPoint.
        // Calculamos la posición trasera del primer módulo (3 longitudes de buffer).
        // nextSpawnPoint = -(10 * 3) = -30.
        // Esto coloca el Módulo 1 en -30m, asegurando que el Módulo 4 esté de 0m a 10m.
        nextSpawnPoint = new Vector3(0, 0, -(moduleLength * (initialModules - 1)));

        // 2. Generar los módulos iniciales
        for (int i = 0; i < initialModules; i++)
        {
            SpawnModule();
        }
    }


    /// <summary>
    /// Instancia un nuevo módulo de pista y avanza el punto de aparición.
    /// Esta función es llamada por el PlayerController (al cruzar el SpawnTrigger en Z=10).
    /// </summary>
    public void SpawnModule()
    {
        // 1. Instancia el módulo en nextSpawnPoint (su parte trasera Z=0).
        GameObject newModule = Instantiate(roadModulePrefab, nextSpawnPoint, Quaternion.identity);

        // 2. Avanza el punto de aparición. La nueva posición es la parte trasera del siguiente módulo (10m más adelante).
        nextSpawnPoint.z += moduleLength;

        // Opcional: Esto es útil si RoadModule necesita la longitud para su propia lógica.
        if (newModule.GetComponent<RoadModule>() != null)
        {
            newModule.GetComponent<RoadModule>().moduleLength = moduleLength;
        }
    }
}