using UnityEngine;

// Asegura que el script solo pueda existir si tiene un Rigidbody y un CapsuleCollider
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    // --- COMPONENTES Y REFERENCIAS ---

    [Header("Componentes de Animación")]
    [Tooltip("Arrastra el componente Animator de tu modelo 3D aquí.")]
    public Animator playerAnimator;

    // Conexión con el sistema de generación de niveles
    [Header("Generador de Pista")]
    [Tooltip("Arrastra el script TrackGenerator aquí para la generación modular.")]
    public TrackGenerator generator; // <<-- NUEVA REFERENCIA PARA GENERACIÓN

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    // --- VARIABLES DE MOVIMIENTO Y CARRIL ---

    [Header("Velocidad y Carriles")]
    public float forwardSpeed = 10f;
    public float laneChangeSpeed = 15f;
    public float laneDistance = 3f;

    // Control de Carriles: 0=Izquierda, 1=Centro, 2=Derecha.
    private int currentLane = 1;
    private float targetXPosition;

    // --- VARIABLES DE SALTO Y DESLIZAMIENTO ---

    [Header("Salto y Deslizamiento")]
    public float jumpForce = 8f;
    public float gravityModifier = 1.5f;
    public float slideDuration = 1.0f;

    // FSM: Banderas de estado
    private bool isJumping = false;
    private bool isSliding = false;

    // Valores originales del Collider
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;


    // Start se llama una vez al inicio.
    void Start()
    {
        // Obtener componentes.
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        // Guardar valores originales del Collider.
        if (capsuleCollider != null)
        {
            originalColliderHeight = capsuleCollider.height;
            originalColliderCenter = capsuleCollider.center;
        }

        // Aplicar gravedad personalizada.
        Physics.gravity *= gravityModifier;

        // Iniciar la animación de correr.
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsRunning", true);
        }
    }


    // Update se llama en cada frame. 
    void Update()
    {
        // Solo procesamos el Input si el script está habilitado (no muerto).
        if (this.enabled)
        {
            HandleInput();
        }
    }


    // FixedUpdate se llama a intervalos fijos. Ideal para Físicas (Rigidbody).
    void FixedUpdate()
    {
        // Solo movemos el Player si el script está habilitado (no muerto).
        if (this.enabled)
        {
            MovePlayer();
        }
    }


    // --- MANEJO DE INPUT (TECLAS) ---

    private void HandleInput()
    {
        // 1. Lógica de Cambio de Carril
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentLane > 0) currentLane--;
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentLane < 2) currentLane++;
        }

        // 2. Salto (FSM: Transición a Jumping)
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && !isJumping && !isSliding)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;

            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("Jump");
            }
        }

        // 3. Deslizamiento (FSM: Transición a Sliding)
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && !isJumping && !isSliding)
        {
            StartSliding();
        }

        targetXPosition = (currentLane - 1) * laneDistance;
    }


    // --- APLICACIÓN DEL MOVIMIENTO EN FIXEDUPDATE ---

    private void MovePlayer()
    {
        // Avance constante (Z)
        Vector3 forwardMovement = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;

        // Movimiento Lateral (X)
        float newX = Mathf.Lerp(rb.position.x, targetXPosition, Time.fixedDeltaTime * laneChangeSpeed);

        Vector3 finalPosition = new Vector3(newX, rb.position.y, rb.position.z + forwardMovement.z);

        rb.MovePosition(finalPosition);
    }


    // --- MÉTODOS DE ESTADO DE ACCIÓN ---

    private void StartSliding()
    {
        isSliding = true;

        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Slide");
        }

        // Ajuste de Collider para el Deslizamiento
        capsuleCollider.height /= 2f;
        Vector3 newCenter = new Vector3(originalColliderCenter.x, originalColliderCenter.y / 2f, originalColliderCenter.z);
        capsuleCollider.center = newCenter;

        Invoke("StopSliding", slideDuration);
    }

    private void StopSliding()
    {
        isSliding = false;

        // Reestablecer Collider a sus valores originales
        capsuleCollider.height = originalColliderHeight;
        capsuleCollider.center = originalColliderCenter;
    }

    // --- DETECCIÓN DE COLISIÓN Y TRIGGERS ---

    private void OnCollisionEnter(Collision collision)
    {
        // 1. Lógica para finalizar el Salto (aterrizaje)
        if (isJumping)
        {
            isJumping = false;
        }

        // 2. Lógica de Muerte: si choca con un obstáculo
        // MUERE siempre que choque con el "Obstaculo"
        if (collision.gameObject.CompareTag("Obstaculo"))
        {
            // Cancelamos el deslizamiento pendiente si choca durante el slide
            CancelInvoke("StopSliding");
            Die();
        }
    }

    // **NUEVO** Método para detectar Triggers (como el de aparición de pista)
    private void OnTriggerEnter(Collider other)
    {
        // Lógica de Generación de Pista (Trigger al final del módulo)
        if (other.CompareTag("Trigger") && generator != null)
        {
            // Llama al generador para instanciar el siguiente módulo.
            generator.SpawnModule();

            // Destruye el trigger del módulo viejo para que no se dispare dos veces.
            Destroy(other.gameObject);
        }
    }


    // --- MÉTODO: GESTIÓN DE MUERTE (ESTADO TERMINAL) ---

    private void Die()
    {
        Debug.Log("Game Over: El jugador chocó con un obstáculo.");

        // 1. Deshabilitar Controles (detener el input y movimiento)
        this.enabled = false;

        // Detener completamente la física del jugador
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 2. Cancelar el deslizamiento pendiente (si lo hay)
        CancelInvoke("StopSliding");

        // 3. Activar Animación de Muerte
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Die");
        }

        // 4. (Futuro) Llamar al Game Manager Singleton
        // if (GameManager.Instance != null) { GameManager.Instance.EndGame(); }
    }
}