using UnityEngine;
using UnityEngine.SceneManagement;

// ==============================================================================
// >>> G2_GAMEMANAGER: Controlador específico del Minijuego 2
// ==============================================================================
public class G2_GameManager : MonoBehaviour
{
    // ----------- SINGLETON -----------
    public static G2_GameManager Instance;

    void Awake()
    {
        Instance = this; // Instancia global para que Player y Alien puedan avisar de eventos
    }

    // ----------- VARIABLES ESTÁTICAS (CHECKPOINTS) -----------
    // Estas variables NO se borran al usar LoadScene, permitiendo recordar datos entre niveles
    private static int NivelCheckpoint = 1;   // Guarda en qué nivel se quedó el jugador
    private static int PuntosCheckpoint = 0;  // Guarda los puntos acumulados para que no bajen a cero al cambiar de fase

    // ----------- CONFIGURACIÓN -----------
    [Header("Ajustes de Tiempo y Fases")]
    public float tiempoNivel = 40f;         // Segundos que dura cada fase del minijuego
    private float tiempoRestante;           // El reloj interno que descuenta segundos
    private bool juegoTerminado = false;    // Si es true, detiene el reloj y la suma de puntos
    private int puntosTotales = 0;          // Puntos acumulados en la partida actual
    public bool juegoIniciado = false;
    private static bool yaHaDadoAlStart = false;

    [SerializeField] public int NivelActual = 1; // Nivel que estamos jugando (público para el Spawner)
    public int NivelesTotales = 3;               // Cuántos niveles hay que pasar para ganar

    // ----------- REFERENCIAS INTERNAS -----------
    [Header("Referencias UI y Paneles")]
    [SerializeField] private G2_UIManager uiManager; // Acceso al script que dibuja los textos en pantalla
    public GameObject G2_StartPanel;                 // Panel de inicio
    public GameObject G2_LevelPanel;                 // Panel de transición entre niveles
    public GameObject G2_VictoryPanel;               // Panel que se activa al completar el nivel final     

    [Header("Botones de Victoria Final")]
    public GameObject G2_ButtonContinue;             // Botón que solo sale si estamos en Modo Historia
    public GameObject G2_ButtonVolver;               // Botón para volver al menú de selección
    public GameObject G2_ButtonVolver_Historia;      // Botón volver recolocado para modo historia
    public GameObject G2_ButtonVolver_sincontinue;   // Botón para volver al menú de selección reubicado

    // ==========================================================================
    // ----------- INICIO -----------
    // ==========================================================================
    void Start()
    {
        // 1. Cargamos el progreso desde el último checkpoint
        NivelActual = NivelCheckpoint;
        puntosTotales = PuntosCheckpoint;

        // 2. Sincronizamos con el MainManager para el conteo global
        if (MainManager.Instance != null)
        {
            MainManager.Instance.puntosEnEsteMinijuego = puntosTotales;
        }

        // 3. Iniciamos el cronómetro y nos aseguramos de que el tiempo no esté pausado
        tiempoRestante = tiempoNivel;
        Time.timeScale = 1f;

        // 4. Inicio
        if (NivelActual == 1 && G2_StartPanel != null && !yaHaDadoAlStart)
        {
            G2_StartPanel.SetActive(true);
            juegoIniciado = false;
            Time.timeScale = 0f; // Pausa total la primera vez que entra
        }
        else
        {
            if (G2_StartPanel != null) G2_StartPanel.SetActive(false);
            juegoIniciado = true;
            Time.timeScale = 1f; // Empieza directo si ya ha muerto
        }

        // 5. Dibujamos los datos iniciales en la interfaz
        ActualizarTodoEnUI();


    }

    // ==========================================================================
    // ----------- BUCLE DE JUEGO -----------
    // ==========================================================================
    void Update()
    {
        if (!juegoIniciado || juegoTerminado) return;

        // Cuenta atrás del cronómetro
        if (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;

            // Evita que el tiempo baje de 0 y aparezca -0.01
            if (tiempoRestante < 0)
            {
                tiempoRestante = 0;
            }

            ActualizarTodoEnUI();

            // Cuando llega a 0, pasamos de fase o ganamos
            if (tiempoRestante <= 0)
            {
                if (NivelActual < NivelesTotales)
                {
                    TerminarFase();
                }
                else
                {
                    GanarMinijuego();
                }
            }
        }
    }

    // =========================================================================
    // ----------- GESTIÓN DE INICIO -----------
    // =========================================================================
    public void IniciarJuego()
    {
        yaHaDadoAlStart = true;

        // 1. Activamos el tiempo
        Time.timeScale = 1f;

        // 2. Marcamos que ha iniciado
        juegoIniciado = true;

        // 3. Desactivamos el panel
        if (G2_StartPanel != null) G2_StartPanel.SetActive(false);

        // 4. Transportamos la nave a su sitio
        G2_Player player = Object.FindAnyObjectByType<G2_Player>();
        if (player != null)
        {
            // Evita que el click del botón Start se convierta en salto.
            player.IgnorarInputInicial();
            player.IniciarPosicionJugador();
        }
    }

    // =========================================================================
    // ----------- GESTIÓN DE PUNTOS -----------
    // =========================================================================
    public void ItemRecogido(int puntosGanados)
    {
        if (juegoTerminado) return; // Si ya hemos muerto o terminado, ignoramos cualquier punto extra

        puntosTotales += puntosGanados; // Sumamos al contador local de esta escena

        // Informamos al MainManager para que él gestione el sonido y los puntos de "historia"
        if (MainManager.Instance != null)
        {
            MainManager.Instance.SumarPuntoTemporal(puntosGanados);
        }

        ActualizarTodoEnUI();
    }

    // =========================================================================
    // ----------- GESTIÓN DE UI -----------
    // =========================================================================
    private void ActualizarTodoEnUI()
    {
        // Le pasamos los datos al UIManager para que él los dibuje
        if (uiManager != null)
        {
            uiManager.ActualizarInterfaz(puntosTotales, tiempoRestante);
        }
    }

    // =========================================================================
    // ----------- TERMINAR NIVEL -----------
    // =========================================================================
    private void TerminarFase()
    {
        if (juegoTerminado) return;
        juegoTerminado = true;

        if (G2_LevelPanel != null) // Mostramos el panel de nivel completado y pausamos para que el jugador respire
        {
            G2_LevelPanel.SetActive(true);
            Time.timeScale = 0f; // Pausa solo al mostrar el menú de "Nivel Completado"
        }
    }

    // =========================================================================
    // ----------- GANAR MINIJUEGO -----------
    // =========================================================================
    private void GanarMinijuego()
    {
        if (juegoTerminado) return; // Seguridad para no ejecutar la victoria varias veces
        juegoTerminado = true; // Bloqueamos el estado del juego

        if (G2_VictoryPanel != null)
        {
            G2_VictoryPanel.SetActive(true); // Encendemos el panel de victoria final
            Time.timeScale = 0f; // Congelamos el movimiento del juego

            // Comprobamos si estamos en modo historia
            bool modoHistoria = MainManager.Instance != null && MainManager.Instance.modoHistoriaActivo;

            // --- LÓGICA DE BOTONES SEGÚN EL MODO ---
            if (modoHistoria)
            {
                // MODO HISTORIA:
                // Aparece Continue. Aparece el botón volver específico de historia. Se ocultan los botones volver de modo libre.
                if (G2_ButtonContinue != null) G2_ButtonContinue.SetActive(true);
                if (G2_ButtonVolver != null) G2_ButtonVolver.SetActive(true);
                if (G2_ButtonVolver_sincontinue != null) G2_ButtonVolver_sincontinue.SetActive(false);
            }
            else
            {
                // MODO LIBRE:
                // No aparece Continue. Aparece solo el botón volver recolocado para cuando NO hay Continue. Se ocultan el volver normal y el volver de historia.
                if (G2_ButtonContinue != null) G2_ButtonContinue.SetActive(false);
                if (G2_ButtonVolver != null) G2_ButtonVolver.SetActive(false);
                if (G2_ButtonVolver_sincontinue != null) G2_ButtonVolver_sincontinue.SetActive(true);
            }

            // Al ganar el minijuego completo, reseteamos los estáticos para la próxima vez
            ResetCheckpoints();
        }
    }

    // =========================================================================
    // ----------- SIGUIENTE NIVEL -----------
    // =========================================================================
    public void NextLevel()
    {
        // 1. Antes de irnos, guardamos el progreso en los Checkpoints estáticos
        NivelCheckpoint = NivelActual + 1; // Subimos el nivel
        PuntosCheckpoint = puntosTotales;  // Mantenemos los puntos

        // 2. IMPORTANTE: Ponemos el tiempo a 1 antes de recargar
        Time.timeScale = 1f;

        // 3. RECARGAMOS LA ESCENA
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // =========================================================================
    // ----------- SEGURIDAD -----------
    // =========================================================================
    public static void ResetCheckpoints()
    {
        // Función para limpiar los puntos estáticos (Llamar al salir al Menú)
        NivelCheckpoint = 1;
        PuntosCheckpoint = 0;
        yaHaDadoAlStart = false;
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    // =========================================================================
    // ----------- GESTIÓN DE ESTADOS: GAME OVER -----------
    // =========================================================================
    public void Morir()
    {
        if (juegoTerminado) return;

        juegoTerminado = true; // Detiene el cronómetro en el Update sin pausar el mundo
    }
}