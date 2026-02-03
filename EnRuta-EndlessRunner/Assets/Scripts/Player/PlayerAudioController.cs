using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    // 1. COMPONENTE AUDIO SOURCE
    [Tooltip("El componente AudioSource para reproducir los sonidos.")]
    public AudioSource audioSource;

    // 2. CLIPS DE SONIDO DEL JUGADOR
    [Header("Clips de Acción")]
    public AudioClip jumpSound;
    public AudioClip slideSound;
    public AudioClip runLoopSound; // El sonido de correr que se repite
    public AudioClip crashSound;
    public AudioClip dieSound;

    private bool isRunningLooping = false;


    void Awake()
    {
        // Verificar y asignar el AudioSource si no se hizo en el Inspector
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Si aún es nulo, lo añadimos dinámicamente
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.LogWarning("AudioSource añadido dinámicamente al Player.");
        }

        // Recomendación: Asegurar que el AudioSource no se reproduzca al inicio
        audioSource.playOnAwake = false;
    }

    // --- MÉTODOS PÚBLICOS PARA REPRODUCIR SONIDOS CORTOS ---

    /// <summary>
    /// Reproduce el sonido de salto usando PlayOneShot.
    /// </summary>
    public void PlayJump()
    {
        if (jumpSound != null)
        {
            audioSource.PlayOneShot(jumpSound);
        }
    }

    /// <summary>
    /// Reproduce el sonido de deslizarse usando PlayOneShot.
    /// </summary>
    public void PlaySlide()
    {
        if (slideSound != null)
        {
            audioSource.PlayOneShot(slideSound);
        }
    }

    /// <summary>
    /// Reproduce el sonido de choque usando PlayOneShot.
    /// </summary>
    public void PlayCrash()
    {
        if (crashSound != null)
        {
            audioSource.PlayOneShot(crashSound);
        }
    }

    /// <summary>
    /// Reproduce el sonido de muerte (Debería ser llamado antes de la destrucción del objeto).
    /// </summary>
    public void PlayDie()
    {
        if (dieSound != null)
        {
            // Usamos PlayOneShot para asegurar que se reproduzca todo el clip
            audioSource.PlayOneShot(dieSound);
        }
    }

    // --- MÉTODOS PARA EL SONIDO DE CORRER (LOOP) ---

    /// <summary>
    /// Inicia el sonido de correr en loop.
    /// </summary>
    public void StartRunLoop()
    {
        if (runLoopSound == null || isRunningLooping) return;

        audioSource.clip = runLoopSound;
        audioSource.loop = true;
        audioSource.Play();
        isRunningLooping = true;
    }

    /// <summary>
    /// Detiene el sonido de correr en loop.
    /// </summary>
    public void StopRunLoop()
    {
        if (!isRunningLooping) return;

        // Sólo detenemos si el clip actual es el de correr.
        if (audioSource.clip == runLoopSound)
        {
            audioSource.Stop();
        }
        isRunningLooping = false;
    }
}