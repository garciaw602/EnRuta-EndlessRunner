using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Configuración de Movimiento Base")]
    public float baseSpeed = 10f;
    public float lateralSpeed = 5f;
    public float jumpForce = 10f;
    public float laneDistance = 4f;

    // currentSpeedMultiplier se mantiene aquí para ser modificado por PowerUpEffectController.
    [HideInInspector] public float currentSpeedMultiplier = 1f;

    // --- REFERENCIAS A COMPONENTES ---
    private SlideHandler slideHandler; // Manejador de deslizamiento
    private PowerUpEffectController powerUpEffects; // Manejador de efectos temporales
    private Rigidbody rb;
    private Animator anim;
    private CapsuleCollider playerCollider;
    // -------------------------------------------

    // 🛑 ELIMINADAS VARIABLES DE INVENTARIO (Delegadas a ScoreManager)
    // [Header("Inventario y Estadísticas")]
    // public int totalGarbage = 0;
    // public int plasticCount = 0;
    // public int glassCount = 0;
    // public int cardboardCount = 0;

    // Variables de estado
    private bool isGrounded = true;
    private int currentLane = 1; // 0: Izq, 1: Centro, 2: Der
    private bool isDead = false;

    void Start()
    {
        // 1. Obtención de Componentes Propios
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider>();

        // 2. Obtención de Componentes Refactorizados
        slideHandler = GetComponent<SlideHandler>();
        powerUpEffects = GetComponent<PowerUpEffectController>();

        // 3. Verificaciones CRÍTICAS (Asegura que el movimiento/animación funcionen)
        if (rb == null || playerCollider == null || anim == null)
        {
            Debug.LogError("FATAL: Componente Rigidbody, CapsuleCollider o Animator FALTANTE. El movimiento NO funcionará.");
            enabled = false; // Desactiva el script si falta un componente crítico
            return;
        }

        // 4. Inicialización del Slide Handler (Crucial para el deslizamiento y salto)
        if (slideHandler != null)
        {
            // Pasa las referencias y dimensiones originales para que el handler pueda manipular el collider
            float originalHeight = playerCollider.height;
            Vector3 originalCenter = playerCollider.center;
            slideHandler.Initialize(playerCollider, anim, originalHeight, originalCenter);
        }
        else
        {

            Debug.LogError("FATAL: SlideHandler.cs no está adjunto. Deslizamiento y Salto fallarán.");
        }

        if (powerUpEffects == null)
        {
            Debug.LogWarning("Advertencia: PowerUpEffectController.cs no está adjunto. Los power-ups no funcionarán.");
        }

        anim.SetBool("IsRunning", true);
    }

    void Update()
    {
        bool canMove = !isDead && (GameManager.Instance != null && !GameManager.Instance.IsGameOver);
        // Usa la propiedad IsSliding del SlideHandler
        bool canJumpOrSlide = isGrounded && slideHandler != null && !slideHandler.IsSliding;

        if (!canMove) return;

        // 1. Lógica de Salto
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            && canJumpOrSlide)
        {
            Jump();
        }

        // 2. Lógica de Deslizamiento (DELEGADA)
        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            && canJumpOrSlide)
        {
            slideHandler.StartSlide(); // DELEGA la lógica
        }

        // 3. Lógica de Movimiento Lateral (Input)
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveLane(1);
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLane(-1);
        }
    }

    void FixedUpdate()
    {
        if (isDead || (GameManager.Instance != null && GameManager.Instance.IsGameOver))
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        // 1. Avance Constante (Eje Z)
        float finalSpeed = baseSpeed * currentSpeedMultiplier;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, finalSpeed);

        // 2. Movimiento Lateral (Eje X)
        float targetX = (currentLane - 1) * laneDistance;
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);

        Vector3 newPosition = Vector3.Lerp(rb.position, targetPosition, Time.fixedDeltaTime * lateralSpeed);

        rb.MovePosition(new Vector3(newPosition.x, rb.position.y, rb.position.z));
    }

    // --- MANEJO DE COLISIONES (GAME OVER / SALTO) ---
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // Permite saltar de nuevo
            anim.SetBool("IsJumping", false);
        }

        // Si choca con un Rigidbody (sólido)
        if (collision.gameObject.CompareTag("Obstaculo") && !isDead)
        {
            Die();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Collectable item = other.GetComponent<Collectable>();

        if (item != null)
        {
            ProcessCollectable(item.data);
            return;
        }

    }

    // --- LÓGICA DE COLECCIÓN (DELEGADA) ---
    public void ProcessCollectable(CollectableData data)
    {
        // 1. Lógica de Power-Up (Delegada al Strategy y al PowerUpEffectController)
        if (data.type == CollectableType.PowerUp)
        {
            if (data.powerUpEffect != null && powerUpEffects != null)
            {
                
                // 'powerUpEffects' es de tipo PowerUpEffectController, que es lo que espera el contrato.
                data.powerUpEffect.ApplyEffect(powerUpEffects, data.powerUpEffect.duration);
            }
        }

        // 2. Lógica de Inventario (DELEGADA COMPLETAMENTE a ScoreManager, Tarea 2.1)
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddToInventory(data.collectableName, data.baseValue, data.type);
        }

        // Destrucción del objeto (pendiente de tu lógica final en Collectable.cs)
        // ...
    }


    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        anim.SetBool("IsJumping", true);
    }

    private void MoveLane(int direction)
    {
        int newLane = currentLane + direction;
        if (newLane >= 0 && newLane <= 2)
        {
            currentLane = newLane;
        }
    }

    // --- LÓGICA DE MUERTE (TAREA 1.2) ---
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // 1. Inicia la animación de muerte (se ejecuta en el próximo frame de Update)
        anim.SetTrigger("Die");

        // 2. Llama al GameManager para que maneje la pausa y los eventos
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndGame();
        }

        Debug.Log("¡GAME OVER!");
    }
}