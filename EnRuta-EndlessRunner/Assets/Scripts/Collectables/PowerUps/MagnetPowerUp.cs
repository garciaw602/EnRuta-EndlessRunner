using UnityEngine;

// crea la opci�n 'Assets/Create/PowerUps/Magnet' en el men� (ajusta tu men� si es diferente).
[CreateAssetMenu(fileName = "MagnetPowerUp", menuName = "PowerUps/Magnet")]
public class MagnetPowerUp : PowerUpEffectData
{
    [Tooltip("Radio de atracci�n que aplica el im�n.")]
    public float attractionRadius = 10f; // Distancia m�xima para atraer collectibles

    // Implementaci�n del efecto: Llama a la l�gica central del PlayerController.
    public override void ApplyEffect(PlayerController player, float duration)
    {
        player.ActivateMagnet(attractionRadius, duration);
    }
}