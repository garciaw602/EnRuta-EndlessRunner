using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour
{
    public CollectableData data;

    // Componentes para la lógica de atracción
    private PlayerController player;
    private PowerUpEffectController powerUpEffects; // FIX: NUEVA REFERENCIA
    private Rigidbody rb;
    private float attractionSpeed = 500f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        player = FindFirstObjectByType<PlayerController>();

        // FIX: Obtener el componente PowerUpEffectController del jugador
        if (player != null)
        {
            powerUpEffects = player.GetComponent<PowerUpEffectController>();
        }

        if (powerUpEffects == null)
        {
            Debug.LogWarning("Collectable no encontró PowerUpEffectController. La lógica de imán fallará.");
        }
    }

    void Update()
    {
        // 1. Lógica del Imán (Atracción y Recolección por Proximidad)
        // FIX: Comprueba si existe y si el imán está activo en el PowerUpEffectController
        if (player != null && powerUpEffects != null && powerUpEffects.isMagnetActive && data.type != CollectableType.PowerUp)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            // Umbral de Recolección
            if (distance < 0.15f)
            {
                player.ProcessCollectable(data);
                Destroy(gameObject);
                return;
            }

            // 2. Lógica de Atracción
            // FIX: Usa el radio del PowerUpEffectController
            if (distance < powerUpEffects.CurrentAttractRadius)
            {
                Vector3 targetPosition = player.transform.position;
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, attractionSpeed * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();

        if (pc != null)
        {
            // FIX: Obtener el PowerUpEffectController para verificar el estado del imán en TriggerEnter
            PowerUpEffectController pufx = pc.GetComponent<PowerUpEffectController>();

            bool isMagnetActive = (pufx != null) ? pufx.isMagnetActive : false;

            // Solo procesamos si el Imán NO está activo O si es un PowerUp.
            bool isManualCollection = !isMagnetActive;
            bool isPowerUp = data.type == CollectableType.PowerUp;

            if (isManualCollection || isPowerUp)
            {
                if (data != null)
                {
                    Debug.Log($"Recolectado: {data.collectableName} | Tipo General: {data.type}");
                }
                else
                {
                    Debug.LogError($"Error: CollectableData no está asignado en {gameObject.name}");
                }

                pc.ProcessCollectable(data);
                Destroy(gameObject);
            }
        }
    }
}