// MagnetDetector.cs (Adjuntar al Magnet_Hitbox - Tag: Player)
using UnityEngine;

public class MagnetDetector : MonoBehaviour
{
    // Asignado en el Inspector al PlayerController del padre
    public PlayerController playerController;
    private PowerUpEffectController powerUpController;

    void Start()
    {
        // Obtenemos el controlador del padre (donde está el PlayerController)
        if (playerController != null)
        {
            powerUpController = playerController.GetComponent<PowerUpEffectController>();
        }

        if (powerUpController == null)
        {
            Debug.LogError("MagnetDetector no puede encontrar PowerUpEffectController en el objeto padre.");
        }
    }

    // Este es el único OnTriggerEnter que tiene el radio GRANDE (Imán)
    private void OnTriggerEnter(Collider other)
    {
        // Solo verificamos la lógica si el Imán está activo
        if (powerUpController == null || !powerUpController.isMagnetActive) return;

        // Asumiendo que tus coleccionables tienen un script Collectable (o similar)
        Collectable collectable = other.GetComponent<Collectable>();

        if (collectable != null && collectable.data != null)
        {
            // 1. FILTRADO: Si es un PowerUp, lo ignoramos, la atracción solo es para Basura/Reciclaje.
            // (Asumimos que CollectableType.PowerUp es la única excepción)
            if (collectable.data.type == CollectableType.PowerUp)
            {
                return;
            }

            // 2. ATRACCIÓN: Si es Basura/Reciclaje, lo añadimos a la lista del controlador principal.
            if (other.CompareTag("Collectables")) // Solo atrae coleccionables
            {
                powerUpController.attractableObjects.Add(other.gameObject);
                // Nota: El PowerUpEffectController ya se encarga de mover y recolectar el objeto en su Update.
            }
        }
    }
}