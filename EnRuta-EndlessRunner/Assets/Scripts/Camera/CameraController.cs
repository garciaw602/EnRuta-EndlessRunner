using UnityEngine;

public class CameraController : MonoBehaviour
{
    // --- Variables P�blicas (Ajustables desde el Inspector de Unity) ---

    [Header("Objetivo")]
    [Tooltip("Referencia al GameObject del jugador que la c�mara debe seguir.")]
    public Transform playerTarget;

    [Header("Offset y Suavizado")]
    [Tooltip("Vector de desplazamiento fijo de la c�mara respecto al jugador (ej: X=0, Y=4, Z=-6).")]
    public Vector3 cameraOffset = new Vector3(0f, 4.5f, -6f);

    [Tooltip("Velocidad de suavizado del movimiento de la c�mara (cuanto m�s alto, m�s r�pido sigue).")]
    public float followSpeed = 5f;


    // --- Variables Privadas ---

    // La posici�n objetivo que la c�mara intentar� alcanzar en cada frame.
    private Vector3 targetPosition;


    // LateUpdate se llama despu�s de que todos los objetos han sido actualizados en Update().
    // Esto asegura que la c�mara sigue la posici�n FINAL del jugador en el frame actual.
    void LateUpdate()
    {
        // Verificaci�n de seguridad: si no hay jugador asignado, no hacemos nada.
        if (playerTarget == null)
        {
            Debug.LogError("ERROR: El Player Target no est� asignado en CameraController.");
            return;
        }

        // 1. Calcular la Posici�n Objetivo
        // La posici�n objetivo es la posici�n actual del jugador m�s el desplazamiento (offset) fijo.
        targetPosition = playerTarget.position + cameraOffset;

        // 2. Aplicar Suavizado (Lerp)
        // Usamos Vector3.Lerp para mover la c�mara de su posici�n actual
        // a la posici�n objetivo de forma suave, usando la velocidad definida (followSpeed).
        // Time.deltaTime es crucial para asegurar que el movimiento sea independiente de la tasa de frames.
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

        // OPCIONAL: Asegurar que la c�mara siempre mire al jugador
        // transform.LookAt(playerTarget); 
        // Nota: En un Endless Runner simple, a menudo la rotaci�n es fija para un look m�s "arcade".
    }
}