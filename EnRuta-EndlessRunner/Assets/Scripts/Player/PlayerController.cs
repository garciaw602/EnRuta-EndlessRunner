using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    // --- 1. CONFIGURACIÓN DE MOVIMIENTO BASE ---
    [Header("Configuración de Movimiento Base")]
    public float baseSpeed = 10f;
    public float lateralSpeed = 5f;
    public float jumpForce = 10f;
    public float laneDistance = 4f; // Distancia entre carriles (ej. 4 metros)

    [HideInInspector] public float currentSpeedMultiplier = 1f;

    // --- 2. REFERENCIAS Y COMPONENTES ---
    private SlideHandler slideHandler;
    private PowerUpEffectController powerUpEffects;
    private Rigidbody rb;
    private Animator anim;
    private CapsuleCollider playerCollider;
    private GameManager gameManager; // Referencia al GameManager

    // --- 3. VARIABLES DE ESTADO ---
    private bool isGrounded = true;
    private int currentLane = 1; // 0: Izquierda, 1: Centro, 2: Derecha
    private bool isDead = false;

    // --- 4. INICIALIZACIÓN (Start/Awake) ---
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider>();
        slideHandler = GetComponent<SlideHandler>();
        powerUpEffects = GetComponent<PowerUpEffectController>();
        gameManager = GameManager.Instance; // Obtener la instancia del GameManager

        if (rb == null || playerCollider == null || anim == null || gameManager == null)
        {
            Debug.LogError("[PlayerController] Faltan componentes o GameManager. Deshabilitando PlayerController.");
            enabled = false;
            return;
        }

        if (slideHandler != null)
        {
            // Asume que SlideHandler tiene un método Initialize (como el que ya tenías)
            slideHandler.Initialize(playerCollider, anim, playerCollider.height, playerCollider.center);
        }

        anim.SetBool("IsRunning", true);
    }

    // --- 5. LÓGICA DE CONTROL (Update) ---
    void Update()
    {
        if (isDead || gameManager.IsGameOver) return;

        // ------------------------------------
        // 🚀 LÓGICA DE INPUT (RECUPERADA) 🚀
        // ------------------------------------

        // --- Movimiento Lateral (Izquierda) ---
        // Tecla A o Flecha Izquierda
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ChangeLane(-1);
        }

        // --- Movimiento Lateral (Derecha) ---
        // Tecla D o Flecha Derecha
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeLane(1);
        }

        // --- Salto --- (Espaciador, W, o Flecha Arriba)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            Jump();
        }

        // --- Deslizamiento --- (S o Flecha Abajo)
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && slideHandler != null)
        {
            slideHandler.StartSlide();
        }

        MovePlayer();
    }

    // --- 6. MÉTODOS DE MOVIMIENTO ---

    void ChangeLane(int direction)
    {
        // Calculamos el nuevo carril (0: Izquierda, 1: Centro, 2: Derecha)
        int newLane = currentLane + direction;

        // Clamp asegura que el carril se mantenga entre 0 y 2
        currentLane = Mathf.Clamp(newLane, 0, 2);

        // Opcional: Ejecuta animación de carril 
        if (anim != null)
        {
            if (direction < 0) anim.SetTrigger("MoveLeft");
            if (direction > 0) anim.SetTrigger("MoveRight");
        }
    }

    void MovePlayer()
    {
        // Movimiento hacia adelante (constante, afectado por Power-Ups)
        float speed = baseSpeed * currentSpeedMultiplier;
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Movimiento lateral (interpolación hacia la posición X del carril)
        // Carriles: 0 -> -laneDistance, 1 -> 0, 2 -> +laneDistance
        float targetX = (currentLane - 1) * laneDistance;
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);

        // Movemos la posición X suavemente hacia el objetivo
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, lateralSpeed * Time.deltaTime);
    }

    public void Jump()
    {
        if (isGrounded && !isDead)
        {
            isGrounded = false;
            // Aplicamos la fuerza de salto
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Opcional: Ejecuta animación de salto
            if (anim != null) anim.SetTrigger("Jump");
        }
    }

    // --- 7. LÓGICA DE COLISIÓN Y MUERTE ---

    private void OnCollisionEnter(Collision collision)
    {
        // Reestablecer isGrounded al tocar el suelo
        // Asegúrate de que tus segmentos de camino tengan el Tag "Ground"
        if (collision.gameObject.CompareTag("Ground") && !isGrounded)
        {
            isGrounded = true;

            // Opcional: Ejecuta animación de aterrizaje
            if (anim != null) anim.SetTrigger("Land");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 🔴 Lógica de Muerte (Si choca con un Trigger de Obstáculo)
        if (other.CompareTag("Obstaculo") || other.CompareTag("Obstaculo"))
        {
            Die();
            other.enabled = false; // Desactiva el collider del obstáculo para evitar colisiones múltiples
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        // Notificar al GameManager que el juego ha terminado
        if (gameManager != null)
        {
            gameManager.GameOver();
        }

        // Detener el movimiento y activar animación de muerte
        anim.SetTrigger("Die");

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        enabled = false; // Deshabilitar el script para detener el Update
    }


    // --- 8. LÓGICA DE COLECCIÓN DELEGADA ---

    public void ProcessCollectable(CollectableData data)
    {
        // 1. Manejo de Power-Ups
        if (data.type == CollectableType.PowerUp)
        {
            if (data.powerUpEffect != null && powerUpEffects != null)
            {
                // Aplica el efecto del SO al controlador
                // NOTA: Esto asume que ApplyEffect ya pasa los argumentos necesarios (como la duración)
                data.powerUpEffect.ApplyEffect(powerUpEffects, data.powerUpEffect.duration);
            }
            return;
        }

        // 2. Manejo de Basura/Reciclables
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddToInventory(data.collectableName, data.baseValue, data.type);
        }
    }
}