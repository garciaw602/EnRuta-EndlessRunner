// PowerUpEffectData.cs
// Clase base abstracta que implementa la interfaz de la Estrategia.

using UnityEngine;

// Debe ser 'abstract' porque no queremos crear instancias directas de este ScriptableObject.
public abstract class PowerUpEffectData : ScriptableObject, IPowerUpEffect
{
    [Tooltip("Duración estándar del efecto si no se especifica en la implementación concreta.")]
    public float duration = 5f;

    
    // La clase abstracta ahora pide el tipo correcto.
    public abstract void ApplyEffect(PowerUpEffectController controller, float duration);
}