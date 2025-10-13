using UnityEngine;

public class CameraController : MonoBehaviour
{
    // --- Variables Públicas (Ajustables desde el Inspector de Unity) ---

    [Header("Objetivo")]
    [Tooltip("Referencia al GameObject del jugador que la cámara debe seguir.")]
    public Transform playerTarget;

    [Header("Offset y Suavizado")]
    [Tooltip("Vector de desplazamiento fijo de la cámara respecto al jugador (ej: X=0, Y=4, Z=-6).")]
    public Vector3 cameraOffset = new Vector3(0f, 4.5f, -6f);

    [Tooltip("Velocidad de suavizado del movimiento de la cámara (cuanto más alto, más rápido sigue).")]
    public float followSpeed = 5f;


    // --- Variables Privadas ---

    // La posición objetivo que la cámara intentará alcanzar en cada frame.
    private Vector3 targetPosition;


    // LateUpdate se llama después de que todos los objetos han sido actualizados en Update().
    // Esto asegura que la cámara sigue la posición FINAL del jugador en el frame actual.
    void LateUpdate()
    {
        // Verificación de seguridad: si no hay jugador asignado, no hacemos nada.
        if (playerTarget == null)
        {
            Debug.LogError("ERROR: El Player Target no está asignado en CameraController.");
            return;
        }

        // 1. Calcular la Posición Objetivo
        // La posición objetivo es la posición actual del jugador más el desplazamiento (offset) fijo.
        targetPosition = playerTarget.position + cameraOffset;

        // 2. Aplicar Suavizado (Lerp)
        // Usamos Vector3.Lerp para mover la cámara de su posición actual
        // a la posición objetivo de forma suave, usando la velocidad definida (followSpeed).
        // Time.deltaTime es crucial para asegurar que el movimiento sea independiente de la tasa de frames.
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * followSpeed);

        // OPCIONAL: Asegurar que la cámara siempre mire al jugador
        // transform.LookAt(playerTarget); 
        // Nota: En un Endless Runner simple, a menudo la rotación es fija para un look más "arcade".
    }
}