using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    [Header("Referencias de Pista")]
    [Tooltip("Prefab del m�dulo de carretera a instanciar. Debe ser RoadModule_01.")]
    public GameObject roadModulePrefab;

    [Header("Configuraci�n de Generaci�n")]
    [Tooltip("Longitud exacta del m�dulo (10m). Aseg�rate de que este valor es 10f en el Inspector.")]
    public float moduleLength = 50f;

    [Tooltip("N�mero de m�dulos iniciales para crear el buffer (ej. 4).")]
    public int initialModules = 4;

    // Posici�n donde aparecer� el siguiente m�dulo (la parte trasera Z=0 del prefab).
    private Vector3 nextSpawnPoint;


    void Start()
    {
        // 1. Inicializar nextSpawnPoint.
        // Calculamos la posici�n trasera del primer m�dulo (3 longitudes de buffer).
        // nextSpawnPoint = -(10 * 3) = -30.
        // Esto coloca el M�dulo 1 en -30m, asegurando que el M�dulo 4 est� de 0m a 10m.
        nextSpawnPoint = new Vector3(0, 0, -(moduleLength * (initialModules - 1)));

        // 2. Generar los m�dulos iniciales
        for (int i = 0; i < initialModules; i++)
        {
            SpawnModule();
        }
    }


    /// <summary>
    /// Instancia un nuevo m�dulo de pista y avanza el punto de aparici�n.
    /// Esta funci�n es llamada por el PlayerController (al cruzar el SpawnTrigger en Z=10).
    /// </summary>
    public void SpawnModule()
    {
        // 1. Instancia el m�dulo en nextSpawnPoint (su parte trasera Z=0).
        GameObject newModule = Instantiate(roadModulePrefab, nextSpawnPoint, Quaternion.identity);

        // 2. Avanza el punto de aparici�n. La nueva posici�n es la parte trasera del siguiente m�dulo (10m m�s adelante).
        nextSpawnPoint.z += moduleLength;

        // Opcional: Esto es �til si RoadModule necesita la longitud para su propia l�gica.
        if (newModule.GetComponent<RoadModule>() != null)
        {
            newModule.GetComponent<RoadModule>().moduleLength = moduleLength;
        }
    }
}