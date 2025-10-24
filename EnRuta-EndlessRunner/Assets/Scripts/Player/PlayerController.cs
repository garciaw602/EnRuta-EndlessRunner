using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Configuración de Movimiento Base")]
    public float baseSpeed = 10f;
    public float lateralSpeed = 5f;
    public float jumpForce = 10f;
    public float laneDistance = 4f;

    [Header("Inventario y Estadísticas")]
    public int totalGarbage = 0;
    public int plasticCount = 0;
    public int glassCount = 0;
    public int cardboardCount = 0;

    // Variables de estado y Power-Up
    [HideInInspector] public float currentSpeedMultiplier = 1f;
    [HideInInspector] public bool isMagnetActive = false;
    private Rigidbody rb;
    private Animator anim;
    private bool isGrounded = true;
    private int currentLane = 1;
    private bool isSliding = false;
    private bool isDead = false; // Estado de Muerte

    // Corrutinas de Power-Up y Deslizamiento
    private Coroutine speedCoroutine;
    private Coroutine magnetCoroutine;
    private Coroutine slideCoroutine;

    // --- CONFIGURACIÓN DEL DESLIZAMIENTO ---
    [Header("Configuración de Deslizamiento")]
    public float slideDuration = 1f;
    public CapsuleCollider playerCollider; // 🛑 Asignar en Inspector
    public float reducedColliderHeight = 0.5f; // Altura del collider al deslizarse
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;

    // --- CONFIGURACIÓN DEL IMÁN ---
    [Header("Componentes de Power-Up")]
    public SphereCollider magnetAttractionCollider; // Asignar en Inspector

    // Solución del error CS0122: Usamos una variable privada y una propiedad pública de solo lectura.
    private float _currentAttractRadius = 0f;
    public float CurrentAttractRadius => _currentAttractRadius; // Leída por Collectable.cs


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider>(); // Obtiene el Collider automáticamente

        if (playerCollider != null)
        {
            originalColliderHeight = playerCollider.height;
            originalColliderCenter = playerCollider.center;
        }

        if (rb == null) Debug.LogError("PlayerController requiere un componente Rigidbody.");
        if (anim == null) Debug.LogError("PlayerController requiere un componente Animator.");

        // Inicializa la animación de correr
        anim.SetBool("IsRunning", true);
    }

    void Update()
    {
        if (isDead) return;

        // 1. Lógica de Salto
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            && isGrounded && !isSliding)
        {
            Jump();
        }

        // 2. Lógica de Deslizamiento
        if ((Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            && isGrounded && !isSliding)
        {
            StartSlide();
        }

        // 3. Lógica de Movimiento Lateral
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
        if (isDead)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        float finalSpeed = baseSpeed * currentSpeedMultiplier;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, finalSpeed);

        float targetX = (currentLane - 1) * laneDistance;
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);

        Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * lateralSpeed);
        rb.MovePosition(new Vector3(newPosition.x, rb.position.y, rb.position.z));
    }

    void OnCollisionEnter(Collision collision)
    {
        // Detección de Colisión con el suelo
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            //Resetea la animación de salto al tocar el suelo
            anim.SetBool("IsJumping", false);
        }

        // Detección de Colisión con Obstáculo
        if (collision.gameObject.CompareTag("Obstaculo") && !isDead)
        {
            Die();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Colisión con Collectibles
        Collectable item = other.GetComponent<Collectable>();

        if (item != null)
        {
            ProcessCollectable(item.data);
            return;
        }

        // Detección de Obstáculos de tipo Trigger
        if (other.CompareTag("Obstaculo") && !isDead)
        {
            Die();
        }
    }

    public void ProcessCollectable(CollectableData data)
    {
        switch (data.type)
        {
            case CollectableType.GeneralGarbage:
                totalGarbage += data.baseValue;
                break;
            case CollectableType.Recyclable:
                ProcessRecyclable(data.collectableName, data.baseValue);
                break;
            case CollectableType.PowerUp:
                if (data.powerUpEffect != null)
                {
                    // Nota: Asumo que la clase PowerUpEffectData tiene una propiedad 'duration'.
                    data.powerUpEffect.ApplyEffect(this, data.powerUpEffect.duration);
                }
                break;
        }
    }

    private void ProcessRecyclable(string name, int value)
    {
        if (name.Contains("Plástico")) plasticCount += value;
        else if (name.Contains("Vidrio")) glassCount += value;
        else if (name.Contains("Cartón")) cardboardCount += value;
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
        // Activa la animación de salto (Bool)
        anim.SetBool("IsJumping", true);
    }

    private void StartSlide()
    {
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideRoutine());
    }

    private IEnumerator SlideRoutine()
    {
        isSliding = true;
        anim.SetBool("Slide", true);

        // 🛑 Lógica para REDUCIR el Collider (para pasar bajo obstáculos)
        if (playerCollider != null)
        {
            // Guarda el centro original para poder restaurar correctamente
            playerCollider.height = reducedColliderHeight;
            playerCollider.center = new Vector3(originalColliderCenter.x, reducedColliderHeight / 2f, originalColliderCenter.z);
        }

        yield return new WaitForSeconds(slideDuration);

        // 🛑 Lógica para RESTABLECER el Collider
        if (playerCollider != null)
        {
            playerCollider.height = originalColliderHeight;
            playerCollider.center = originalColliderCenter;
        }

        isSliding = false;
        anim.SetBool("Slide", false);
        slideCoroutine = null;
    }

    private void MoveLane(int direction)
    {
        int newLane = currentLane + direction;
        if (newLane >= 0 && newLane <= 2)
        {
            currentLane = newLane;
        }
    }

    public void ActivateSpeedBoost(float multiplier, float duration)
    {
        if (speedCoroutine != null) StopCoroutine(speedCoroutine);
        speedCoroutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }
    private IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        currentSpeedMultiplier = multiplier;
      

        yield return new WaitForSeconds(duration);
        currentSpeedMultiplier = 1f;        
        speedCoroutine = null;
    }

    // --- LÓGICA DEL IMÁN ---
    public void ActivateMagnet(float radius, float duration)
    {
        if (magnetCoroutine != null) StopCoroutine(magnetCoroutine);
        magnetCoroutine = StartCoroutine(MagnetRoutine(radius, duration));
    }

    private IEnumerator MagnetRoutine(float radius, float duration)
    {
        isMagnetActive = true;
        _currentAttractRadius = radius; // 🛑 Asignando a la variable privada

        if (magnetAttractionCollider != null)
        {
            magnetAttractionCollider.radius = radius;
            magnetAttractionCollider.enabled = true;
        }

        yield return new WaitForSeconds(duration);

        isMagnetActive = false;
        _currentAttractRadius = 0f; // 🛑 Reseteando la variable privada
        if (magnetAttractionCollider != null)
        {
            magnetAttractionCollider.enabled = false;
        }
        magnetCoroutine = null;
    }

    // --- Lógica de Muerte ---
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        anim.SetTrigger("Die"); // Activa la animación de morir (Trigger)

        // Aquí se puede detener el tiempo, mostrar el menú de Game Over, etc.
        Debug.Log("¡GAME OVER!");
    }
}