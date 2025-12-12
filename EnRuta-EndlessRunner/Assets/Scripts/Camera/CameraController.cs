using UnityEngine;

public class CameraController : MonoBehaviour
{
    // --- Variables P�blicas (Ajustables desde el Inspector de Unity) ---

    [Header("Objetivo")]
    [Tooltip("Referencia al GameObject del jugador que la c�mara debe seguir.")]
    public Transform playerTarget;

    [Header("Posici�n Fija")]
    [Tooltip("Posici�n X fija de la c�mara.")]
    public float fixedCameraX = 0f;

    [Tooltip("Posici�n Y fija de la c�mara.")]
    public float fixedCameraY = 4.5f;

    [Tooltip("Offset en Z respecto al jugador (distancia hacia atr�s).")]
    public float zOffset = -6f;

    [Tooltip("Velocidad de suavizado del movimiento en Z (cuanto m�s alto, m�s r�pido sigue).")]
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
        // X e Y son fijos, Z sigue al jugador m�s el offset
        targetPosition = new Vector3(
            fixedCameraX,
            fixedCameraY,
            playerTarget.position.z + zOffset
        );

        // 2. Aplicar Suavizado (Lerp) solo en Z
        // La c�mara mantiene X e Y fijos, pero sigue suavemente el eje Z del jugador.
        // Time.deltaTime es crucial para asegurar que el movimiento sea independiente de la tasa de frames.
        Vector3 newPosition = transform.position;
        newPosition.z = Mathf.Lerp(transform.position.z, targetPosition.z, Time.deltaTime * followSpeed);
        transform.position = newPosition;

        // OPCIONAL: Asegurar que la c�mara siempre mire al jugador
        // transform.LookAt(playerTarget); 
        // Nota: En un Endless Runner simple, a menudo la rotaci�n es fija para un look m�s "arcade".
    }
}