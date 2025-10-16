using UnityEngine;

public class RoadModule : MonoBehaviour
{
    // Esta velocidad debe ser id�ntica a la 'forwardSpeed' de tu PlayerController
    [Tooltip("Velocidad de movimiento hacia atr�s. Debe coincidir con la forwardSpeed del Player.")]
    public float moveSpeed = 10f;

    // Opcional: Para calcular distancias correctamente en el generador.
    // Asigna el valor de longitud en el Inspector despu�s de medir tu prefab.
    [HideInInspector]
    public float moduleLength = 60f;

    // NOTA: La velocidad de movimiento debe ser constante.
    void Update()
    {
        // Mueve el m�dulo hacia atr�s (Z negativo) a velocidad constante.
        // Usamos Space.World para asegurar que siempre se mueva a lo largo del eje Z global.
        transform.Translate(Vector3.back * moveSpeed * Time.deltaTime, Space.World);
    }

    // Detecta el Trigger que se encarga de la destrucci�n de los m�dulos viejos.
    private void OnTriggerEnter(Collider other)
    {
        // Si el m�dulo choca con el Trigger de Destrucci�n (que estar� est�tico detr�s del Player)
        if (other.CompareTag("Destroy"))
        {
            // Destruye este GameObject (el m�dulo completo).
            Destroy(gameObject);
        }
    }
}