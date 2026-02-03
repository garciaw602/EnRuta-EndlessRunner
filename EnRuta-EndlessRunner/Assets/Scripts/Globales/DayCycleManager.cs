using UnityEngine;
using System.Collections;

public class DayCycleManager : MonoBehaviour
{
    // Duración de cada fase (Mañana, Tarde, Noche)
    // Se convierten en variables públicas para poder ajustarlas en el Inspector.

    [Header("Control de Tiempos")]
    [Tooltip("Duración de la transición gradual entre fases (ej: Noche -> Mañana).")]
    public float transitionDuration = 30f;

    [Tooltip("Tiempo que la fase permanece estable (ej: El punto más brillante del Mediodía).")]
    public float stableDuration = 30f;

    // Referencia a la Luz Direccional que simula el sol
    [Tooltip("Asigna la luz direccional de tu escena aquí (El Sol).")]
    public Light directionalLight;

    // Estructura para definir los ajustes de cada fase
    [System.Serializable]
    public struct DayPhase
    {
        public string name;
        public Color lightColor;
        // Rango ampliado si es necesario, pero 2 es un buen máximo estándar.
        [Range(0, 3)]
        public float lightIntensity;
    }

    [Header("Configuración de Fases")]
    public DayPhase morning;
    public DayPhase afternoon;
    public DayPhase night;

    // Variables de control de la rutina
    private DayPhase currentState;
    private DayPhase nextState;
    private Coroutine cycleRoutine;

    void Awake()
    {
        // Verificar que la luz esté asignada antes de empezar
        if (directionalLight == null)
        {
            Debug.LogError("ERROR: Asigna la luz direccional (el sol) al script DayCycleManager en el Inspector.");
            this.enabled = false;
            return;
        }

        // Asegura que las duraciones mínimas sean válidas
        if (transitionDuration <= 0) transitionDuration = 1f;
        if (stableDuration <= 0) stableDuration = 1f;


        // Configuración inicial del juego (siempre empezar en la Mañana)
        directionalLight.color = morning.lightColor;
        directionalLight.intensity = morning.lightIntensity;
        currentState = morning;

        // Inicia el ciclo
        cycleRoutine = StartCoroutine(DayCycleRoutine());
    }

    /// <summary>
    /// Corrutina principal que gestiona el ciclo y las transiciones.
    /// </summary>
    IEnumerator DayCycleRoutine()
    {
        while (true) // Bucle infinito
        {
            // 1. TRANSICIÓN MAÑANA -> TARDE
            nextState = afternoon;
            yield return StartCoroutine(TransitionPhase(currentState, nextState, transitionDuration));
            currentState = afternoon;
            Debug.Log($"Cambio de Fase: Tarde (Estable por: {stableDuration}s)");

            // 2. ESPERA en la Tarde (máximo brillo)
            yield return new WaitForSeconds(stableDuration);

            // 3. TRANSICIÓN TARDE -> NOCHE
            nextState = night;
            yield return StartCoroutine(TransitionPhase(currentState, nextState, transitionDuration));
            currentState = night;
            Debug.Log($"Cambio de Fase: Noche (Estable por: {stableDuration}s)");

            // 4. ESPERA en la Noche
            yield return new WaitForSeconds(stableDuration);

            // 5. TRANSICIÓN NOCHE -> MAÑANA
            nextState = morning;
            yield return StartCoroutine(TransitionPhase(currentState, nextState, transitionDuration));
            currentState = morning;
            Debug.Log($"Cambio de Fase: Mañana (Estable por: {stableDuration}s)");

            // 6. ESPERA en la Mañana
            yield return new WaitForSeconds(stableDuration);
        }
    }

    /// <summary>
    /// Realiza la interpolación gradual (Lerp) entre dos fases.
    /// </summary>
    IEnumerator TransitionPhase(DayPhase start, DayPhase end, float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            float t = timer / duration;

            // Aplica la interpolación a la Luz Direccional
            directionalLight.color = Color.Lerp(start.lightColor, end.lightColor, t);
            directionalLight.intensity = Mathf.Lerp(start.lightIntensity, end.lightIntensity, t);

            timer += Time.deltaTime;
            yield return null;
        }

        // Asegura que los valores finales sean exactos
        directionalLight.color = end.lightColor;
        directionalLight.intensity = end.lightIntensity;
    }
}