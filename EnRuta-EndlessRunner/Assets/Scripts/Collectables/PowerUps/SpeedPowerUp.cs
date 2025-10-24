// SpeedPowerUp.cs
// Implementación específica del poder de velocidad.

using UnityEngine;

// crea la opción 'Assets/Create/PowerUps/Speed' en el menú.
[CreateAssetMenu(fileName = "SpeedPowerUp", menuName = "PowerUps/Speed")]
public class SpeedPowerUp : PowerUpEffectData
{
    [Tooltip("Multiplicador de velocidad que aplica este poder.")]
    public float speedMultiplier = 2.0f;

    //  Implementamos la función obligatoria de la interfaz (IPowerUpEffect).
    public override void ApplyEffect(PlayerController player, float duration)
    {
        // La lógica del Power-Up vive aquí, encapsulada.
        player.ActivateSpeedBoost(speedMultiplier, duration);
    }
}