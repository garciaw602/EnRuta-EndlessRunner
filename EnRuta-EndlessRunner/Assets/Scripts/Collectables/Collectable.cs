// Collectable.cs
using UnityEngine;

// Asegurarse de que el script Collectable.cs esté en el objeto con el Collider de la Basura/PowerUp
[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour
{
    [Tooltip("Arrastra aquí el archivo CollectableData (.asset)")]
    public CollectableData data;

    private PowerUpEffectController powerUpEffects;

    void Start()
    {
        // ... (El código de Rigidbody y Trigger se mantiene) ...
    }

    /// <summary>
    /// Maneja la recolección por contacto (cuerpo del jugador o ítem atraído).
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        // 🛑 CRÍTICO: Asegura que la colisión es SOLO con el Tag "Player" (cuerpo del jugador o Magnet Hitbox si también tiene el tag).
        if (!other.CompareTag("Player")) return;

        // Intentamos obtener PlayerController del objeto que colisionó o de su padre
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null)
        {
            player = other.GetComponentInParent<PlayerController>();
        }

        if (player != null)
        {
            // 1. Obtener controlador de efectos para limpieza de lista
            if (powerUpEffects == null)
            {
                powerUpEffects = player.GetComponent<PowerUpEffectController>();
            }

            // 2. Procesar la lógica (Activar PowerUp o Sumar Basura)
            player.ProcessCollectable(data);

            // 3. Limpiar la lista de atracción (si estaba siendo atraído)
            if (powerUpEffects != null)
            {
                // Usamos this.gameObject para referirnos al objeto Collectable
                powerUpEffects.RemoveFromMagnetList(this.gameObject);
            }

            // 4. Destruir el objeto visual (¡Se recoge!)
            Destroy(gameObject);
        }
    }
}