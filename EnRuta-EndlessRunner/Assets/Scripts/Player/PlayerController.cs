using UnityEngine;

// Aseguramos que el script solo pueda existir si hay un Rigidbody (físicas) en el GameObject
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // --- Variables Públicas (Ajustables desde el Inspector de Unity) ---

    [Header("Velocidad")]
    [Tooltip("Velocidad constante de avance del jugador (eje Z).")]
    public float forwardSpeed = 10f;

    [Tooltip("Velocidad de movimiento lateral al cambiar de carril (suavizado).")]
    public float laneChangeSpeed = 15f;


    [Header("Carriles")]
    [Tooltip("Distancia lateral entre cada carril.")]
    public float laneDistance = 3f;

    // --- Variables Privadas (Control de Estado Interno) ---

    // Índice del carril actual: 0=Izquierda, 1=Centro, 2=Derecha.
    private int currentLane = 1;

    // Posición objetivo en el eje X (lateral) a la que se debe mover el jugador.
    private float targetXPosition;

    // Referencia al Rigidbody para mover el personaje (aunque el movimiento es cinemático).
    private Rigidbody rb;


    // Start se llama una vez al inicio, antes del primer frame.
    void Start()
    {
        // Obtenemos el componente Rigidbody adjunto.
        rb = GetComponent<Rigidbody>();

        // Inicializamos la posición X objetivo al carril central.
        targetXPosition = 0f;
    }


    // Update se llama en cada frame. Útil para gestionar la entrada del usuario (Input).
    void Update()
    {
        HandleInput();
    }


    // FixedUpdate se llama a intervalos fijos. Es ideal para cálculos de físicas o movimiento.
    void FixedUpdate()
    {
        MovePlayer();
    }


    // Gestiona la detección de las teclas para cambiar de carril.
    private void HandleInput()
    {
        // Mover a la Izquierda (Tecla 'A' o Flecha Izquierda)
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            // Verificamos que no estemos ya en el carril más a la izquierda (0).
            if (currentLane > 0)
            {
                currentLane--; // Cambiamos al carril anterior (más a la izquierda).
            }
        }

        // Mover a la Derecha (Tecla 'D' o Flecha Derecha)
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            // Verificamos que no estemos ya en el carril más a la derecha (2).
            if (currentLane < 2)
            {
                currentLane++; // Cambiamos al carril siguiente (más a la derecha).
            }
        }

        // Calculamos la posición X objetivo basada en el carril actual.
        // Carril 0: -laneDistance, Carril 1: 0, Carril 2: +laneDistance
        targetXPosition = (currentLane - 1) * laneDistance;
    }


    // Aplica el movimiento al personaje.
    private void MovePlayer()
    {
        // 1. Movimiento Constante hacia Adelante (Eje Z)
        Vector3 forwardMovement = Vector3.forward * forwardSpeed * Time.fixedDeltaTime;

        // 2. Movimiento Lateral (Transición Suave)

        // Creamos un nuevo vector para la posición horizontal (X).
        // Usamos Lerp para hacer una transición suave al targetXPosition.
        float newX = Mathf.Lerp(rb.position.x, targetXPosition, Time.fixedDeltaTime * laneChangeSpeed);

        // Vector final: nueva posición X (suave) y avance en Z (constante).
        Vector3 finalPosition = new Vector3(newX, rb.position.y, rb.position.z + forwardMovement.z);

        // Movemos el Rigidbody a la posición calculada.
        rb.MovePosition(finalPosition);
    }
}