using UnityEngine;
using System.Collections;

public class PowerUpEffectController : MonoBehaviour
{
    [Header("Componentes de Power-Up")]
    [Tooltip("El SphereCollider que se usar� para detectar �tems para el im�n. Debe ser un Trigger.")]
    public SphereCollider magnetAttractionCollider;

    // Propiedades de estado para ser le�das por Collectable.cs
    [HideInInspector] public bool isMagnetActive = false;

    // FIX: Propiedad p�blica de SOLO LECTURA para que Collectable.cs la lea.
    private float _currentAttractRadius = 0f;
    public float CurrentAttractRadius => _currentAttractRadius;

    private PlayerController player; // Referencia para modificar la velocidad.
    private Coroutine speedCoroutine;
    private Coroutine magnetCoroutine;

    void Awake()
    {
        // Obtiene la referencia al PlayerController para modificar currentSpeedMultiplier
        player = GetComponent<PlayerController>();
        if (player == null) Debug.LogError("PowerUpEffectController requiere un PlayerController en el mismo objeto.");

        // Inicializar el collider del im�n desactivado
        if (magnetAttractionCollider != null)
        {
            magnetAttractionCollider.enabled = false;
        }
    }

    // --- L�GICA DE VELOCIDAD/RELENTIZACI�N (Target del Strategy) ---
    /// <summary>
    /// Activa el efecto de velocidad, deteniendo cualquier efecto previo para reiniciar el tiempo.
    /// </summary>
    public void ActivateSpeedBoost(float multiplier, float duration)
    {
        // Detiene la corrutina si ya hay una activa para reiniciar el tiempo
        if (speedCoroutine != null) StopCoroutine(speedCoroutine);
        speedCoroutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    private IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        Debug.Log("Power-Up Velocidad Activado.");
        // Modifica la variable LE�DA por el PlayerController en FixedUpdate
        player.currentSpeedMultiplier = multiplier;

        yield return new WaitForSeconds(duration);

        player.currentSpeedMultiplier = 1f; // Resetea a la velocidad base
        speedCoroutine = null;
        Debug.Log("Power-Up Velocidad Desactivado.");
    }

    // --- L�GICA DEL IM�N (Target del Strategy) ---
    /// <summary>
    /// Activa el im�n para atraer coleccionables.
    /// </summary>
    public void ActivateMagnet(float radius, float duration)
    {
        // Detiene la corrutina si ya hay una activa para reiniciar el tiempo
        if (magnetCoroutine != null) StopCoroutine(magnetCoroutine);
        magnetCoroutine = StartCoroutine(MagnetRoutine(radius, duration));
    }

    private IEnumerator MagnetRoutine(float radius, float duration)
    {
        Debug.Log("Power-Up Im�n Activado.");
        isMagnetActive = true;
        _currentAttractRadius = radius;

        if (magnetAttractionCollider != null)
        {
            magnetAttractionCollider.radius = radius;
            magnetAttractionCollider.enabled = true; // Activa el detector de rango de atracci�n
        }

        yield return new WaitForSeconds(duration);

        isMagnetActive = false;
        _currentAttractRadius = 0f;
        if (magnetAttractionCollider != null)
        {
            magnetAttractionCollider.enabled = false; // Desactiva el detector
        }
        magnetCoroutine = null;
        Debug.Log("Power-Up Im�n Desactivado.");
    }
}