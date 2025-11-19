using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpEffectController : MonoBehaviour
{
    [Header("Componentes de Power-Up")]
    public SphereCollider magnetAttractionCollider;

    [HideInInspector] public bool isMagnetActive = false;

    // Lista de objetos que están siendo atraídos
    private List<GameObject> attractableObjects = new List<GameObject>();

    [Header("Configuración Imán")]
    public float attractionSpeed = 2500f;
    public float collectionHeightOffset = 1.0f;

    private PlayerController player;
    private Coroutine speedCoroutine;
    private Coroutine magnetCoroutine;

    void Awake()
    {
        player = GetComponent<PlayerController>();
        if (player == null) Debug.LogError("PowerUpEffectController requiere un PlayerController.");

        if (magnetAttractionCollider != null)
        {
            magnetAttractionCollider.enabled = false;
            magnetAttractionCollider.isTrigger = true;
        }
    }

    void Update()
    {
        if (!isMagnetActive) return;

        // Iteramos al revés para poder eliminar objetos de la lista si se destruyen
        for (int i = attractableObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = attractableObjects[i];

            if (obj == null)
            {
                attractableObjects.RemoveAt(i);
                continue;
            }

            // 1. Movimiento hacia el jugador
            Vector3 targetPosition = transform.position + Vector3.up * collectionHeightOffset;
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPosition, attractionSpeed * Time.deltaTime);

            // 2. Recolección automática por proximidad
            float distance = Vector3.Distance(obj.transform.position, targetPosition);
            if (distance < 0.5f)
            {
                Collectable collectable = obj.GetComponent<Collectable>();
                if (collectable != null)
                {
                    player.ProcessCollectable(collectable.data);
                }

                Destroy(obj);
                attractableObjects.RemoveAt(i);
            }
        }
    }

    // --- LÓGICA CRÍTICA DE FILTRADO ---
    void OnTriggerEnter(Collider other)
    {
       

        // 1. Chequeo básico
        if (!isMagnetActive || other.gameObject == gameObject) return;

        Collectable collectable = other.GetComponent<Collectable>();

        if (collectable != null && collectable.data != null)
        {
            // 2. FILTRO DE POWERUP: Si es un PowerUp, debemos ignorarlo.
            if (collectable.data.type == CollectableType.PowerUp)
            {
                
                return; // ¡Salimos del método, NO lo añadimos a la lista!
            }

            // 3. LÓGICA DE ATRACCIÓN: Si llegamos aquí, NO es un PowerUp (es basura/reciclaje)
            if (!attractableObjects.Contains(other.gameObject))
            {
                attractableObjects.Add(other.gameObject);

               
            }
        }
    }

    // --- MÉTODOS DE ACTIVACIÓN (Speed Boost) ---
    public void ActivateSpeedBoost(float multiplier, float duration)
    {
        if (speedCoroutine != null) StopCoroutine(speedCoroutine);
        speedCoroutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    private IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        player.currentSpeedMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        player.currentSpeedMultiplier = 1f;
        speedCoroutine = null;
    }

    // --- MÉTODOS DE ACTIVACIÓN (Magnet) ---
    public void ActivateMagnet(float radius, float duration)
    {
        if (magnetCoroutine != null) StopCoroutine(magnetCoroutine);
        magnetCoroutine = StartCoroutine(MagnetRoutine(radius, duration));
    }

    private IEnumerator MagnetRoutine(float radius, float duration)
    {
        isMagnetActive = true;
        if (magnetAttractionCollider != null)
        {
            magnetAttractionCollider.radius = radius;
            magnetAttractionCollider.enabled = true;
        }

        yield return new WaitForSeconds(duration);

        attractableObjects.Clear();
        isMagnetActive = false;
        if (magnetAttractionCollider != null) magnetAttractionCollider.enabled = false;
        magnetCoroutine = null;
    }

    public void RemoveFromMagnetList(GameObject obj)
    {
        if (attractableObjects.Contains(obj))
        {
            attractableObjects.Remove(obj);
        }
    }
}