using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpEffectController : MonoBehaviour
{
    [Header("Componentes de Power-Up")]
    // Collider del Player que define el radio de atracción del imán.
    public SphereCollider magnetAttractionCollider;

    [HideInInspector] public bool isMagnetActive = false;

    // Lista de objetos de BASURA que están dentro del radio del imán.
    public List<GameObject> attractableObjects = new List<GameObject>();

    private PlayerController player;
    private Coroutine speedCoroutine;
    private Coroutine magnetCoroutine;

    // Variable necesaria para almacenar la duración y el nombre del PowerUp de Velocidad
    private float currentSpeedDuration = 0f;

    [Header("Magnet Movement")]
    public float attractionSpeed = 2500f;
    public float collectionHeightOffset = 1.0f;

    void Awake()
    {
        player = GetComponent<PlayerController>();
        if (player == null) Debug.LogError("PowerUpEffectController requiere un PlayerController en el mismo objeto.");

        if (magnetAttractionCollider != null)
        {
            magnetAttractionCollider.enabled = false;
            magnetAttractionCollider.isTrigger = true;
        }
    }

    void Update()
    {
        // 1. Lógica de Atracción del Imán
        if (isMagnetActive)
        {
            HandleMagnetAttraction();
        }

        // 2. Actualización de UI para PowerUp de Velocidad (Si es necesario mostrar el tiempo)
        // Nota: El tiempo del imán se gestiona dentro de su Corrutina, no aquí.
        if (currentSpeedDuration > 0)
        {
            // Opcional: Si quieres mostrar el Boost, llama a UIManager aquí también.
            // Ejemplo: UIManager.Instance.ShowPowerUpStatus("VELOCIDAD", currentSpeedDuration);
            // currentSpeedDuration -= Time.deltaTime;
        }
    }

    private void HandleMagnetAttraction()
    {
        // Iteración inversa para poder eliminar objetos de la lista mientras iteramos.
        for (int i = attractableObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = attractableObjects[i];

            // Si el objeto fue destruido, lo removemos de la lista.
            if (obj == null)
            {
                attractableObjects.RemoveAt(i);
                continue;
            }

            // Mover el objeto hacia la posición del jugador (incluyendo X)
            Vector3 targetPosition = transform.position + Vector3.up * collectionHeightOffset;

            // Mover hacia el jugador
            obj.transform.position = Vector3.MoveTowards(
                obj.transform.position,
                targetPosition,
                attractionSpeed * Time.deltaTime
            );

            // Recolección por proximidad (cuando llegan al cuerpo del jugador)
            float distance = Vector3.Distance(obj.transform.position, targetPosition);
            if (distance < 0.5f)
            {
                Collectable collectable = obj.GetComponent<Collectable>();
                if (collectable != null)
                {
                    // Reproduce el sonido de recolección
                    if (AudioManager.Instance != null && collectable.collectionSound != null)
                    {
                        AudioManager.Instance.PlaySFX(collectable.collectionSound);
                    }

                    player.ProcessCollectable(collectable.data);
                }
                Destroy(obj);
                attractableObjects.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// Detecta objetos que entran al radio del imán (magnetAttractionCollider).
    /// </summary>
    void OnTriggerEnter(Collider other)
    {
        // El OnTriggerEnter es llamado por el magnetAttractionCollider.
        if (!isMagnetActive || other.gameObject == gameObject) return;

        // PRIMERO: Solo procesar objetos que tengan el componente Collectable
        // Esto garantiza que SOLO la basura/reciclaje sea atraída
        Collectable collectable = other.GetComponent<Collectable>();
        if (collectable == null || collectable.data == null) return;

        // SEGUNDO: Rechazar explícitamente PowerUps y obstáculos
        if (collectable.data.type == CollectableType.PowerUp)
        {
            return; // PowerUps no deben ser atraídos
        }

        // TERCERO: Seguridad adicional - rechazar si tiene tag de obstáculo
        if (other.CompareTag("Obstaculo") || other.CompareTag("Ground"))
        {
            return; // No atraer obstáculos ni el suelo
        }

        // Si es basura válida (GeneralGarbage o Recyclable), lo añadimos para ser atraído
        if (!attractableObjects.Contains(other.gameObject))
        {
            attractableObjects.Add(other.gameObject);
        }
    }

    /// <summary>
    /// MÃ©todo de limpieza llamado por Collectable.cs al ser recolectado por el cuerpo del jugador.
    /// </summary>
    public void RemoveAttractableObject(GameObject obj)
    {
        if (attractableObjects.Contains(obj))
        {
            attractableObjects.Remove(obj);
        }
    }


    // --- LÓGICA DE EFECTOS ---

    public void ActivateSpeedBoost(float multiplier, float duration)
    {
        if (speedCoroutine != null) StopCoroutine(speedCoroutine);
        speedCoroutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    private IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        player.currentSpeedMultiplier = multiplier;
        currentSpeedDuration = duration; // Para seguimiento opcional en Update

        // Opcional: Llama a UIManager para mostrar que el boost está activo
        // UIManager.Instance.ShowPowerUpStatus("VELOCIDAD", duration); 

        yield return new WaitForSeconds(duration);

        player.currentSpeedMultiplier = 1f;
        currentSpeedDuration = 0f;
        speedCoroutine = null;

        // Opcional: Llama a UIManager para limpiar el boost si se mostró
        // UIManager.Instance.ClearPowerUpStatus();
    }

    public void ActivateMagnet(float radius, float duration)
    {
        if (magnetCoroutine != null) StopCoroutine(magnetCoroutine);
        magnetCoroutine = StartCoroutine(MagnetRoutine(radius, duration));
    }

    private IEnumerator MagnetRoutine(float radius, float duration)
    {
        // Lógica de Activación
        isMagnetActive = true;
        if (magnetAttractionCollider != null)
        {
            magnetAttractionCollider.radius = radius;
            magnetAttractionCollider.enabled = true;
        }

        // Control de Tiempo y Actualización de UI (¡LA CORRECCIÓN!)
        float timer = duration;
        string powerUpName = "IMÁN";

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            // ➡️ Llama al UIManager para actualizar el tiempo restante CADA FRAME
            if (UIManager.Instance != null)
            {
                // Solo llamamos a ShowPowerUpStatus, UIManager maneja el formato ":0.0s"
                UIManager.Instance.ShowPowerUpStatus(powerUpName, timer);
            }

            yield return null; // Espera al siguiente frame
        }

        // Lógica de Desactivación (Cuando timer <= 0)

        // Limpia el estado en el UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ClearPowerUpStatus();
        }

        // Desactiva el imán primero (detiene la atracción)
        isMagnetActive = false;

        if (magnetAttractionCollider != null)
        {
            magnetAttractionCollider.enabled = false;
        }

        // Ahora continúa con el desplazamiento suave de los objetos que aún se están moviendo
        // hacia los carriles correctos mientras se recolectan
        float smoothDuration = 0.5f; // Tiempo para que terminen de moverse
        float elapsed = 0f;

        while (elapsed < smoothDuration && attractableObjects.Count > 0)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / smoothDuration;

            // Continúa atrayendo objetos pero suavemente
            for (int i = attractableObjects.Count - 1; i >= 0; i--)
            {
                GameObject obj = attractableObjects[i];

                if (obj == null)
                {
                    attractableObjects.RemoveAt(i);
                    continue;
                }

                Vector3 targetPosition = transform.position + Vector3.up * collectionHeightOffset;

                // Movimiento más suave durante el fade-out del imán
                obj.transform.position = Vector3.Lerp(
                    obj.transform.position,
                    targetPosition,
                    Time.deltaTime * (attractionSpeed * 0.3f)
                );

                float distance = Vector3.Distance(obj.transform.position, targetPosition);
                if (distance < 0.5f)
                {
                    Collectable collectable = obj.GetComponent<Collectable>();
                    if (collectable != null)
                    {
                        // Reproduce el sonido de recolección
                        if (AudioManager.Instance != null && collectable.collectionSound != null)
                        {
                            AudioManager.Instance.PlaySFX(collectable.collectionSound);
                        }

                        player.ProcessCollectable(collectable.data);
                    }
                    Destroy(obj);
                    attractableObjects.RemoveAt(i);
                }
            }

            yield return null;
        }

        attractableObjects.Clear();
        magnetCoroutine = null;
    }
}