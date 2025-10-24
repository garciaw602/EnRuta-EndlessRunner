// PowerUpEffectData.cs
// Clase base abstracta que implementa la interfaz de la Estrategia.

using UnityEngine;

// Debe ser 'abstract' porque no queremos crear instancias directas de este ScriptableObject.
public abstract class PowerUpEffectData : ScriptableObject, IPowerUpEffect
{
    [Tooltip("Duraci�n est�ndar del efecto si no se especifica en la implementaci�n concreta.")]
    public float duration = 5f;

    // M�todo abstracto: Obliga a cada poder espec�fico a definir su propia l�gica de ApplyEffect.
    public abstract void ApplyEffect(PlayerController player, float duration);
}