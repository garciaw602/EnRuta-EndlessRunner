using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour
{
    [Tooltip("Arrastra aquí el archivo CollectableData (.asset)")]
    public CollectableData data;

    // Componentes para la lógica de atracción
    private PlayerController player;
    private Rigidbody rb;
    private float attractionSpeed = 15f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
        }

        // Solución para el aviso CS0618: FindFirstObjectByType
        player = FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        // Solo atraer si el Imán está activo, NO somos un PowerUp, y existe el jugador.
        if (player != null && player.isMagnetActive && data.type != CollectableType.PowerUp)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            // La atracción comienza si estamos dentro del radio actual del imán.
            // Ahora usamos la propiedad pública CurrentAttractRadius:
            if (distance < player.CurrentAttractRadius) // <--- ¡Esta es la corrección!
            {
                Vector3 targetPosition = player.transform.position;
                float attractionSpeed = 15f;

                // Mueve el objeto directamente hacia el centro del jugador
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, attractionSpeed * Time.deltaTime);
            }
        }
    
}

    private void OnTriggerEnter(Collider other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();

        if (pc != null)
        {
            // ==========================================================
            // DEBUGGING DETALLADO PARA VER EL TIPO ESPECÍFICO DE RECICLAJE
            // ==========================================================
            if (data != null)
            {
                // La variable 'data.collectableName' contiene 'Plástico', 'Vidrio' o 'Cartón'.
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