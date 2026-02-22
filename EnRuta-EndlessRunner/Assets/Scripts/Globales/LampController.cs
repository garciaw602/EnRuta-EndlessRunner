using UnityEngine;

public class LampController : MonoBehaviour
{
    [Header("Configuración de la Lámpara")]
    public Light lampLight; // La luz física de la lámpara
    public GameObject emissiveMesh; // Opcional: El objeto que brilla (vidrio de la lámpara)

    private DayCycleManager dayCycle;
    private bool isNight = false;

    void Start()
    {
        // Busca el manager en la escena
        dayCycle = Object.FindFirstObjectByType<DayCycleManager>();

        if (dayCycle == null)
        {
            Debug.LogWarning("LampController: No se encontró DayCycleManager en la escena.");
        }

        // Estado inicial apagado
        SetLampState(false);
    }

    void Update()
    {
        if (dayCycle == null) return;

        // Comparamos el nombre de la fase actual con el nombre de la fase noche definida en el manager
        // Usamos el nombre porque DayPhase es una struct y el manager la expone internamente
        bool shouldBeOn = dayCycle.night.name == GetCurrentPhaseName();

        if (shouldBeOn != isNight)
        {
            isNight = shouldBeOn;
            SetLampState(isNight);
        }
    }

    private string GetCurrentPhaseName()
    {
        // Accedemos de forma indirecta o por reflexión si es necesario, 
        // pero lo más sencillo es comparar la intensidad o color si el manager no expone el nombre.
        // Nota: En tu script original 'night' es pública, así que podemos comparar nombres.
        return dayCycle.night.name;
    }

    private void SetLampState(bool state)
    {
        if (lampLight != null)
            lampLight.enabled = state;

        if (emissiveMesh != null)
        {
            // Opcional: Cambia el material o la visibilidad del brillo
            emissiveMesh.SetActive(state);
        }
    }
}