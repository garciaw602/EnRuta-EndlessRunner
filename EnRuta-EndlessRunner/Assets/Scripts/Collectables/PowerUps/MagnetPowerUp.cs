using UnityEngine;

[CreateAssetMenu(fileName = "MagnetPowerUp", menuName = "PowerUps/Magnet")]
public class MagnetPowerUp : PowerUpEffectData
{
    [Tooltip("Radio de atracción que aplica el imán.")]
    public float attractionRadius = 10f;

    // FIX: Llama al método del PowerUpEffectController.
    public override void ApplyEffect(PowerUpEffectController receiver, float duration)
    {
        receiver.ActivateMagnet(attractionRadius, duration);
    }
}