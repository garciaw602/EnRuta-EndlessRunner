using UnityEngine;
using System;
using System.Collections; // Necesario para usar Coroutines

public class GameManager : MonoBehaviour
{
    // 1. Instancia Singleton: Propiedad de solo lectura para acceso global.
    public static GameManager Instance { get; private set; }

    // 2. Evento Global (Observer Pattern)
    public event Action OnGameOver;
    public event Action OnScoreUpdated; 
    public event Action OnGameStart;

    // 3. Estado del Juego
    public bool IsGameOver { get; private set; } = false;

    // 4. Configuración de Pausa para la Animación
    [Header("Configuración de Pausa")]
    [Tooltip("Tiempo en segundos que espera el juego antes de pausarse (para que la animación de muerte se vea).")]
    public float timeBeforePauseOnDeath = 0.8f; // Valor inicial recomendado: 0.8 segundos


    void Awake()
    {
        // Implementación Estándar de Singleton
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Opcional
        }
        else
        {
            Destroy(gameObject);
        }

        Debug.Log("DEBUG: GameManager Singleton inicializado correctamente.");
    }

    /// <summary>
    /// Llamado por PlayerController.cs al chocar con un Obstáculo.
    /// Inicia el proceso de fin de juego, notifica y luego pausa usando una coroutine.
    /// </summary>
    public void EndGame()
    {
        if (IsGameOver) return;

        IsGameOver = true;

        // Inicia la corrutina que maneja el retraso para la animación.
        StartCoroutine(EndGameRoutine());
    }

    /// <summary>
    /// Método público para que otras clases (como ScoreManager) puedan notificar
    /// que la puntuación ha cambiado. SOLUCIÓN al ERROR CS0070.
    /// </summary>
    public void NotifyScoreUpdated()
    {
        // Se invoca el evento desde DENTRO de la clase donde fue declarado.
        OnScoreUpdated?.Invoke();
        Debug.Log("EVENTO EMITIDO: OnScoreUpdated fue invocado.");
    }

    /// <summary>
    /// Corrutina para pausar el juego después de un tiempo para permitir que la animación se reproduzca.
    /// </summary>
    private IEnumerator EndGameRoutine()
    {
        // 1. Notifica a los suscriptores (debe ir antes de la pausa para que reaccionen a tiempo)
        OnGameOver?.Invoke();

        Debug.Log("EVENTO: OnGameOver emitido. Esperando " + timeBeforePauseOnDeath + " segundos para la animación...");

        // 2. Esperar el tiempo definido para que la animación de muerte se ejecute.
        yield return new WaitForSeconds(timeBeforePauseOnDeath);

        // 3. Pausa el juego
        Time.timeScale = 0f;

        Debug.Log("VERIFICACIÓN: Juego Pausado. Time.timeScale actual es: " + Time.timeScale);
    }


    /// <summary>
    /// Función para reiniciar el estado del juego (útil para empezar una nueva partida).
    /// </summary>
    public void ResetGame()
    {
        IsGameOver = false;
        Time.timeScale = 1f; // Reanuda el tiempo
        Debug.Log("DEBUG: GameManager reseteado. Juego reanudado.");
    }
}