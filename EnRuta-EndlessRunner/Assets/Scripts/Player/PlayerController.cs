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
    public float gravityMultiplier = 3f;

    [Header("Detección de Suelo")]
    public Transform groundCheck;
    public float groundDistance = 0.3f;
    public LayerMask groundMask;

    [HideInInspector] public float currentSpeedMultiplier = 1f;

    private SlideHandler slideHandler;
    private PowerUpEffectController powerUpEffects;
    private Rigidbody rb;
    private Animator anim;
    private CapsuleCollider playerCollider;

    [Header("Inventario y Estadísticas")]
    public int totalGarbage = 0;
    public int plasticCount = 0;
    public int glassCount = 0;
    public int cardboardCount = 0;

    private bool isGrounded = true;
    private int currentLane = 1;
    private bool isDead = false;
    private bool isJumping = false;
    private bool isFalling = false;

    private PlayerAudioController playerAudio;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider>();
        slideHandler = GetComponent<SlideHandler>();
        powerUpEffects = GetComponent<PowerUpEffectController>();
        playerAudio = GetComponent<PlayerAudioController>();

        if (rb == null || playerCollider == null || anim == null)
        {
            Debug.LogError("Componentes críticos faltantes.");
            enabled = false;
            return;
        }

        if (slideHandler != null)
        {
            slideHandler.Initialize(playerCollider, anim, playerCollider.height, playerCollider.center);
        }

        anim.SetTrigger("IsRun");
    }

    void Update()
    {
        if (isDead || (GameManager.Instance != null && GameManager.Instance.IsGameOver)) return;

        bool canJumpOrSlide = isGrounded && (slideHandler == null || !slideHandler.IsSliding);

        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && canJumpOrSlide)
        {
            Jump();
        }

        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) && canJumpOrSlide)
        {
            slideHandler.StartSlide();
            if (playerAudio != null) playerAudio.PlaySlide(); //
        }

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) MoveLane(1);
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) MoveLane(-1);

        HandleFallingLogic();
    }

    void FixedUpdate()
    {
        if (isDead || (GameManager.Instance != null && GameManager.Instance.IsGameOver))
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        bool wasGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && !wasGrounded)
        {
            isJumping = false;
            isFalling = false;
            anim.ResetTrigger("IsJump");
            anim.SetBool("IsFalling", false);
            anim.SetBool("IsLanding", true);
        }

        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * (Physics.gravity.magnitude * gravityMultiplier * rb.mass), ForceMode.Force);
        }

        float finalSpeed = baseSpeed * currentSpeedMultiplier;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, finalSpeed);

        float targetX = (currentLane - 1) * laneDistance;
        float newX = Mathf.Lerp(rb.position.x, targetX, Time.fixedDeltaTime * lateralSpeed);
        rb.MovePosition(new Vector3(newX, rb.position.y, rb.position.z));
    }

    // --- ESTAS SON LAS FUNCIONES QUE TE FALTABAN DENTRO DE LAS LLAVES ---

    private void HandleFallingLogic()
    {
        if (!isGrounded && !isFalling && rb.linearVelocity.y < -0.1f)
        {
            isFalling = true;
            isJumping = false;
            anim.SetBool("IsFalling", true);
            anim.SetBool("IsLanding", false);
        }
    }

    private void Jump()
    {
        if (!isGrounded || isJumping) return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        isJumping = true;
        isFalling = false;
        anim.SetBool("IsLanding", false);
        anim.SetBool("IsFalling", false);
        if (playerAudio != null) playerAudio.PlayJump(); //
        anim.SetTrigger("IsJump");
    }

    private void MoveLane(int direction)
    {
        int newLane = currentLane + direction;
        if (newLane >= 0 && newLane <= 2) currentLane = newLane;
    }

    public void ProcessCollectable(CollectableData data)
    {
        if (data.type == CollectableType.PowerUp)
        {
            if (data.powerUpEffect != null && powerUpEffects != null)
            {
                data.powerUpEffect.ApplyEffect(powerUpEffects, data.powerUpEffect.duration);
            }
            return;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddToInventory(data.collectableName, data.baseValue, data.type);
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // --- LÓGICA DE AUDIO AL MORIR ---
        if (playerAudio != null)
        {
            playerAudio.StopRunLoop(); // Detiene el sonido de pasos
            playerAudio.PlayDie();     // Suena el grito/muerte
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopBackgroundMusic(); // Detiene la música de fondo
        }




        anim.SetTrigger("Die");
        if (GameManager.Instance != null) GameManager.Instance.GameOver();





    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstaculo")) Die();
    }

    void OnTriggerEnter(Collider other)
    {
        // 1. Verificamos si es un obstáculo
        if (other.CompareTag("Obstaculo"))
        {
            // 2. FILTRO DE SEGURIDAD: Solo muere si el objeto que tocó el obstáculo 
            // es el que tiene este script (el cuerpo del Player), no sus sensores hijos.
            // Comparamos si el 'other' entró en contacto con nuestro CapsuleCollider.
            if (playerCollider != null && playerCollider.bounds.Intersects(other.bounds))
            {
                Die();
            }
            return; // Salimos para no procesar el resto si ya morimos
        }

        // Lógica de coleccionables se mantiene igual
        Collectable item = other.GetComponent<Collectable>();
        if (item != null) item.AttemptCollection(this);
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}