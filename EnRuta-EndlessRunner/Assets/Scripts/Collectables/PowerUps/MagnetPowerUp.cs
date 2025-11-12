using UnityEngine;

[CreateAssetMenu(fileName = "MagnetPowerUp", menuName = "PowerUps/Magnet")]
public class MagnetPowerUp : PowerUpEffectData
{
    [Tooltip("Radio de atracci�n que aplica el im�n.")]
    public float attractionRadius = 10f;

    // Implementaci�n del efecto: Llama a la l�gica central del PowerUpEffectController.
    
    public override void ApplyEffect(PowerUpEffectController controller, float duration)
    {
        controller.ActivateMagnet(attractionRadius, duration);
    }
}