using UnityEngine;

// Aseguramos que el script solo pueda existir si tiene un Rigidbody y un CapsuleCollider
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    // --- COMPONENTES ---

    [Header("Componentes de Animación")]
    [Tooltip("Arrastra el componente Animator de tu modelo 3D aquí.")]
    public Animator playerAnimator; // ¡Importante: Asignar en el Inspector!

    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    // --- VARIABLES DE MOVIMIENTO Y CARRIL ---

    [Header("Velocidad y Carriles")]
    [Tooltip("Velocidad constante de avance del jugador (eje Z).")]
    public float forwardSpeed = 10f;

    [Tooltip("Velocidad de movimiento lateral al cambiar de carril (suavizado/Lerp).")]
    public float laneChangeSpeed = 15f;

    [Tooltip("Distancia lateral entre cada carril.")]
    public float laneDistance = 3f;

    // Control de Carriles: 0=Izquierda, 1=Centro, 2=Derecha.
    private int currentLane = 1;
    private float targetXPosition;

    // --- VARIABLES DE SALTO Y DESLIZAMIENTO ---

    [Header("Salto y Deslizamiento")]
    [Tooltip("Fuerza vertical inicial aplicada al saltar.")]
    public float jumpForce = 8f;

    [Tooltip("Multiplicador de gravedad para una caída más rápida (efecto arcade).")]
    public float gravityModifier = 1.5f;

    [Tooltip("Duración, en segundos, del estado de deslizamiento.")]
    public float slideDuration = 1.0f;

    // FSM: Banderas de estado
    private bool isJumping = false;
    private bool isSliding = false;

    // Valores originales del Collider para el deslizamiento
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

        // Iniciar la animación de correr si el Animator está asignado.
        if (playerAnimator != null)
        {
            // Usamos "IsRunning" (Bool) para indicar que el estado base es correr.
            playerAnimator.SetBool("IsRunning", true);
        }
    }


    // Update se llama en cada frame. Ideal para Input.
    void Update()
    {
        HandleInput();
    }


    // FixedUpdate se llama a intervalos fijos. Ideal para Físicas (Rigidbody).
    void FixedUpdate()
    {
        MovePlayer();
    }


    // --- MANEJO DE INPUT (TECLAS) ---

    private void HandleInput()
    {
        // 1. Lógica de Cambio de Carril (A/D o Flechas Izquierda/Derecha)
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentLane > 0) currentLane--;
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentLane < 2) currentLane++;
        }

        // 2. Salto (FSM: Transición a Jumping)
        // Solo si no está ya saltando o deslizándose.
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && !isJumping && !isSliding)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;

            // >> CONTROL DE ANIMACIÓN: Activa el Trigger para pasar a la animación de Salto.
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

        // Calcular la posición X objetivo final para la transición de carril.
        targetXPosition = (currentLane - 1) * laneDistance;
    }


    // --- APLICACIÓN DEL MOVIMIENTO EN FIXEDUPDATE ---

    private void MovePlayer()
    {
        // 1. Movimiento Constante hacia Adelante (Eje Z)
        Vector3 forwardMovement = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;

        // 2. Movimiento Lateral (Transición Suave con Lerp)
        // newX se mueve suavemente desde la posición actual hasta el carril objetivo.
        float newX = Mathf.Lerp(rb.position.x, targetXPosition, Time.fixedDeltaTime * laneChangeSpeed);

        // Vector final: nueva posición X (suave) y avance en Z (constante).
        Vector3 finalPosition = new Vector3(newX, rb.position.y, rb.position.z + forwardMovement.z);

        // Mueve el Rigidbody (preferido para movimiento cinemático).
        rb.MovePosition(finalPosition);
    }


    // --- MÉTODOS DE ESTADO DE ACCIÓN ---

    private void StartSliding()
    {
        isSliding = true;

        // >> CONTROL DE ANIMACIÓN: Activa el Trigger para pasar a la animación de Deslizamiento.
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger("Slide");
        }

        // 1. Ajustar el Capsule Collider (reducir altura a la mitad).
        capsuleCollider.height /= 2f;

        // 2. Ajustar el centro del Collider (moverlo hacia abajo).
        Vector3 newCenter = new Vector3(originalColliderCenter.x, originalColliderCenter.y / 2f, originalColliderCenter.z);
        capsuleCollider.center = newCenter;

        // Programar la función StopSliding para que se ejecute al final de la duración.
        Invoke("StopSliding", slideDuration);
    }

    private void StopSliding()
    {
        isSliding = false;

        // Reestablecer la altura y el centro del Collider a sus valores originales.
        capsuleCollider.height = originalColliderHeight;
        capsuleCollider.center = originalColliderCenter;
    }


    // --- DETECCIÓN DE COLISIÓN (Para Finalizar el Salto) ---

    private void OnCollisionEnter(Collision collision)
    {
        // Si estábamos saltando y colisionamos, asumimos que aterrizamos en el suelo.
        if (isJumping)
        {
            isJumping = false; // Vuelve al estado Running
        }
    }
}