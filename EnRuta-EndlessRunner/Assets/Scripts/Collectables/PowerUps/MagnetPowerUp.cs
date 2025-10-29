using UnityEngine;

// crea la opción 'Assets/Create/PowerUps/Magnet' en el menú (ajusta tu menú si es diferente).
[CreateAssetMenu(fileName = "MagnetPowerUp", menuName = "PowerUps/Magnet")]
public class MagnetPowerUp : PowerUpEffectData
{
    [Tooltip("Radio de atracción que aplica el imán.")]
    public float attractionRadius = 10f; // Distancia máxima para atraer collectibles

    // Implementación del efecto: Llama a la lógica central del PlayerController.
    public override void ApplyEffect(PlayerController player, float duration)
    {
        player.ActivateMagnet(attractionRadius, duration);
    }
}