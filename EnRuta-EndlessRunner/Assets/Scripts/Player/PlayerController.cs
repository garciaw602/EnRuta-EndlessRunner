﻿using UnityEngine;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour
{
    [Header("Configuración de Movimiento Base")]
    public float baseSpeed = 10f;
    public float lateralSpeed = 5f;
    public float jumpForce = 10f;
    public float laneDistance = 4f;
    public float gravityMultiplier = 3f; // Multiplicador de gravedad para caídas rápidas

    // currentSpeedMultiplier se mantiene aquí para ser modificado por PowerUpEffectController.
    [HideInInspector] public float currentSpeedMultiplier = 1f;

    // --- REFERENCIAS A COMPONENTES ---
    private SlideHandler slideHandler; // Manejador de deslizamiento
    private PowerUpEffectController powerUpEffects; // Manejador de efectos temporales
    private Rigidbody rb;
    private Animator anim;
    private CapsuleCollider playerCollider;
    // -------------------------------------------

    [Header("Inventario y Estadísticas")]
    public int totalGarbage = 0;
    public int plasticCount = 0;
    public int glassCount = 0;
    public int cardboardCount = 0;

    // Variables de estado
    private bool isGrounded = true;
    private int currentLane = 1; // 0: Izq, 1: Centro, 2: Der
    private bool isDead = false;
    
    // Estados de salto/caída
    private bool isJumping = false;
    private bool isFalling = false;
    private float fallThreshold = -0.5f;

    void Start()
    {
        // 1. Obtención de Componentes Propios
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider>();

        // 2. Obtención de Componentes Refactorizados
        slideHandler = GetComponent<SlideHandler>();
        powerUpEffects = GetComponent<PowerUpEffectController>();

        // 3. Verificaciones CRÍTICAS
        if (rb == null || playerCollider == null || anim == null)
        {
            Debug.LogError("FATAL: Componente Rigidbody, CapsuleCollider o Animator FALTANTE.");
            enabled = false; 
            return;
        }

        // 4. Inicialización del Slide Handler
        if (slideHandler != null)
        {
            float originalHeight = playerCollider.height;
            Vector3 originalCenter = playerCollider.center;
            slideHandler.Initialize(playerCollider, anim, originalHeight, originalCenter);
        }
        
        anim.SetTrigger("IsRun");
    }

    void Update()
    {
        bool canMove = !isDead && (GameManager.Instance != null && !GameManager.Instance.IsGameOver);
        bool canJumpOrSlide = isGrounded && slideHandler != null && !slideHandler.IsSliding;

        if (!canMove) return;

        // 1. Lógica de Salto
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            && canJumpOrSlide)
        {
            Jump();
        }

        // 2. Lógica de Deslizamiento
        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            && canJumpOrSlide)
        {
            slideHandler.StartSlide(); 
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

        // 4. Monitorear fases de salto/caída
        UpdateJumpAnimationState();
    }

    void FixedUpdate()
    {
        if (isDead || (GameManager.Instance != null && GameManager.Instance.IsGameOver))
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        // --- MULTIPLICADOR DE GRAVEDAD ---
        if (!isGrounded)
        {
            rb.AddForce(Vector3.down * (Physics.gravity.magnitude * gravityMultiplier * rb.mass), ForceMode.Force);
        }

        // 1. Avance Constante (Eje Z)
        float finalSpeed = baseSpeed * currentSpeedMultiplier;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, finalSpeed);

        // 2. Movimiento Lateral (Eje X) - FUNCIONA EN EL AIRE
        float targetX = (currentLane - 1) * laneDistance;
        Vector3 currentPos = rb.position;
        float newX = Mathf.Lerp(currentPos.x, targetX, Time.fixedDeltaTime * lateralSpeed);
        
        rb.MovePosition(new Vector3(newX, rb.position.y, rb.position.z));
    }

    private void UpdateJumpAnimationState()
    {
        if (isGrounded)
        {
            isJumping = false;
            isFalling = false;
            return;
        }

        float currentYVelocity = rb.linearVelocity.y;

        if (currentYVelocity > 0 && !isJumping)
        {
            isJumping = true;
            isFalling = false;
        }
        else if (currentYVelocity < fallThreshold && isJumping && !isFalling)
        {
            isFalling = true;
            anim.SetTrigger("IsFalling");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // PRIORIDAD 1: Suelo (incluyendo techos de obstáculos etiquetados como Ground)
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (!isGrounded && (isJumping || isFalling))
            {
                isJumping = false;
                isFalling = false;
                anim.SetTrigger("IsRun");
            }
            isGrounded = true; 
            return; // IMPORTANTE: Salimos de la función para no evaluar la muerte si caímos en algo seguro
        }

        // PRIORIDAD 2: Obstáculos (Solo si no estamos tocando Ground al mismo tiempo)
        if (collision.gameObject.CompareTag("Obstaculo") && !isDead)
        {
            Die();
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Collectable item = other.GetComponent<Collectable>();
        if (item != null)
        {
            item.AttemptCollection(this); 
        }
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

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        isJumping = true;
        isFalling = false;
        anim.SetTrigger("IsJump");
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
            GameManager.Instance.GameOver();
        }
    }
}