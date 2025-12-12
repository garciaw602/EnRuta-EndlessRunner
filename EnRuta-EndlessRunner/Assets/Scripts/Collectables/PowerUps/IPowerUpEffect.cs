// IPowerUpEffect.cs
// Define el contrato para el Patrón Strategy.

public interface IPowerUpEffect
{
    
    // El contrato ahora pide el tipo correcto de controlador (PowerUpEffectController).
    void ApplyEffect(PowerUpEffectController controller, float duration);
}