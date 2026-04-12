using UnityEngine;

// ==============================================================================
// >>> G2_ALIENCONTROLLER: Enemigo que persigue al jugador en el eje Y
// ==============================================================================
public class G2_AlienController : MonoBehaviour
{
    [Header("Configuración")]
    public float followSpeed = 1.5f;    // Velocidad con la que sigue al jugador en Y
    public float respawnTime = 3f;    // Tiempo hasta reaparecer tras morir
    public float positionX = -2f;     // Posición X fija del alien en pantalla
    public float entrySpeed = 2f;     // Velocidad de entrada desde el borde izquierdo

    private Transform playerTransform; // Referencia al jugador
    private bool isDead = false;       // ¿Está muerto el alien?
    private bool isEntering = false;   // ¿Está entrando desde el borde izquierdo?
    private float respawnTimer = 0f;   // Timer de reaparición
    private SpriteRenderer spriteRenderer; // Para activar/desactivar el sprite

    [Header("Efectos")]
    public GameObject explosionPrefab; // Prefab de la explosión
    private GameObject thrusterEffect; // El hijo con la animación del motor

    void Start()
    {
        // Buscamos al jugador por su tag 
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        // Si existe, guardamos su transform para seguirlo luego
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
                
        spriteRenderer = GetComponent<SpriteRenderer>(); // Buscamos el sprite del alien 

        if (transform.childCount > 0)
        {
            thrusterEffect = transform.GetChild(0).gameObject;
        }

        StartEntry(); // Iniciamos la entrada del alien en pantalla
    }

    void Update()
    {
        // Si no hay jugador o el juego está pausado (TimeScale a 0), no hacemos nada
        if (playerTransform == null || Time.timeScale == 0f) return;

        // ------- MUERTE ---------
        if (isDead)
        {
            // Reducimos el tiempo de respawn
            respawnTimer -= Time.deltaTime;

            // Cuando llega a 0, reaparece
            if (respawnTimer <= 0f)
            {
                StartEntry(); // Iniciamos la entrada del alien en pantalla
            }
        }

        // ------- ENTRADA ---------
        else if (isEntering)
        {
            // Se mueve desde la izquierda hasta su X fija
            float newX = Mathf.MoveTowards(transform.position.x, positionX, entrySpeed * Time.deltaTime);
            // Mientras entra, también sigue al jugador en Y
            float targetY = playerTransform.position.y;
            float newY = Mathf.MoveTowards(transform.position.y, targetY, followSpeed * Time.deltaTime);

            // Aplicamos la nueva posición
            transform.position = new Vector2(newX, newY);

            // Si ya llegó a su posición X, deja de "estar entrando" (isEntering)
            if (transform.position.x >= positionX)
            {
                isEntering = false;
            }
        }

        // ------- SEGUIMIENTO DEL JUGADOR ---------
        else
        {
            // Solo sigue al jugador en el eje Y
            float targetY = playerTransform.position.y;
            float newY = Mathf.MoveTowards(transform.position.y, targetY, followSpeed * Time.deltaTime);

            // Mantiene su X fija
            transform.position = new Vector2(positionX, newY);
        }
    }

    // =========================================================================
    // >>> ENTRADA
    // =========================================================================
    void StartEntry()
    {
        // Posicionamos el alien fuera de pantalla a la izquierda
        transform.position = new Vector2(-6f, 0f);

        // Reiniciamos estados
        isDead = false;
        isEntering = true;

        // Activamos el sprite (por si estaba muerto)
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (thrusterEffect != null) thrusterEffect.SetActive(true); // Activamos el motor visual 
    }

    // =========================================================================
    // >>> MUERTE
    // =========================================================================
    void OnDie()
    {
        if (isDead) return; // Evitamos que muera 2 veces a la vez

        isDead = true;
        respawnTimer = respawnTime;

        // COMUNICACIÓN CON EL CORE: Sumamos puntos a través del sistema central del juego
        if (MainManager.Instance != null)
        {
            MainManager.Instance.SumarPuntoTemporal(25);
        }

        // Instanciamos efecto de explosión
        if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Ocultamos el sprite
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (thrusterEffect != null) thrusterEffect.SetActive(false);
    }

    // El Alien muere si choca contra un asteroide y está vivo
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("G2_Asteroid") && !isDead) 
        {
            OnDie();
        }
    }
}
