// CollectableData.cs
// Define los datos de un �tem coleccionable.

using UnityEngine;

// crea la opci�n 'Assets/Create/Collectables/Collectable Data' en el men�.
[CreateAssetMenu(fileName = "NewCollectableData", menuName = "Collectables/Collectable Data")]
public class CollectableData : ScriptableObject
{
    [Header("Identificaci�n")]
    public string collectableName;
    public CollectableType type;

    [Tooltip("Valor que a�ade al inventario (ej. 1 bolsa, 1 unidad de pl�stico)")]
    public int baseValue = 1;

    [Header("Comportamiento (Patr�n Strategy)")]
    // Referencia al archivo SO que ejecuta el efecto si 'type' es PowerUp.
    [Tooltip("El ScriptableObject que contiene la l�gica del power-up si es de tipo PowerUp.")]
    public PowerUpEffectData powerUpEffect;
}

// Enum para la categorizaci�n 
public enum CollectableType
{
    GeneralGarbage,
    Recyclable,
    PowerUp
}