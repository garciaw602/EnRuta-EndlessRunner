// PowerUpEffectData.cs
// Clase base abstracta que implementa la interfaz de la Estrategia.

using UnityEngine;

public abstract class PowerUpEffectData : ScriptableObject, IPowerUpEffect
{
    [Tooltip("Duración estándar del efecto si no se especifica en la implementación concreta.")]
    public float duration = 5f;

    //  tipo esperado  PowerUpEffectController.
    public abstract void ApplyEffect(PowerUpEffectController receiver, float duration);
}