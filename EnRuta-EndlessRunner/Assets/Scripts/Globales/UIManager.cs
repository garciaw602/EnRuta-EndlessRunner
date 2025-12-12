using UnityEngine;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    // 1. INSTANCIA SINGLETON
    public static UIManager Instance { get; private set; }

    // --- 2. Referencias de Contadores (Asignar en el Inspector) ---
    [Header("Contadores de Reciclaje")]
    public TextMeshProUGUI plasticText;
    public TextMeshProUGUI glassText;
    public TextMeshProUGUI cardboardText;
    public TextMeshProUGUI totalGarbageText;

    // --- 3. Referencias de Pantallas de Estado ---
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI powerUpText;

    // --- 4. Referencias de Pantallas de Fin de Juego ---
    public GameObject gameOverPanel;

    private bool isSubscribed = false;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (powerUpText != null) powerUpText.text = string.Empty;
    }


    void OnEnable()
    {
        if (GameManager.Instance != null && !isSubscribed)
        {
            // ➡️ Suscripción a Eventos (¡CRÍTICO!):
            GameManager.Instance.OnGameOver += HandleGameOverEvent;
            GameManager.Instance.OnScoreUpdated += UpdateInventoryUI;

            isSubscribed = true;

            // Llamadas iniciales
            UpdateInventoryUI();
            UpdateGameTime(0f);
        }
    }

    void OnDisable()
    {
        if (GameManager.Instance != null && isSubscribed)
        {
            // ⬅️ Desuscripción de Eventos (¡Importante para evitar errores!):
            GameManager.Instance.OnGameOver -= HandleGameOverEvent;
            GameManager.Instance.OnScoreUpdated -= UpdateInventoryUI;
            isSubscribed = false;
        }
    }

    void Update()
    {
        // El tiempo se actualiza constantemente
        if (GameManager.Instance != null && !GameManager.Instance.IsGameOver)
        {
            UpdateGameTime(GameManager.Instance.SurvivalTime);
        }
    }

    // --- MÉTODOS DE ACTUALIZACIÓN ---

    public void UpdateGameTime(float timeInSeconds)
    {
        if (timeText == null) return;

        int minutes = Mathf.FloorToInt(timeInSeconds / 60F);
        int seconds = Mathf.FloorToInt(timeInSeconds - minutes * 60);

        timeText.text = string.Format("TIEMPO: {0:00}:{1:00}", minutes, seconds);
    }

    // Llamado desde PowerUpEffectController.cs (no usa evento, por ahora se actualiza directo)
    public void ShowPowerUpStatus(string powerUpName, float remainingTime)
    {
        if (powerUpText == null) return;
        powerUpText.text = $"[ACTIVO] {powerUpName}: {remainingTime:0.0}s";
    }

    // Llamado desde PowerUpEffectController.cs
    public void ClearPowerUpStatus()
    {
        if (powerUpText == null) return;
        powerUpText.text = string.Empty;
    }

    // 🔔 Lógica que se ejecuta al recibir el evento OnScoreUpdated
    private void UpdateInventoryUI()
    {
        if (ScoreManager.Instance == null) return;

        // Lee directamente los contadores del ScoreManager (que es otro Singleton)
        if (plasticText != null) plasticText.text = "x" + ScoreManager.Instance.PlasticCount.ToString();
        if (glassText != null) glassText.text = "x" + ScoreManager.Instance.GlassCount.ToString();
        if (cardboardText != null) cardboardText.text = "x" + ScoreManager.Instance.CardboardCount.ToString();
        if (totalGarbageText != null) totalGarbageText.text = "x" + ScoreManager.Instance.TotalGarbage.ToString();
    }

    // 🔔 Lógica que se ejecuta al recibir el evento OnGameOver
    private void HandleGameOverEvent()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
}