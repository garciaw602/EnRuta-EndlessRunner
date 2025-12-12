using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 1. Instancia Singleton
    public static GameManager Instance { get; private set; }

    // 2. Eventos Globales (Observer Pattern)
    public event Action OnGameOver;       // Evento para notificar el fin del juego.
    public event Action OnScoreUpdated;   // Evento para notificar que la UI de puntajes debe refrescarse.
    public event Action OnGameStart;      // Evento para notificar el inicio.

    // 3. Estado del Juego
    public bool IsGameOver { get; private set; } = false;

    // 4. Temporizador de Supervivencia
    private float _survivalTime = 0f;
    public float SurvivalTime => _survivalTime;

    [Header("Configuración de Pausa")]
    public float timeBeforePauseOnDeath = 0.8f;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        // Asegura que el juego NO esté pausado al inicio
        Time.timeScale = 1f;
    }

    void Start()
    {
        // Se emite el evento al iniciar (para que sistemas como EnvironmentManager y UIManager reaccionen si es necesario)
        OnGameStart?.Invoke();
    }

    void Update()
    {
        if (IsGameOver) return;
        _survivalTime += Time.deltaTime;
    }

    /// <summary>
    /// Llamado por PlayerController.cs al chocar con un Obstáculo.
    /// Inicia el proceso de fin de juego y notifica a la UI/sistemas.
    /// </summary>
    public void GameOver()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        // La única llamada al fin de juego es aquí:
        StartCoroutine(EndGameRoutine());
    }

    /// <summary>
    /// Método público llamado por ScoreManager para notificar que la UI debe actualizarse.
    /// </summary>
    public void NotifyScoreUpdated()
    {
        // Solo el GameManager puede invocar el evento.
        OnScoreUpdated?.Invoke();
    }

    private IEnumerator EndGameRoutine()
    {
        // 1. Notifica a los suscriptores (ej: UIManager activa el panel)
        OnGameOver?.Invoke();

        yield return new WaitForSeconds(timeBeforePauseOnDeath);

        // 3. Pausa el juego
        Time.timeScale = 0f;
    }
    public void RestartGame()
    {
        // 1. Despausa el juego (importante, ya que GameOver lo pausó a 0)
        Time.timeScale = 1f;

        // 2. Obtiene el índice de la escena actual
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 3. Carga la escena por su índice, reiniciando el juego
        SceneManager.LoadScene(currentSceneIndex);
    }
}