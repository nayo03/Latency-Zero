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
    private float deactivateAtX = -15f;

    // ----------- PUNTUACIÓN -----------
    [Header("Puntuación")]
    public int points = 5;
    public bool isCosmicBread = false;

    private bool puedeDarPuntos = false; // <--- ESCUDO DE SEGURIDAD

    // ==========================================================================
    // ----------- ESTADOS DEL POOL -----------
    // ==========================================================================

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
    // ----------- CICLO DE MOVIMIENTO -----------
    // ==========================================================================
    void Update()
    {
        transform.Translate(Vector2.left * speedX * Time.deltaTime, Space.World);

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