using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "SpeedPowerUp", menuName = "PowerUps/Speed")]
public class SpeedPowerUp : PowerUpEffectData
{
    [Tooltip("Multiplicador de velocidad que aplica este poder.")]
    public float speedMultiplier = 1.2f;

    public override void ApplyEffect(PlayerController player, float duration)
    {
        player.StartCoroutine(ApplySpeedBoost(duration));
    }

    private IEnumerator ApplySpeedBoost(float duration)
    {
        float originalSpeed = MoveBack.GlobalSpeed;
        float boostedSpeed = originalSpeed * speedMultiplier;

        MoveBack.SetGlobalSpeed(boostedSpeed);
        yield return new WaitForSeconds(duration);
        MoveBack.SetGlobalSpeed(originalSpeed);
    }
}
