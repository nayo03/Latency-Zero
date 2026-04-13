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

    [SerializeField] public int NivelActual = 1; // Nivel que estamos jugando (público para el Spawner)
    public int NivelesTotales = 3;               // Cuántos niveles hay que pasar para ganar

    // ----------- REFERENCIAS INTERNAS -----------
    [Header("Referencias UI y Paneles")]
    [SerializeField] private G2_UIManager uiManager; // Acceso al script que dibuja los textos en pantalla
    public GameObject G2_VictoryPanel;               // Panel que se activa al completar el nivel final
    public GameObject G2_LevelPanel;                 // Panel de transición entre niveles

    [Header("Botones de Victoria Final")]
    public GameObject G2_ButtonContinue;             // Botón que solo sale si estamos en Modo Historia
    public GameObject G2_ButtonExit;                 // Botón para volver al menú de selección

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

        // 4. Dibujamos los datos iniciales en la interfaz
        ActualizarTodoEnUI();
    }

    // ==========================================================================
    // ----------- BUCLE DE JUEGO -----------
    // ==========================================================================
    void Update()
    {
        if (juegoTerminado) return; // Si el jugador ha muerto o hemos ganado, dejamos de descontar tiempo

        // Cuenta atrás del cronómetro
        if (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime; // Mientras quede tiempo, restamos la fracción de segundo que ha pasado (deltaTime)
            ActualizarTodoEnUI();
        }
        else
        {
            // El reloj llega a cero: bloqueamos el tiempo y decidimos si pasar fase o ganar
            tiempoRestante = 0;
            ActualizarTodoEnUI();

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

            // --- LÓGICA DE BOTONES SEGÚN EL MODO ---
            if (MainManager.Instance != null)
            {
                // Si el modo historia está activo, permitimos "Continuar" a la siguiente escena
                if (MainManager.Instance.modoHistoriaActivo)
                {
                    if (G2_ButtonContinue != null) G2_ButtonContinue.SetActive(true);
                    if (G2_ButtonExit != null) G2_ButtonExit.SetActive(true);
                }
                else
                {
                    // Si es juego libre, ocultamos el botón de continuar
                    if (G2_ButtonContinue != null) G2_ButtonContinue.SetActive(false);
                    if (G2_ButtonExit != null) G2_ButtonExit.SetActive(true);
                }
            }
        }

        // Al ganar el minijuego completo, reseteamos los estáticos para la próxima vez
        ResetCheckpoints();
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