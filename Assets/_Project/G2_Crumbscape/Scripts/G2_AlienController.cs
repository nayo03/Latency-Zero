using UnityEngine;

// ==============================================================================
// G2_ALIENCONTROLLER: Alien que persigue a la nave y da puntos
// ==============================================================================
public class G2_AlienController : MonoBehaviour
{
    // ----------- CONFIGURACIÓN -----------
    [Header("Ajustes de Movimiento")]
    [SerializeField] private float followSpeed = 1.5f; // Lo rápido que sube/baja para seguir al jugador
    [SerializeField] private float respawnTime = 2f;   // Segundos de espera tras explotar
    [SerializeField] private float positionX = -2f;    // El punto exacto de la pantalla donde se queda
    [SerializeField] private float entrySpeed = 2f;    // La prisa que tiene por entrar a pantalla
    [SerializeField] private int puntosAlien = 25;     // Los puntos que da al jugador al morir

    [Header("Efectos Visuales")]
    [SerializeField] private GameObject explosionPrefab; // El objeto de la explosión (partículas)

    // ----------- REFERENCIAS INTERNAS -----------
    private GameObject thrusterEffect;  // El fuego que sale del motor del alien
    private Transform playerTransform;  // Para saber en qué coordenadas está el jugador
    private SpriteRenderer spriteRenderer; // Para ocultar al alien cuando explota
    private Collider2D alienCollider;   // El "cuerpo" que detecta choques
    private G2_Player playerScript;     // Para leer si el jugador está vivo o muerto
    private G2_GameManager gameManager; // Para poder enviarle los puntos ganados

    // ----------- ESTADOS -----------
    private bool isDead = false;        // Si es verdadero, el alien está destruido
    private bool isEntering = false;    // Si es verdadero, está viajando hacia su sitio
    private float respawnTimer = 3f;    // El contador que dice cuánto falta para renacer

    // ==========================================================================
    // PREPARACIÓN INICIAL (Se ejecuta al nacer el objeto)
    // ==========================================================================
    void Awake()
    {
        // Guardamos su componente de choque para poder encenderlo y apagarlo
        alienCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        // 1. Buscamos al jugador por su etiqueta "Player" para tenerlo localizado
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            // Guardamos su posición y su script para consultarlos luego
            playerTransform = playerObj.transform;
            playerScript = playerObj.GetComponent<G2_Player>();
        }

        // 2. Buscamos el objeto del motor (fuego) que es un hijo de este objeto
        Transform t = transform.Find("AlienThruster");
        if (t != null) { thrusterEffect = t.gameObject; }

        // 3. Buscamos el gestor de puntos y el dibujante de imagen (SpriteRenderer)
        gameManager = Object.FindAnyObjectByType<G2_GameManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 4. Mandamos al alien fuera de pantalla para que empiece su entrada
        StartEntry();
    }

    // ==========================================================================
    // BUCLE DE JUEGO (Se ejecuta 60 veces por segundo)
    // ==========================================================================
    void Update()
    {
        // FILTRO 1: Si no hay jugador o el tiempo está parado (pausa), no hacemos nada
        if (playerTransform == null || Time.timeScale == 0f) return;

        // FILTRO 2: Si el jugador ha muerto, el alien deja de moverse por respeto
        if (playerScript != null && playerScript.isDead) return;

        // FILTRO 3: Si el alien está destruido, restamos tiempo al reloj de renacimiento
        if (isDead)
        {
            respawnTimer -= Time.deltaTime; // Restamos tiempo real (segundos)
            if (respawnTimer <= 0f) StartEntry(); // Si llega a cero, el alien renace
            return; // Salimos para no ejecutar el movimiento si está muerto
        }

        // Si ha pasado todos los filtros, movemos al alien
        ManejarMovimiento();
    }

    // ==========================================================================
    // LÓGICA DE MOVIMIENTO (Fase de entrada y Fase de acecho)
    // ==========================================================================
    private void ManejarMovimiento()
    {
        if (isEntering)
        {
            // FASE ENTRADA: Movimiento desde el punto de origen hasta la X de destino.
            // Lógica: MoveTowards(Origen, Destino, Velocidad * Tiempo) para regular el movimiento por segundos, no por frames.
            float newX = Mathf.MoveTowards(transform.position.x, positionX, entrySpeed * Time.deltaTime); // Calculamos nueva X
            float targetY = playerTransform.position.y; // Localizamos la altura actual del jugador
            float newY = Mathf.MoveTowards(transform.position.y, targetY, followSpeed * Time.deltaTime); // Calculamos nueva Y

            transform.position = new Vector2(newX, newY); // Aplicamos las coordenadas calculadas

            // Cuando el alien llega a su coordenada X, termina la entrada
            if (transform.position.x >= positionX)
            {
                isEntering = false;

                if (alienCollider != null) alienCollider.enabled = true; // Encendemos el collider para que pueda chocar
            }
        }
        else // Si ya ha terminado de entrar...
        {
            // FASE ACECHO: Se queda clavado en la X, pero imita la altura (Y) del jugador
            float targetY = playerTransform.position.y; // Obtenemos de nuevo la altura del jugador
            float newY = Mathf.MoveTowards(transform.position.y, targetY, followSpeed * Time.deltaTime); // Calculamos nueva Y

            transform.position = new Vector2(positionX, newY); // Aplicamos la altura nueva manteniendo la X siempre en el mismo punto (positionX)
        }
    }

    // ==========================================================================
    // NACIMIENTO Y RESURRECCIÓN
    // ==========================================================================
    void StartEntry()
    {
        // 1. Teletransportamos al alien al borde izquierdo (-4)
        transform.position = new Vector2(-4f, 0f);

        // 2. Apagamos sus colisiones para que no muera al nacer
        if (alienCollider != null) alienCollider.enabled = false;

        // 3. Reseteamos sus estados y encendemos su imagen y su motor
        isDead = false;
        isEntering = true;
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (thrusterEffect != null) thrusterEffect.SetActive(true);
    }

    // ==========================================================================
    // DETECCIÓN DE CHOQUES (Cuando algo entra en su zona)
    // ==========================================================================
    void OnTriggerEnter2D(Collider2D other)
    {
        // Solo reaccionamos si lo que nos toca tiene la etiqueta "G2_Asteroid"
        if (other.CompareTag("G2_Asteroid") && !isDead)
        {
            OnDie(); // Llamamos a la función de muerte
        }
    }

    // ==========================================================================
    // PROCESO DE MUERTE (Explosión y Puntos)
    // ==========================================================================
    void OnDie()
    {
        if (isDead) return; // Seguridad para no morir dos veces
        isDead = true; // Marcamos como muerto
        respawnTimer = respawnTime; // Iniciamos cuenta atrás para renacer
        
        if (alienCollider != null) alienCollider.enabled = false; // Apagamos su colisionador 

        // ---------- GESTIÓN DE PUNTOS -----------
        // Esto evita que el jugador gane puntos después de haber perdido
        if (gameManager != null && Time.timeScale > 0) // Si hay gestor y el juego corre (no está pausado)...
            if (playerScript != null && playerScript.isDead == false) // Y solo si el player está vivo...
                gameManager.ItemRecogido(puntosAlien); // SUMAR PUNTOS

        // FEEDBACK VISUAL: Creamos la explosión y escondemos al alien
        if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (thrusterEffect != null) thrusterEffect.SetActive(false);
    }
}