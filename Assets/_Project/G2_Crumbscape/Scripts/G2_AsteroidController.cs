using UnityEngine;

// ==============================================================================
// >>> G2_ASTEROIDCONTROLLER: Movimiento, rotación y auto-desactivación
// ==============================================================================
public class G2_AsteroidController : MonoBehaviour
{
    // ----------- CONFIGURACIÓN -----------
    [Header("Movimiento horizontal")]
    [SerializeField] private float speedX = 3f; // Velocidad de desplazamiento hacia la izquierda

    [Header("Movimiento vertical")]
    [SerializeField] private float speedY = 0f; // Frecuencia del vaivén (qué tan rápido sube/baja)
    [SerializeField] private float verticalRange = 2f; // Amplitud del vaivén (qué tan lejos llega)
    [SerializeField] private bool hasVerticalMovement = false; // Interruptor para activar el movimiento Y

    [Header("Rotación")]
    [SerializeField] private float rotationSpeed = 50f; // Velocidad de giro sobre su propio eje

    // ----------- REFERENCIAS INTERNAS -----------
    [SerializeField] private float puntoDeDespawn = -15f; // Límite en X para desactivar el objeto
    private Vector2 startPosition; // Posición de origen para el cálculo del seno
    private float localTime; // Cronómetro propio para resetear el ciclo de movimiento

    // ==========================================================================
    // PREPARACIÓN AL ACTIVARSE (Se ejecuta cada vez que sale del Pool)
    // ==========================================================================
    void OnEnable()
    {
        startPosition = transform.position; // Guardamos dónde aparece para que el vaivén sea relativo
        localTime = 0f; // Reiniciamos el reloj para que la curva de movimiento empiece de cero

        // Aleatoriedad: Invertimos el sentido del giro al azar para que no todos giren igual
        float currentSpeed = Mathf.Abs(rotationSpeed); // Obtenemos el valor positivo de la velocidad
        rotationSpeed = Random.Range(0, 2) == 0 ? currentSpeed : -currentSpeed; // Elegimos dirección horaria o antihoraria
    }

    // ==========================================================================
    // BUCLE DE LÓGICA (Update)
    // ==========================================================================
    void Update()
    {
        if (Time.timeScale == 0) return; // Por si se pausa el juego

        // 1. MOVIMIENTO HORIZONTAL
        // Desplazamos hacia la izquierda usando Space.World para ignorar rotaciones locales
        transform.Translate(Vector2.left * speedX * Time.deltaTime, Space.World);

        // 2. MOVIMIENTO VERTICAL 
        if (hasVerticalMovement) // Solo si el booleano está marcado en el Inspector
        {
            localTime += Time.deltaTime; // Aumentamos nuestro cronómetro interno

            // Lógica: Usamos Seno para crear un movimiento ondulado suave
            // NuevaY = OrigenY + Sin(Tiempo * Velocidad) * Rango
            float newY = startPosition.y + Mathf.Sin(localTime * speedY) * verticalRange;
            transform.position = new Vector2(transform.position.x, newY); // Aplicamos la altura calculada
        }

        // 3. ROTACIÓN
        // Giramos en el eje Z (profundidad) para simular la rotación espacial
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        // 4. AUTO-DESACTIVACIÓN (Gestión de Memoria)
        if (transform.position.x <= puntoDeDespawn) // Si cruza la frontera de la izquierda...
        {
            gameObject.SetActive(false); // Se apaga para que el generador de asteroides lo reutilice
        }
    }
}