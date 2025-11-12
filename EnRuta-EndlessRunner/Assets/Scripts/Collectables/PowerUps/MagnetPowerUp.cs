using UnityEngine;

[CreateAssetMenu(fileName = "MagnetPowerUp", menuName = "PowerUps/Magnet")]
public class MagnetPowerUp : PowerUpEffectData
{
    [Tooltip("Radio de atracción que aplica el imán.")]
    public float attractionRadius = 10f;

    // Implementación del efecto: Llama a la lógica central del PowerUpEffectController.
    
    public override void ApplyEffect(PowerUpEffectController controller, float duration)
    {
        controller.ActivateMagnet(attractionRadius, duration);
    }
}