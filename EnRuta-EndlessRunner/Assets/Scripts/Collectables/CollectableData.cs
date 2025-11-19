// CollectableData.cs
// Define los datos de un ítem coleccionable.

using UnityEngine;

// crea la opción 'Assets/Create/Collectables/Collectable Data' en el menú.
[CreateAssetMenu(fileName = "NewCollectableData", menuName = "Collectables/Collectable Data")]
public class CollectableData : ScriptableObject
{
    [Header("Identificación")]
    public string collectableName;
    public CollectableType type;

    [Tooltip("Valor que añade al inventario (ej. 1 bolsa, 1 unidad de plástico)")]
    public int baseValue = 1;

    [Header("Comportamiento (Patrón Strategy)")]
    // Referencia al archivo SO que ejecuta el efecto si 'type' es PowerUp.
    [Tooltip("El ScriptableObject que contiene la lógica del power-up si es de tipo PowerUp.")]
    public PowerUpEffectData powerUpEffect;
}

// Enum para la categorización 
public enum CollectableType
{
    GeneralGarbage,
    Recyclable,
    PowerUp
}