// IPowerUpEffect.cs
// Define el contrato para el Patr�n Strategy.

public interface IPowerUpEffect
{
    // M�todo que cualquier poder debe implementar para aplicarse al jugador.
    void ApplyEffect(PlayerController player, float duration);
}