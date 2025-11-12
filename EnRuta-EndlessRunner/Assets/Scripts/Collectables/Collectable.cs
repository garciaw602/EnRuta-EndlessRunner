using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Collectable : MonoBehaviour
{
    [Tooltip("Arrastra aquí el archivo CollectableData (.asset)")]
    public CollectableData data;

    // Componentes para la lógica de atracción
    private PlayerController player;
    private PowerUpEffectController powerUpEffects; // 
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

        // Obtener la referencia del jugador y su controlador de efectos
        player = FindFirstObjectByType<PlayerController>();
        if (player != null)
        {
            // OBTENER LA REFERENCIA AL CONTROLADOR DE EFECTOS
            powerUpEffects = player.GetComponent<PowerUpEffectController>();
        }
    }

    void Update()
    {
        // 1. Lógica del Imán (Atracción y Recolección por Proximidad)
        // Verifica si el PowerUpEffectController existe y si el imán está activo.
        if (player != null && powerUpEffects != null && powerUpEffects.isMagnetActive && data.type != CollectableType.PowerUp)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            // Umbral de Recolección por proximidad
            if (distance < 0.15f)
            {
                player.ProcessCollectable(data);
                Destroy(gameObject);
                return;
            }

            // 2. Atracción Visible
            //  Usamos powerUpEffects.CurrentAttractRadius
            if (distance < powerUpEffects.CurrentAttractRadius)
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
            // OBTENEMOS la referencia local del controlador de efectos del jugador
            PowerUpEffectController pcEffects = pc.GetComponent<PowerUpEffectController>();

            // El objeto se recoge si: 1) el Imán NO está activo O 2) es un PowerUp (siempre se recoge al contacto).
            //  Usamos pcEffects.isMagnetActive
            bool isManualCollection = (pcEffects == null || !pcEffects.isMagnetActive);
            bool isPowerUp = data.type == CollectableType.PowerUp;

            if (isManualCollection || isPowerUp)
            {
                pc.ProcessCollectable(data);
                Destroy(gameObject);
            }
        }
    }
}