using UnityEngine;

[CreateAssetMenu(fileName = "SpeedPowerUp", menuName = "PowerUps/Speed")]
public class SpeedPowerUp : PowerUpEffectData
{
    [Tooltip("Multiplicador de velocidad (usar < 1 para ralentizar)")]
    public float speedMultiplier = 2.0f;

    //  Implementamos la funci�n obligatoria de la interfaz (IPowerUpEffect).
    
    public override void ApplyEffect(PowerUpEffectController controller, float duration)
    {
        // La l�gica del Power-Up vive en el controlador.
        controller.ActivateSpeedBoost(speedMultiplier, duration);
    }
}