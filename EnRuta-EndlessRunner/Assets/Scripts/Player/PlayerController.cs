using UnityEngine;

// Aseguramos que el script solo pueda existir si hay un Rigidbody (f�sicas) en el GameObject
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // --- Variables P�blicas (Ajustables desde el Inspector de Unity) ---

    [Header("Velocidad")]
    [Tooltip("Velocidad constante de avance del jugador (eje Z).")]
    public float forwardSpeed = 10f;

    [Tooltip("Velocidad de movimiento lateral al cambiar de carril (suavizado).")]
    public float laneChangeSpeed = 15f;


    [Header("Carriles")]
    [Tooltip("Distancia lateral entre cada carril.")]
    public float laneDistance = 3f;

    // --- Variables Privadas (Control de Estado Interno) ---

    // �ndice del carril actual: 0=Izquierda, 1=Centro, 2=Derecha.
    private int currentLane = 1;

    // Posici�n objetivo en el eje X (lateral) a la que se debe mover el jugador.
    private float targetXPosition;

    // Referencia al Rigidbody para mover el personaje (aunque el movimiento es cinem�tico).
    private Rigidbody rb;


    // Start se llama una vez al inicio, antes del primer frame.
    void Start()
    {
        // Obtenemos el componente Rigidbody adjunto.
        rb = GetComponent<Rigidbody>();

        // Inicializamos la posici�n X objetivo al carril central.
        targetXPosition = 0f;
    }


    // Update se llama en cada frame. �til para gestionar la entrada del usuario (Input).
    void Update()
    {
        HandleInput();
    }


    // FixedUpdate se llama a intervalos fijos. Es ideal para c�lculos de f�sicas o movimiento.
    void FixedUpdate()
    {
        MovePlayer();
    }


    // Gestiona la detecci�n de las teclas para cambiar de carril.
    private void HandleInput()
    {
        // Mover a la Izquierda (Tecla 'A' o Flecha Izquierda)
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Verificamos que no estemos ya en el carril m�s a la izquierda (0).
            if (currentLane > 0)
            {
                currentLane--; // Cambiamos al carril anterior (m�s a la izquierda).
            }
        }

        // Mover a la Derecha (Tecla 'D' o Flecha Derecha)
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Verificamos que no estemos ya en el carril m�s a la derecha (2).
            if (currentLane < 2)
            {
                currentLane++; // Cambiamos al carril siguiente (m�s a la derecha).
            }
        }

        // Calculamos la posici�n X objetivo basada en el carril actual.
        // Carril 0: -laneDistance, Carril 1: 0, Carril 2: +laneDistance
        targetXPosition = (currentLane - 1) * laneDistance;
    }


    // Aplica el movimiento al personaje.
    private void MovePlayer()
    {
        // 1. Movimiento Constante hacia Adelante (Eje Z)
        Vector3 forwardMovement = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;

        // 2. Movimiento Lateral (Transici�n Suave)

        // Creamos un nuevo vector para la posici�n horizontal (X).
        // Usamos Lerp para hacer una transici�n suave al targetXPosition.
        float newX = Mathf.Lerp(rb.position.x, targetXPosition, Time.fixedDeltaTime * laneChangeSpeed);

        // Vector final: nueva posici�n X (suave) y avance en Z (constante).
        Vector3 finalPosition = new Vector3(newX, rb.position.y, rb.position.z + forwardMovement.z);

        // Movemos el Rigidbody a la posici�n calculada.
        rb.MovePosition(finalPosition);
    }
}