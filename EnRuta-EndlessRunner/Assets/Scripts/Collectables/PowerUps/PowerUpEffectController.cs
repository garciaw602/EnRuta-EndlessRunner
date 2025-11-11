using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Necesario para guardar los objetos a atraer

public class PowerUpEffectController : MonoBehaviour
{
    [Header("Componentes de Power-Up")]
    // Collider del Player que define el radio de atracción del imán.
    public SphereCollider magnetAttractionCollider; 

    [HideInInspector] public bool isMagnetActive = false;
    
    // Lista de objetos de BASURA que están dentro del radio del imán.
    private List<GameObject> attractableObjects = new List<GameObject>();

    private PlayerController player;
    private Coroutine speedCoroutine;
    private Coroutine magnetCoroutine;
    
    [Header("Magnet Movement")]
    // AUMENTADO DE 1000f a 2500f para un efecto de atracción más dramático.
    public float attractionSpeed = 2500f; 

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
    
    /// <summary>
    /// Maneja el movimiento de todos los objetos en la lista 'attractableObjects'.
    /// </summary>
    void Update()
    {
        // Solo ejecutar si el imán está activo.
        if (!isMagnetActive) return;

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

            // Mover el objeto hacia la posición del jugador
            Vector3 targetPosition = transform.position;
            // Se usa MoveTowards para una velocidad constante, lo cual se siente muy potente.
            obj.transform.position = Vector3.MoveTowards(obj.transform.position, targetPosition, attractionSpeed * Time.deltaTime);
            
            // Recolección por proximidad (cuando llegan al cuerpo del jugador)
            float distance = Vector3.Distance(obj.transform.position, targetPosition);
            if (distance < 0.5f)
            {
                Collectable collectable = obj.GetComponent<Collectable>();
                if (collectable != null)
                {
                    // Recolección directa, ya que ha sido atraído por el imán
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
        // Ignora si el imán no está activo o si el objeto es el propio jugador.
        if (!isMagnetActive || other.gameObject == gameObject) return;
        
        Collectable collectable = other.GetComponent<Collectable>();
        
        // Debe tener el script Collectable y sus datos.
        if (collectable == null || collectable.data == null) return;
        
        // 1. VERIFICACIÓN CRÍTICA: Descartar PowerUps.
        if (collectable.data.type == CollectableType.PowerUp)
        {
            Debug.Log($"[MAGNET IGNORE SUCCESS] PowerUp '{other.gameObject.name}' detectado correctamente como PowerUp. IGNORADO.");
            return; 
        }
        
        // 2. Si es basura (y no está ya en la lista), lo añadimos para ser atraído por Update.
        if (!attractableObjects.Contains(other.gameObject))
        {
            attractableObjects.Add(other.gameObject);
        }
    }

    /// <summary>
    /// Método de limpieza llamado por Collectable.cs al ser recolectado por el cuerpo del jugador.
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
        yield return new WaitForSeconds(duration);
        player.currentSpeedMultiplier = 1f;
        speedCoroutine = null;
    }

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

        // Al finalizar, limpiamos la lista de objetos y desactivamos.
        attractableObjects.Clear();
        
        isMagnetActive = false;
        if (magnetAttractionCollider != null)
        {
            magnetAttractionCollider.enabled = false;
        }
        magnetCoroutine = null;
    }
}