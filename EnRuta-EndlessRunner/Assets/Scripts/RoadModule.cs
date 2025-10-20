using UnityEngine;

public class RoadModule : MonoBehaviour
{
    // VARIABLES DEL INSPECTOR
    [Tooltip("Velocidad de movimiento del m�dulo.")]
    public float moveSpeed = 10f;

    [Tooltip("Longitud real del m�dulo para calcular la posici�n de respawn.")]
    public float moduleLength = 100f; // Debe coincidir con el Size Z del Box Collider.

    // VARIABLES EST�TICAS PARA EL POSICIONAMIENTO GLOBAL
    // Usaremos un campo est�tico para saber d�nde colocar SIEMPRE el siguiente m�dulo.
    private static float nextSpawnZ = 0f;

    // Se llama solo una vez en la vida del m�dulo.
    void Start()
    {
        // Si este es el primer m�dulo, inicializamos la posici�n.
        if (nextSpawnZ == 0f)
        {
            // El primer m�dulo se posiciona en 0.
            transform.position = Vector3.zero;
            // El siguiente m�dulo debe empezar donde termina este.
            nextSpawnZ = moduleLength;
        }
        else
        {
            // Posicionamos el m�dulo justo donde el anterior termin�.
            // La posici�n central del nuevo m�dulo debe ser el punto de spawn + (mitad de su largo).
            transform.position = new Vector3(0, 0, nextSpawnZ + (moduleLength / 2));

            // Actualizamos la posici�n para el SIGUIENTE m�dulo.
            nextSpawnZ += moduleLength;
        }
    }

    void Update()
    {
        // 1. Mover el m�dulo hacia atr�s (eje Z negativo)
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);

        // 2. L�gica para reposicionar (reciclar) el m�dulo
        // Si el centro del m�dulo se ha movido una longitud completa detr�s del punto de origen (0),
        // es hora de enviarlo al final de la carretera.
        if (transform.position.z < -moduleLength)
        {
            // REPOSICIONAMIENTO: Simplemente lo colocamos en el punto 'nextSpawnZ'

            // 1. Calculamos la nueva posici�n central.
            transform.position = new Vector3(0, 0, nextSpawnZ + (moduleLength / 2));

            // 2. Actualizamos el punto de spawn para que el siguiente m�dulo sepa d�nde ir.
            nextSpawnZ += moduleLength;
        }
    }
}