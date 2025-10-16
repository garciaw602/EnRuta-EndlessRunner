using UnityEngine;

public class RoadModule : MonoBehaviour
{
    // Esta velocidad debe ser idéntica a la 'forwardSpeed' de tu PlayerController
    [Tooltip("Velocidad de movimiento hacia atrás. Debe coincidir con la forwardSpeed del Player.")]
    public float moveSpeed = 10f;

    // Opcional: Para calcular distancias correctamente en el generador.
    // Asigna el valor de longitud en el Inspector después de medir tu prefab.
    [HideInInspector]
    public float moduleLength = 60f;

    // NOTA: La velocidad de movimiento debe ser constante.
    void Update()
    {
        // Mueve el módulo hacia atrás (Z negativo) a velocidad constante.
        // Usamos Space.World para asegurar que siempre se mueva a lo largo del eje Z global.
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);
    }

    // Detecta el Trigger que se encarga de la destrucción de los módulos viejos.
    private void OnTriggerEnter(Collider other)
    {
        // Si el módulo choca con el Trigger de Destrucción (que estará estático detrás del Player)
        if (other.CompareTag("Destroy"))
        {
            // Destruye este GameObject (el módulo completo).
            Destroy(gameObject);
        }
    }
}