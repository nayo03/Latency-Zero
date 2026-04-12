using UnityEngine;
using UnityEngine.InputSystem; // Necesario para usar PlayerInput

public class G3_Player : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f; // Qué tan rápido se mueve la nave. Lo puedes cambiar desde el Inspector

    private PlayerInput _playerInput;     // Referencia al componente PlayerInput que acabas de añadir
    private Vector2 _inputMovimiento;     // Guarda hacia dónde está moviendo el jugador (ej: arriba-derecha = (1,1))
    private Vector2 _minBounds;           // Límite inferior-izquierdo de la pantalla en coordenadas de mundo
    private Vector2 _maxBounds;           // Límite superior-derecho de la pantalla en coordenadas de mundo

    void Start() // Se ejecuta UNA vez al arrancar la escena
    {
        // Buscamos el PlayerInput en este mismo GameObject
        _playerInput = GetComponent<PlayerInput>();
       // _playerInput.actions.FindActionMap("Player").Enable();

        // Calculamos los bordes de la pantalla para que la nave no salga
        Camera cam = Camera.main;
        float margen = 0.3f; // Un pequeño margen para que no quede pegado al borde
        _minBounds = cam.ViewportToWorldPoint(new Vector2(0, 0)); // Esquina inferior izquierda
        _maxBounds = cam.ViewportToWorldPoint(new Vector2(1, 1)); // Esquina superior derecha
        _minBounds += Vector2.one * margen; // Aplicamos el margen
        _maxBounds -= Vector2.one * margen;
    }

    void Update() // Se ejecuta CADA FRAME (60 veces por segundo aprox)
    {
        if (_playerInput == null) return; // Seguridad: si no hay PlayerInput, no hace nada

        // Le preguntamos al PlayerInput qué está haciendo el jugador
        // "Move" es el nombre de la acción que definiremos en el Action Asset
        _inputMovimiento = _playerInput.actions["Move"].ReadValue<Vector2>();

        // Convertimos ese input en movimiento 3D (Z=0 porque es 2D)
        Vector3 mov = new Vector3(_inputMovimiento.x, _inputMovimiento.y, 0);

        // Movemos la nave: posición actual + dirección * velocidad * tiempo
        // Time.deltaTime hace que el movimiento sea igual independientemente de los FPS
        transform.position += mov * velocidad * Time.deltaTime;

        // Limitamos la posición a los bordes calculados en Start()
        float x = Mathf.Clamp(transform.position.x, _minBounds.x, _maxBounds.x);
        float y = Mathf.Clamp(transform.position.y, _minBounds.y, _maxBounds.y);
        transform.position = new Vector3(x, y, 0);
        //Debug.Log("Input: " + _inputMovimiento);
    }
}