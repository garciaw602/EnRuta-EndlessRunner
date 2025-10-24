// SpeedPowerUp.cs
// Implementaci�n espec�fica del poder de velocidad.

using UnityEngine;

// crea la opci�n 'Assets/Create/PowerUps/Speed' en el men�.
[CreateAssetMenu(fileName = "SpeedPowerUp", menuName = "PowerUps/Speed")]
public class SpeedPowerUp : PowerUpEffectData
{
    [Tooltip("Multiplicador de velocidad que aplica este poder.")]
    public float speedMultiplier = 2.0f;

    //  Implementamos la funci�n obligatoria de la interfaz (IPowerUpEffect).
    public override void ApplyEffect(PlayerController player, float duration)
    {
        // La l�gica del Power-Up vive aqu�, encapsulada.
        player.ActivateSpeedBoost(speedMultiplier, duration);
    }
}