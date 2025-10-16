using UnityEngine;

// Asegura que el script solo pueda existir si tiene un Rigidbody y un CapsuleCollider
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    // --- COMPONENTES Y REFERENCIAS ---

    [Header("Componentes de Animaci�n")]
    [Tooltip("Arrastra el componente Animator de tu modelo 3D aqu�.")]
    public Animator playerAnimator;

    // Conexi�n con el sistema de generaci�n de niveles
    [Header("Generador de Pista")]
    [Tooltip("Arrastra el script TrackGenerator aqu� para la generaci�n modular.")]
    public TrackGenerator generator; // <<-- NUEVA REFERENCIA PARA GENERACI�N

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

        // Iniciar la animaci�n de correr.
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("IsRunning", true);
        }
    }


    // Update se llama en cada frame. 
    void Update()
    {
        // Solo procesamos el Input si el script est� habilitado (no muerto).
        if (this.enabled)
        {
            HandleInput();
        }
    }


    // FixedUpdate se llama a intervalos fijos. Ideal para F�sicas (Rigidbody).
    void FixedUpdate()
    {
        // Solo movemos el Player si el script est� habilitado (no muerto).
        if (this.enabled)
        {
            MovePlayer();
        }
    }


    // --- MANEJO DE INPUT (TECLAS) ---

    private void HandleInput()
    {
        // 1. L�gica de Cambio de Carril
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentLane > 0) currentLane--;
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentLane < 2) currentLane++;
        }

        // 2. Salto (FSM: Transici�n a Jumping)
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && !isJumping && !isSliding)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;

            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("Jump");
            }
        }

        // 3. Deslizamiento (FSM: Transici�n a Sliding)
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && !isJumping && !isSliding)
        {
            StartSliding();
        }

        targetXPosition = (currentLane - 1) * laneDistance;
    }


    // --- APLICACI�N DEL MOVIMIENTO EN FIXEDUPDATE ---

    private void MovePlayer()
    {
        // Avance constante (Z)
        Vector3 forwardMovement = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;

        // Movimiento Lateral (X)
        float newX = Mathf.Lerp(rb.position.x, targetXPosition, Time.fixedDeltaTime * laneChangeSpeed);

        Vector3 finalPosition = new Vector3(newX, rb.position.y, rb.position.z + forwardMovement.z);

        rb.MovePosition(finalPosition);
    }


    // --- M�TODOS DE ESTADO DE ACCI�N ---

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

    // --- DETECCI�N DE COLISI�N Y TRIGGERS ---

    private void OnCollisionEnter(Collision collision)
    {
        // 1. L�gica para finalizar el Salto (aterrizaje)
        if (isJumping)
        {
            isJumping = false;
        }

        // 2. L�gica de Muerte: si choca con un obst�culo
        // MUERE siempre que choque con el "Obstaculo"
        if (collision.gameObject.CompareTag("Obstaculo"))
        {
            // Cancelamos el deslizamiento pendiente si choca durante el slide
            CancelInvoke("StopSliding");
            Die();
        }
    }

    // **NUEVO** M�todo para detectar Triggers (como el de aparici�n de pista)
    private void OnTriggerEnter(Collider other)
    {
        // L�gica de Generaci�n de Pista (Trigger al final del m�dulo)
        if (other.CompareTag("Trigger") && generator != null)
        {
            // Llama al generador para instanciar el siguiente m�dulo.
            generator.SpawnModule();

            // Destruye el trigger del m�dulo viejo para que no se dispare dos veces.
            Destroy(other.gameObject);
        }
    }


    // --- M�TODO: GESTI�N DE MUERTE (ESTADO TERMINAL) ---

    private void Die()
    {
        Debug.Log("Game Over: El jugador choc� con un obst�culo.");

        // 1. Deshabilitar Controles (detener el input y movimiento)
        this.enabled = false;

        // Detener completamente la f�sica del jugador
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 2. Cancelar el deslizamiento pendiente (si lo hay)
        CancelInvoke("StopSliding");

        // 3. Activar Animaci�n de Muerte
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Die");
        }

        // 4. (Futuro) Llamar al Game Manager Singleton
        // if (GameManager.Instance != null) { GameManager.Instance.EndGame(); }
    }
}