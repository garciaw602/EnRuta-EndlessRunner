// PowerUpEffectData.cs
// Clase base abstracta que implementa la interfaz de la Estrategia.

using UnityEngine;

// Debe ser 'abstract' porque no queremos crear instancias directas de este ScriptableObject.
public abstract class PowerUpEffectData : ScriptableObject, IPowerUpEffect
{
    [Tooltip("Duración estándar del efecto si no se especifica en la implementación concreta.")]
    public float duration = 5f;

    // Método abstracto: Obliga a cada poder específico a definir su propia lógica de ApplyEffect.
    public abstract void ApplyEffect(PlayerController player, float duration);
}