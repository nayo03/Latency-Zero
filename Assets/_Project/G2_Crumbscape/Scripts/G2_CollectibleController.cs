using UnityEngine;

// ==============================================================================
// >>> G2_COLLECTIBLECONTROLLER: Lógica de movimiento y recolección de items
// ==============================================================================
public class G2_CollectibleController : MonoBehaviour
{
    // ----------- CONFIGURACIÓN -----------
    [Header("Configuración")]
    public float speedX = 3f;
    public GameObject StarsEffectPrefab;
    private float deactivateAtX = -12f;

    // ----------- PUNTUACIÓN -----------
    [Header("Puntuación")]
    public int points = 5;
    public bool isCosmicBread = false;
    private bool puedeDarPuntos = false; // Para prevenir que de puntos cuando no debe

    [Header("Ajustes del Imán")]
    [SerializeField] private float attractionRange = 3f; // Distancia de detección
    [SerializeField] private float magnetSpeed = 6f;     // Velocidad de succión
    private Transform playerTransform;
    private G2_Player playerScript; // Para que cuando muera desaparezca

    // ==========================================================================
    // ----------- PREPARACIÓN -----------
    // ==========================================================================
    void Awake()
    {
        // Buscamos al jugador una vez al inicio (Awake es más seguro que Start aquí)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerScript = player.GetComponent<G2_Player>(); 
        }
    }

    // Al activarse desde el Spawner, habilitamos la posibilidad de dar puntos
    void OnEnable()
    {
        puedeDarPuntos = true;

    }

    // Al desactivarse (o al nacer en el Pool), bloqueamos los puntos
    void OnDisable()
    {
        puedeDarPuntos = false;
    }

    // ==========================================================================
    // ----------- IMÁN Y CICLO DE MOVIMIENTO -----------
    // ==========================================================================
    void Update()
    {
        if (Time.timeScale == 0) return; // Por si se pausa el juego

        // Cuando el jugador muera desaparecerán las estrellas atmbien
        if (playerScript != null && playerScript.isDead)
        {
            puedeDarPuntos = false;
            if (StarsEffectPrefab != null)
            {
                Instantiate(StarsEffectPrefab, transform.position, Quaternion.identity);
            }
            gameObject.SetActive(false);
            return;
        }

        // 1. MOVIMIENTO BASE: Siempre se desplaza a la izquierda
        transform.Translate(Vector2.left * speedX * Time.deltaTime, Space.World);

        // 2. LÓGICA DEL IMÁN: Si hay jugador, calculamos atracción
        if (playerTransform != null)
        {
            float distance = Vector2.Distance(transform.position, playerTransform.position);

            if (distance < attractionRange)
            {
                // MoveTowards hace que el item "vuele" hacia la nave suavemente
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    playerTransform.position,
                    magnetSpeed * Time.deltaTime
                );
            }
        }

        // 3. AUTO-DESACTIVACIÓN: Si se sale de pantalla
        if (transform.position.x <= deactivateAtX)
        {
            gameObject.SetActive(false);
        }
    }

    // ==========================================================================
    // ----------- LÓGICA DE COLISIÓN (RECOLECCIÓN) -----------
    // ==========================================================================
    void OnTriggerEnter2D(Collider2D other)
    {
        // Solo procesamos el choque si el objeto está "en juego" (puedeDarPuntos)
        if (puedeDarPuntos && other.CompareTag("Player"))
        {
            // Bloqueamos inmediatamente para que no cuente dos veces por error
            puedeDarPuntos = false;

            // ---------- SONIDO ----------
            if (AudioManager.Instance != null)
            {
                if (isCosmicBread)
                {
                    // Sonido para el Pan Cósmico
                    AudioManager.Instance.PlaySFX("bonus02");
                }
                else
                {
                    // Sonido para las Estrellas/Monedas
                    AudioManager.Instance.PlaySFX("bonus01");
                }
            }

            // ---------- GESTIÓN DE PUNTOS ----------
            // Llamada directa al Singleton del Manager
            if (G2_GameManager.Instance != null)
            {
                G2_GameManager.Instance.ItemRecogido(points);
            }

            // ---------- EFECTOS VISUALES ----------
            if (StarsEffectPrefab != null)
            {
                Instantiate(StarsEffectPrefab, transform.position, Quaternion.identity);
            }

            // El objeto vuelve al Pool
            gameObject.SetActive(false);
        }
    }

}