// IPowerUpEffect.cs
// Define el contrato para el Patrón Strategy.

public interface IPowerUpEffect
{
    // Método que cualquier poder debe implementar para aplicarse al jugador.
    void ApplyEffect(PlayerController player, float duration);
}