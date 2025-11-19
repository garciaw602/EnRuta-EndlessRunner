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

    [HideInInspector] public float currentSpeedMultiplier = 1f;

    // --- REFERENCIAS ---
    private SlideHandler slideHandler;
    private PowerUpEffectController powerUpEffects;
    private Rigidbody rb;
    private Animator anim;
    private CapsuleCollider playerCollider;

    // Variables de estado
    private bool isGrounded = true;
    private int currentLane = 1;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider>();
        slideHandler = GetComponent<SlideHandler>();
        powerUpEffects = GetComponent<PowerUpEffectController>();

        if (rb == null || playerCollider == null || anim == null)
        {
            enabled = false;
            return;
        }

        if (slideHandler != null)
        {
            slideHandler.Initialize(playerCollider, anim, playerCollider.height, playerCollider.center);
        }

        anim.SetBool("IsRunning", true);
    }

    void Update()
    {
        bool canMove = !isDead && (GameManager.Instance != null && !GameManager.Instance.IsGameOver);
        bool canJumpOrSlide = isGrounded && slideHandler != null && !slideHandler.IsSliding;

        if (!canMove) return;

        // Inputs de Movimiento
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && canJumpOrSlide)
            Jump();

        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && canJumpOrSlide)
            slideHandler.StartSlide();

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) MoveLane(1);
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) MoveLane(-1);
    }

    void FixedUpdate()
    {
        if (isDead || (GameManager.Instance != null && GameManager.Instance.IsGameOver))
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        float finalSpeed = baseSpeed * currentSpeedMultiplier;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, finalSpeed);

        float targetX = (currentLane - 1) * laneDistance;
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
        Vector3 newPosition = Vector3.Lerp(rb.position, targetPosition, Time.fixedDeltaTime * lateralSpeed);
        rb.MovePosition(new Vector3(newPosition.x, rb.position.y, rb.position.z));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("IsJumping", false);
        }

        if (collision.gameObject.CompareTag("Obstaculo") && !isDead)
        {
            Die();
        }
    }

    // --- LÓGICA DE COLECCIÓN (CONEXIÓN CON SCOREMANAGER) ---
    // Este método es el que recibe la llamada del Imán o del choque directo.
    public void ProcessCollectable(CollectableData data)
    {
        // 1. Lógica de Power-Up (Se queda aquí porque afecta al jugador)
        if (data.type == CollectableType.PowerUp)
        {
            if (data.powerUpEffect != null && powerUpEffects != null)
            {
                data.powerUpEffect.ApplyEffect(powerUpEffects, data.powerUpEffect.duration);
            }
            return;
        }

        // 2. Lógica de Puntuación (SE DELEGA AL SCOREMANAGER)
        
        if (ScoreManager.Instance != null)
        {
            // Llamamos a AddToInventory tal como está definido en tu archivo ScoreManager.cs
            ScoreManager.Instance.AddToInventory(data.collectableName, data.baseValue, data.type);
        }
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

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        anim.SetTrigger("Die");

        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndGame();
        }
    }
}