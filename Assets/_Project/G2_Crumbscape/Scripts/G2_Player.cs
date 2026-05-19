using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// ==============================================================================
// G2_PLAYER: Controlador de la nave del jugador
// ==============================================================================
public class G2_Player : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private float flapForce = 4f;       // Fuerza del impulso hacia arriba al pulsar
    [SerializeField] private GameObject explosionPrefab; // Prefab con el efecto de explosión

    // ----------- REFERENCIAS INTERNAS -----------
    private PlayerInput playerInput;       // Sistema de Input para detectar teclas/clics
    private Rigidbody2D rb;                // Motor de físicas para mover la nave
    private G2_UIManager uiManager;        // Script de la interfaz para los mensajes
    private SpriteRenderer spriteRenderer; // La imagen de la nave (para color o apagarla)
    private GameObject thrusterEffect;     // El fuego del motor (objeto hijo)

    // ----------- ESTADOS -----------
    public bool isDead = false;              // Estado de vida (público para que el Alien lo vea)
    public enum TipoMuerte { Caida, Choque } // Opciones de muerte para elegir el mensaje de UI                                             
    private float ignorarInputInicial = 0f;  // Sirve para ignorar el click/tap que viene del botón Start.

    // ==========================================================================
    // PREPARACIÓN INICIAL (Se ejecuta al nacer el objeto)
    // ==========================================================================
    void Start()
    {
        // Guardamos los componentes para usarlos rápido después
        rb = GetComponent<Rigidbody2D>();                // Componente de físicas
        playerInput = GetComponent<PlayerInput>();       // Componente de controles
        spriteRenderer = GetComponent<SpriteRenderer>(); // Componente de imagen

        uiManager = Object.FindAnyObjectByType<G2_UIManager>(); // Buscamos el manager de la interfaz

        Transform t = transform.Find("PlayerThruster"); // Buscamos el fuego del motor como hijo
        if (t != null) { thrusterEffect = t.gameObject; } // Si existe, lo guardamos

        // Si el juego NO ha iniciado (primera partida, esperando el botón Start)
        if (G2_GameManager.Instance != null && !G2_GameManager.Instance.juegoIniciado)
        {
            // Congelamos la nave arriba en el cielo para que espere al botón
            if (rb != null) rb.bodyType = RigidbodyType2D.Static;
        }
        else
        {
        
        // Si el juego YA ha iniciado (reinicios tras morir), la nave aparece directa en su sitio de juego
        IniciarPosicionJugador();
        }
    }

    // ==========================================================================
    // BUCLE DE LÓGICA (Update)
    // ==========================================================================
    void Update()
    {
        // FILTRO 1: Si el jugador ha muerto o el sistema de input falla, bloqueamos el control
        if (isDead || playerInput == null) return;

        // FILTRO 2. Si el juego está pausado, no permitimos NI el salto NI el límite del techo
        if (Time.timeScale == 0) return;

        // FILTRO 3:
        // Al empezar desde el botón Start, ignoramos el input durante un instante. Así el click del botón no se convierte en salto.
        if (Time.unscaledTime < ignorarInputInicial) return;

        // LÓGICA DE SALTO: 
        if (playerInput.actions["Interact"].WasPressedThisFrame())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * flapForce, ForceMode2D.Impulse);
        }

        // Límite del techo
        if (transform.position.y > 4.6f)
        {
            transform.position = new Vector3(transform.position.x, 4.6f, transform.position.z);
            if (rb.linearVelocity.y > 0) rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        }
    }
    // ==========================================================================
    // BLOQUEO DEL PRIMER INPUT
    // ==========================================================================
    public void IgnorarInputInicial()
    {
        // Ignoramos el input durante 0.2 segundos después de pulsar Start.
        ignorarInputInicial = Time.unscaledTime + 0.02f;
    }

    // ==========================================================================
    // POSICIÓN NAVE
    // ==========================================================================
    public void IniciarPosicionJugador()
    {
        // Teletransportamos la nave a su sitio de juego en horizontal (ej: X=-5, Y=0)
        transform.position = new Vector2(-4.51f, 0f);

        // La volvemos a hacer dinámica para que caiga y responda a los saltos
        if (rb != null) rb.bodyType = RigidbodyType2D.Dynamic;
    }

    // ==========================================================================
    // DETECCIÓN DE COLISIONES
    // ==========================================================================
    private void OnTriggerEnter2D(Collider2D otro)
    {
        // FILTRO 1: Si ya estamos muertos, no procesamos más choques
        if (isDead) return;

        // FILTRO 2: Comprobar qué objeto hemos tocado
        if (otro.CompareTag("G2_Asteroid"))
        {
            OnDie(TipoMuerte.Choque); // Muerte por chocar contra asteroide
        }
        else if (otro.CompareTag("G2_DeathZone"))
        {
            OnDie(TipoMuerte.Caida);  // Muerte por salir de los límites
        }
    }

    // ==========================================================================
    // GESTIÓN DE LA MUERTE (OnDie)
    // ==========================================================================
    void OnDie(TipoMuerte motivo)
    {
        if (isDead) return; // Cláusula de seguridad
        isDead = true;      // Marcamos que la nave ya no está operativa

        // 1. AVISO AL MANAGER: Detenemos el cronómetro de nivel para no ganar por error
        if (G2_GameManager.Instance != null)
        {
            G2_GameManager.Instance.Morir();
        }

        // 2. FÍSICAS: Quitamos las colisiones para que la nave flote o caiga sin estorbar
        rb.simulated = false;

        // 3. FEEDBACK VISUAL:
        // Sonido de muerte
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("playerexplosion");
        }

        // Solo creamos explosión si ha sido un choque (la caída es silenciosa)
        if (motivo == TipoMuerte.Choque && explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        // Apagamos el fuego del motor siempre al morir
        if (thrusterEffect != null) thrusterEffect.SetActive(false);

        // Ajustamos la apariencia según cómo hayamos muerto
        if (spriteRenderer != null)
        {
            if (motivo == TipoMuerte.Caida)
                spriteRenderer.color = Color.black; // Se vuelve negra (quemada)
            else
                spriteRenderer.enabled = false;     // Desaparece (pulverizada por la explosión)
        }

        // 4. INTERFAZ: Mandamos el mensaje correspondiente al UIManager
        if (uiManager != null)
        {
            if (motivo == TipoMuerte.Caida) uiManager.MostrarMensajeMuerteCaida();
            else uiManager.MostrarMensajeMuerteChoque();
        }

        // 5. REINICIO: Esperamos 2 segundos para que el jugador vea el desastre y recargamos
        Invoke("ReiniciarNivel", 2f);
    }

    // ==========================================================================
    // REINICIAR NIVEL
    // ==========================================================================
    void ReiniciarNivel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recargamos la escena actual para volver a intentarlo
    }
}