using UnityEngine;

[CreateAssetMenu(fileName = "SpeedPowerUp", menuName = "PowerUps/Speed")]
public class SpeedPowerUp : PowerUpEffectData
{
    [Tooltip("Multiplicador de velocidad (usar < 1 para ralentizar)")]
    public float speedMultiplier = 2.0f;

    // FIX: Llama al método del PowerUpEffectController.
    public override void ApplyEffect(PowerUpEffectController receiver, float duration)
    {
        receiver.ActivateSpeedBoost(speedMultiplier, duration);
    }
}