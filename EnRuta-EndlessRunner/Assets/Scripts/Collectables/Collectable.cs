using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour
{
    [Tooltip("Arrastra aquí el archivo CollectableData (.asset)")]
    public CollectableData data;

    private PowerUpEffectController powerUpEffects;

    void Start()
    {
        
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        // Asegurar que es Trigger para que el jugador lo atraviese
        GetComponent<Collider>().isTrigger = true;
    }

    /// <summary>
    /// Maneja la recolección por CONTACTO DIRECTO (Jugador toca el objeto).
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();

        if (player != null)
        {
            if (powerUpEffects == null) powerUpEffects = player.GetComponent<PowerUpEffectController>();

            bool isMagnetActive = (powerUpEffects != null && powerUpEffects.isMagnetActive);

            
            // Solo recolectamos si el imán NO está activo.
            // Si el imán está activo, la Basura será atraída y el PowerUp se queda.
            if (!isMagnetActive)
            {
                // Si el imán está inactivo, se recoge CUALQUIER COSA por contacto.

               
                if (powerUpEffects != null)
                {
                    powerUpEffects.RemoveFromMagnetList(gameObject);
                }

                // Procesar la lógica (Activar PowerUp o Sumar Basura)
                player.ProcessCollectable(data);

                // Destruir el objeto visual (¡Se recoge!)
                Destroy(gameObject);
            }

            // Si el imán está activo:
            // - Basura: El código salta esta condición y el objeto espera ser atraído.
            // - PowerUp: El código salta esta condición y el PowerUp se queda en escena, permitiendo que el jugador lo traspase.
        }
    }
}