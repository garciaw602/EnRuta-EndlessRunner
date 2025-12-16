using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // 1. SINGLETON (Acceso global)
    public static AudioManager Instance { get; private set; }

    // 2. FUENTES DE AUDIO (Asignar en Inspector)
    [Header("Fuentes de Audio")]
    [Tooltip("Fuente para la música de fondo (BGM). Debe tener Loop activado.")]
    public AudioSource bgmSource;

    [Tooltip("Fuente para los efectos de sonido (SFX) de recolección, etc.")]
    public AudioSource sfxSource;

    // 3. CLIP GLOBAL DE MÚSICA (Los clips de recolección se quitan de aquí)
    [Header("Clips Globales")]
    public AudioClip backgroundMusic;

    void Awake()
    {
        // Implementación del Singleton
        if (Instance == null)
        {
            Instance = this;
            // Opcional: Esto hace que el AudioManager persista entre escenas
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Verificación inicial de las fuentes
        if (bgmSource == null || sfxSource == null)
        {
            Debug.LogError("FATAL: Asegúrate de asignar las fuentes de audio (bgmSource y sfxSource) en el Inspector del AudioManager.");
            this.enabled = false;
            return;
        }

        // Asegurar que las fuentes no se reproduzcan al inicio (Play on Awake = false)
        bgmSource.playOnAwake = false;
        sfxSource.playOnAwake = false;

        // Iniciar la música de fondo
        PlayBackgroundMusic();
    }

    /// <summary>
    /// Inicia la música de fondo principal del juego en loop.
    /// </summary>
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic == null)
        {
            Debug.LogWarning("No hay música de fondo asignada en el AudioManager.");
            return;
        }

        if (bgmSource.isPlaying)
        {
            return;
        }

        bgmSource.clip = backgroundMusic;
        bgmSource.loop = true; // Asegura que se repita
        bgmSource.Play();
    }

    /// <summary>
    /// Detiene la música de fondo.
    /// </summary>
    public void StopBackgroundMusic()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }

    // --- MÉTODOS PARA EFECTOS DE SONIDO (SFX) ---

    /// <summary>
    /// Reproduce un clip SFX genérico.
    /// Ahora llamado por Collectable.cs con su clip específico.
    /// </summary>
    /// <param name="clip">El clip de audio a reproducir.</param>
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            // Usamos PlayOneShot para que los sonidos no se interrumpan
            sfxSource.PlayOneShot(clip);
        }
    }

 
}