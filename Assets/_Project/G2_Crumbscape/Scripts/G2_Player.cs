using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// ==============================================================================
// >>> G2_PLAYER: Controlador de movimiento y detección (Minijuego 2)
// Este script gestiona el movimiento mediante el nuevo Input System y detecta
// cuando el jugador colisiona con los items para avisar al GameManager local.
// También gestiona la muerte del jugador
// ==============================================================================

public class G2_Player : MonoBehaviour
{
    [Header("Configuración")]
    public float flapForce = 4f;    
    public GameObject explosionPrefab;

    private G2_GameManager gameManager;
    private PlayerInput playerInput;
    private Rigidbody2D rb;

    // Variables para muerte 
    private float minHeight; 
    private bool isDead = false;
    private G2_UIManager uiManager;
    public enum TipoMuerte { Caida, Choque }
    private SpriteRenderer spriteRenderer;
    private GameObject thrusterEffect;

    void Start()
    {
        // -------- ASIGNACIÓN DE COMPONENTES --------
        // Obtiene el componente PlayerInput que está en el mismo GameObject
        playerInput = GetComponent<PlayerInput>();

        // Buscamos el G2_GameManager.
        gameManager = Object.FindAnyObjectByType<G2_GameManager>();

        // Obtiene el Rigidbody2D del objeto
        rb = GetComponent<Rigidbody2D>();

        // Buscamos el UI Manager en la escena para poder imprimir textos personalizados según muerte
        uiManager = Object.FindAnyObjectByType<G2_UIManager>();

        // Buscamos el spriteRenderer de la nave para cambiarle el color si muere por caída
        spriteRenderer = GetComponent<SpriteRenderer>();

        Transform t = transform.Find("ThrusterEffect");
        if (t != null) thrusterEffect = t.gameObject;

    }

    void Update()
    {
        // -------- SALTO ---------
        // Evita errores si el Input no está listo.
        if (isDead || playerInput == null) return;

        // Comprueba si la acción "Interact" se ha pulsado en ESTE frame
        // WasPressedThisFrame = solo detecta el momento exacto del click
        if (playerInput.actions["Interact"].WasPressedThisFrame())
        {
            // 1. Resetear la velocidad vertical (importante)
            // Pone la velocidad en Y a 0 antes de saltar. Esto evita que la gravedad acumulada anule el salto
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);

            // 2. Aplicar el impulso hacia arriba (flapForce define la potencia del salto) (Impulse es un tipo de salto predefinido)
            rb.AddForce(Vector2.up * flapForce, ForceMode2D.Impulse);
        }
    }

    // =========================================================================
    // >>> DETECCIÓN DE TRIGGERS
    // =========================================================================
    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (isDead) return;
        // Buscamos objetos con el tad G2_Asteroid
        if (otro.CompareTag("G2_Asteroid"))
        {
            OnDie(TipoMuerte.Choque);
        }

        // Buscamos objetos con los Tags "G2_Star" y "G2_Bread".
        else if (otro.CompareTag("G2_Star") || otro.CompareTag("G2_Bread"))
        {
            otro.gameObject.SetActive(false); // Desactivamos el objeto para que vuelva al PoolManager

            // COMUNICACIÓN CON EL MANAGER LOCAL:
            if (gameManager != null)
            {
                gameManager.ItemRecogido();
            }            
        }

        else if (otro.CompareTag("G2_DeathZone"))
        {
            OnDie(TipoMuerte.Caida);
        }
    }

    // =========================================================================
    // >>> MUERTE
    // =========================================================================
    void OnDie(TipoMuerte motivo)
    {
        if (isDead) return;
        isDead = true;

        if (explosionPrefab != null) Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Comunicamos al UI el TipoMuerte e imprime según tipo
        if (uiManager != null)
        {
            if (motivo == TipoMuerte.Caida)
                uiManager.MostrarMensajeMuerteCaida();
            else
                uiManager.MostrarMensajeMuerteChoque();
        }

        // -------- EFECTOS VISUALES ---------
        // 1. Pausamos la nave SIEMPRE (se queda congelada) y apagamos la animación del motor
        rb.simulated = false;

        if (thrusterEffect != null)
        {
            thrusterEffect.SetActive(false);
        }

        // 2. Cambiamos el color o visibilidad según cómo muera
        if (spriteRenderer != null)
        {
            if (motivo == TipoMuerte.Caida)
            {
                // Si toca la zona de abajo, se queda congelada y se vuelve negra
                spriteRenderer.color = Color.black;
            }
            else
            {
                // Si choca con un asteroide, desaparece (apagamos el dibujo)
                spriteRenderer.enabled = false;
            }
        }

        Invoke("ReiniciarNivel", 2f);
    }

    void ReiniciarNivel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name 
        );
    }
}