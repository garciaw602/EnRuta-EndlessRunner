using UnityEngine;
using System;
using System.Collections; // Necesario si queremos lógica de puntuación por distancia, pero no por ahora.

public class ScoreManager : MonoBehaviour
{
    // 1. Singleton para acceso rápido
    public static ScoreManager Instance { get; private set; }

    // 2. Almacenamiento de variables (Movidas desde PlayerController)
    private int _totalGarbage = 0;
    private int _plasticCount = 0;
    private int _glassCount = 0;
    private int _cardboardCount = 0;

    // 3. Propiedades Públicas de Solo Lectura (Usadas por la UI)
    public int TotalGarbage => _totalGarbage;
    public int PlasticCount => _plasticCount;
    public int GlassCount => _glassCount;
    public int CardboardCount => _cardboardCount;

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
    }

    /// <summary>
    /// Añade un ítem al inventario y notifica que la puntuación ha cambiado (CRÍTICO para UI).
    /// </summary>
    public void AddToInventory(string name, int value, CollectableType type)
    {
        switch (type)
        {
            case CollectableType.GeneralGarbage:
                _totalGarbage += value;
                break;
            case CollectableType.Recyclable:
                ProcessRecyclable(name, value);
                break;
                // PowerUp no afecta el inventario, solo el estado del jugador.
        }

        // ¡CRÍTICO para Tarea 2.2/2.3! Notifica a la UI que muestre los nuevos valores.
        if (GameManager.Instance != null)
        {
            GameManager.Instance.NotifyScoreUpdated();
        }

        Debug.Log($"INVENTARIO ACTUALIZADO: Total Basura: {_totalGarbage}, Cartón: {_cardboardCount}, Plástico: {_plasticCount}, Vidrio: {_glassCount}");
    }

    private void ProcessRecyclable(string name, int value)
    {
        // Se asume que los nombres de los SO de reciclables contienen estas palabras clave.
        if (name.Contains("Plástico")) _plasticCount += value;
        else if (name.Contains("Vidrio")) _glassCount += value;
        else if (name.Contains("Cartón")) _cardboardCount += value;
    }
}
