using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour
{
    [Tooltip("Arrastra aquí el archivo CollectableData (.asset)")]
    public CollectableData data;

    // Componentes para la lógica de atracción
    private PlayerController player;
    private Rigidbody rb;
    // Velocidad de atracción muy alta para simular la inmediatez
    private float attractionSpeed = 500f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        // Obtener la referencia del jugador
        player = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        // 1. Lógica del Imán (Atracción y Recolección por Proximidad)
        if (player != null && player.isMagnetActive && data.type != CollectableType.PowerUp)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            //  Umbral de Recolección (0.3f)
            // Esto garantiza que el collectible se recoja justo antes de ser arrastrado.
            if (distance < 0.15f)
            {
                player.ProcessCollectable(data);
                Destroy(gameObject);
                return;
            }

            // 2. Atracción Visible (Si está en rango, pero aún no cerca)
            if (distance < player.CurrentAttractRadius)
            {
                Vector3 targetPosition = player.transform.position;

                // Mueve el objeto hacia el centro del jugador
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, attractionSpeed * Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Esta función solo maneja la recolección manual (sin imán) y los Power-ups.

        PlayerController pc = other.GetComponent<PlayerController>();

        if (pc != null)
        {
            // Solo procesamos si el Imán NO está activo O si el objeto recolectado es un PowerUp.
            bool isManualCollection = !pc.isMagnetActive;
            bool isPowerUp = data.type == CollectableType.PowerUp;

            // Si es recolección manual (sin imán) O es un PowerUp (que siempre se recogen al contacto)
            if (isManualCollection || isPowerUp)
            {
                // DEBUGGING DETALLADO 
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
            // Si el Imán está activo y es un Collectible (no un PowerUp), la recolección se hace por proximidad en Update().
        }
    }
}