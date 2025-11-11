using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour
{
    // Datos definidos por el Scriptable Object (SO)
    public CollectableData data;

    // Referencia al controlador de efectos del jugador para la limpieza del imán.
    private PowerUpEffectController powerUpEffects; 
    
    void Start()
    {
        if (data == null)
        {
            Debug.LogError($"Collectable en {gameObject.name} no tiene asignado CollectableData. ¡Esto causará fallos!");
        }
        
        // Búsqueda de la referencia del PlayerController
        GameObject playerGO = GameObject.FindWithTag("Player");
        if (playerGO != null)
        {
            powerUpEffects = playerGO.GetComponent<PowerUpEffectController>();
        }
        else
        {
            Debug.LogError("Collectable no encontró el objeto 'Player'. ¿Tiene la etiqueta 'Player'?");
        }

        // Asegura que el Collider sea Trigger para que el jugador pueda atravesarlo y recolectarlo.
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    // EL MÉTODO Update() FUE ELIMINADO para centralizar el movimiento en PowerUpEffectController.

    /// <summary>
    /// Intenta recolectar el objeto, llamado por el PlayerController al chocar con su Collider de cuerpo.
    /// </summary>
    public void AttemptCollection(PlayerController pc)
    {
        if (data == null) return;

        // Obtiene el estado actual del imán.
        PowerUpEffectController pufx = pc.GetComponent<PowerUpEffectController>();
        bool isMagnetActive = (pufx != null) ? pufx.isMagnetActive : false;

        // LÓGICA DE RECOLECCIÓN SIMPLIFICADA Y CORREGIDA PARA ANIMACIÓN:
        // Solo permitimos la recolección por contacto (el cuerpo del jugador) si el imán NO está activo.
        // Si el imán está activo, la recolección la gestionará el PowerUpEffectController.Update(), 
        // lo que permite ver la animación de atracción.
        bool shouldCollect = !isMagnetActive;


        if (shouldCollect)
        {
            // LIMPIEZA DE SEGURIDAD: Retira el objeto de la lista de atracción del imán (si estaba).
            if (powerUpEffects != null)
            {
                powerUpEffects.RemoveAttractableObject(gameObject);
            }

            pc.ProcessCollectable(data); // Aplica el efecto o suma el puntaje
            Destroy(gameObject); // Destruye el objeto recolectado
        }
        
        // Si el imán está activo, la función termina aquí. El objeto es movido y recolectado
        // por PowerUpEffectController.Update() después de ser animado.
    }
}