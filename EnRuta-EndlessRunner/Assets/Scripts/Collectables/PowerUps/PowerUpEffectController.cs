using UnityEngine;
using System.Collections;

public class PowerUpEffectController : MonoBehaviour
{
    [Header("Componentes de Power-Up")]
    public SphereCollider magnetAttractionCollider;

    [HideInInspector] public bool isMagnetActive = false;

    // FIX: Propiedad pública para que Collectable.cs la lea.
    private float _currentAttractRadius = 0f;
    public float CurrentAttractRadius => _currentAttractRadius;

    private PlayerController player;
    private Coroutine speedCoroutine;
    private Coroutine magnetCoroutine;

    void Awake()
    {
        // Obtiene la referencia al PlayerController para modificar currentSpeedMultiplier
        player = GetComponent<PlayerController>();
        if (player == null) Debug.LogError("PowerUpEffectController requiere un PlayerController en el mismo objeto.");

        // Inicializar el collider del imán desactivado
        if (magnetAttractionCollider != null)
        {
            magnetAttractionCollider.enabled = false;
        }
    }

    // --- LÓGICA DE VELOCIDAD/RELENTIZACIÓN ---
    public void ActivateSpeedBoost(float multiplier, float duration)
    {
        if (speedCoroutine != null) StopCoroutine(speedCoroutine);
        speedCoroutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    private IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        // Modifica la variable LEÍDA por el PlayerController en FixedUpdate
        player.currentSpeedMultiplier = multiplier;

        yield return new WaitForSeconds(duration);

        player.currentSpeedMultiplier = 1f;
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
        _currentAttractRadius = radius;

        if (magnetAttractionCollider != null)
        {
            magnetAttractionCollider.radius = radius;
            magnetAttractionCollider.enabled = true;
        }

        yield return new WaitForSeconds(duration);

        isMagnetActive = false;
        _currentAttractRadius = 0f;
        if (magnetAttractionCollider != null)
        {
            magnetAttractionCollider.enabled = false;
        }
        magnetCoroutine = null;
    }
}