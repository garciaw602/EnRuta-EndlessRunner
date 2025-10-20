using UnityEngine;

public class RoadModule : MonoBehaviour
{
    // VARIABLES DEL INSPECTOR
    [Tooltip("Velocidad de movimiento del módulo.")]
    public float moveSpeed = 10f;

    [Tooltip("Longitud real del módulo para calcular la posición de respawn.")]
    public float moduleLength = 100f; // Debe coincidir con el Size Z del Box Collider.

    // VARIABLES ESTÁTICAS PARA EL POSICIONAMIENTO GLOBAL
    // Usaremos un campo estático para saber dónde colocar SIEMPRE el siguiente módulo.
    private static float nextSpawnZ = 0f;

    // Se llama solo una vez en la vida del módulo.
    void Start()
    {
        // Si este es el primer módulo, inicializamos la posición.
        if (nextSpawnZ == 0f)
        {
            // El primer módulo se posiciona en 0.
            transform.position = Vector3.zero;
            // El siguiente módulo debe empezar donde termina este.
            nextSpawnZ = moduleLength;
        }
        else
        {
            // Posicionamos el módulo justo donde el anterior terminó.
            // La posición central del nuevo módulo debe ser el punto de spawn + (mitad de su largo).
            transform.position = new Vector3(0, 0, nextSpawnZ + (moduleLength / 2));

            // Actualizamos la posición para el SIGUIENTE módulo.
            nextSpawnZ += moduleLength;
        }
    }

    void Update()
    {
        // 1. Mover el módulo hacia atrás (eje Z negativo)
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);

        // 2. Lógica para reposicionar (reciclar) el módulo
        // Si el centro del módulo se ha movido una longitud completa detrás del punto de origen (0),
        // es hora de enviarlo al final de la carretera.
        if (transform.position.z < -moduleLength)
        {
            // REPOSICIONAMIENTO: Simplemente lo colocamos en el punto 'nextSpawnZ'

            // 1. Calculamos la nueva posición central.
            transform.position = new Vector3(0, 0, nextSpawnZ + (moduleLength / 2));

            // 2. Actualizamos el punto de spawn para que el siguiente módulo sepa dónde ir.
            nextSpawnZ += moduleLength;
        }
    }
}