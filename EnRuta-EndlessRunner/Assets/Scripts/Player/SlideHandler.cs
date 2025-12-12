using UnityEngine;
using System.Collections;

public class SlideHandler : MonoBehaviour
{
    [Header("Configuración de Deslizamiento")]
    public float slideDuration = 1f;
    public float reducedColliderHeight = 0.5f;

    // Referencias internas (obtenidas desde PlayerController)
    private CapsuleCollider playerCollider;
    private Animator anim;
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;

    [HideInInspector] public bool IsSliding { get; private set; } = false;
    private Coroutine slideCoroutine;

    /// <summary>
    /// Inicializa el handler con las referencias y valores del PlayerController.
    /// </summary>
    public void Initialize(CapsuleCollider collider, Animator animator, float originalHeight, Vector3 originalCenter)
    {
        playerCollider = collider;
        anim = animator;
        originalColliderHeight = originalHeight;
        originalColliderCenter = originalCenter;
    }

    /// <summary>
    /// Inicia la corrutina de deslizamiento. Llamado por PlayerController.
    /// </summary>
    public void StartSlide()
    {
        if (IsSliding) return;
        if (slideCoroutine != null) StopCoroutine(slideCoroutine);
        slideCoroutine = StartCoroutine(SlideRoutine());
    }

    private IEnumerator SlideRoutine()
    {
        IsSliding = true;
        anim.SetBool("Slide", true);

        // Lógica para REDUCIR el Collider
        if (playerCollider != null)
        {
            playerCollider.height = reducedColliderHeight;
            playerCollider.center = new Vector3(originalColliderCenter.x, reducedColliderHeight / 2f, originalColliderCenter.z);
        }

        yield return new WaitForSeconds(slideDuration);

        // Lógica para RESTABLECER el Collider
        if (playerCollider != null)
        {
            playerCollider.height = originalColliderHeight;
            playerCollider.center = originalColliderCenter;
        }

        IsSliding = false;
        anim.SetBool("Slide", false);
        slideCoroutine = null;
    }
}
