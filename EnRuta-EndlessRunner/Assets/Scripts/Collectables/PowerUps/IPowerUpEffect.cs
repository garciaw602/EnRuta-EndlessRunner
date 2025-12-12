// IPowerUpEffect.cs
// Define el contrato para el Patrón Strategy.

using UnityEngine;

public interface IPowerUpEffect
{
    //  El receptor  es el PowerUpEffectController.
    void ApplyEffect(PowerUpEffectController receiver, float duration);
}