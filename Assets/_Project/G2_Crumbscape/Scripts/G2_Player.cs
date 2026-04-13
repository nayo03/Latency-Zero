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

        Time.timeScale = 1f; // Nos aseguramos de que el tiempo corra (por si venimos de un menú pausado)
    }

    // ==========================================================================
    // BUCLE DE LÓGICA (Update)
    // ==========================================================================
    void Update()
    {
        // FILTRO 1: Si el jugador ha muerto o el sistema de input falla, bloqueamos el control
        if (isDead || playerInput == null) return;

        // FILTRO 2: Comprobamos si se ha pulsado la acción "Interact" en este frame
        if (playerInput.actions["Interact"].WasPressedThisFrame())
        {
            // Aplicamos el impulso:
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Reset de velocidad vertical
            rb.AddForce(Vector2.up * flapForce, ForceMode2D.Impulse); // Impulso instantáneo hacia arriba
        }
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