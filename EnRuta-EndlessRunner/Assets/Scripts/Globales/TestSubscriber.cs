using UnityEngine;

public class TestSubscriber : MonoBehaviour
{
    [Header("Prueba Observer")]
    public string SystemName = "UI Manager";

    void OnEnable()
    {
        // 🚀 SOLUCIÓN CS0120: Acceder al evento a través de la instancia Singleton
        // Se asegura que GameManager.Instance ya exista antes de suscribirse.
        if (GameManager.Instance != null)
        {
            // Este es el único punto de contacto: ¡GameManager.Instance!
            GameManager.Instance.OnGameOver += HandleGameOverEvent;
            Debug.Log("DEBUG: TestSubscriber suscrito al evento OnGameOver.");
        }
        else
        {
            // Si el GameManager aún no se inicializa, intentamos de nuevo más tarde o mostramos un error.
            // Para esta prueba, basta con el log.
            Debug.LogError("Error de Suscripción: GameManager.Instance es NULL. Asegúrate de que GameManager se ejecute primero.");
        }
    }

    void OnDisable()
    {
        // Es crucial desuscribirse para evitar errores cuando el objeto se desactiva.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameOver -= HandleGameOverEvent;
            Debug.Log("DEBUG: TestSubscriber desuscrito.");
        }
    }

    /// <summary>
    /// Función llamada automáticamente cuando GameManager.OnGameOver se invoca.
    /// </summary>
    private void HandleGameOverEvent()
    {
        // Esta es la acción que se ejecuta al recibir la notificación de Game Over.
        Debug.LogFormat("OBSERVER RECIBIDO: El sistema {0} recibió la notificación. Activando mi lógica post-colisión.", SystemName);
    }
}