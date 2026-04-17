using UnityEngine;
using TMPro;

// ==============================================================================
// >>> G3_GAMEMANAGER: Controlador específico del Minijuego 3
// Este es el "cerebro" local de vuestro nivel. Se encarga de gestionar
// la puntuación, vidas, victoria/derrota y navegación entre escenas.
/* ---------------------------------------------------------------------------------
    NOTAS BÁSICAS (COMUNES A TODOS LOS NIVELES)
    --- PUNTOS Y DATOS (MainManager) ---
    - MainManager.Instance.SumarPuntoTemporal(int) -> Suma puntos SOLO en vuestro nivel. 
      Si el jugador abandona la escena o reinicia, este valor se limpia. Solo se guarda 
      en la base de datos al llamar a 'FinalizarEscenaActual()' y si está en modo historia.
    - MainManager.Instance.modoHistoriaActivo -> (Bool) Para saber si es modo Historia o Libre.

    --- INTERFAZ Y NAVEGACIÓN (UIMainManager) ---
    - UIMainManager.Instance.Boton_FinalDelJuego() -> Guarda puntos, limpia RAM y 
      avanza en la historia (Usadlo en el botón "Siguiente/Continuar" al ganar).
    - UIMainManager.Instance.Boton_AbandonarPartida() -> Retorno al menú de selección 
      con limpieza de valores temporales.

    --- CONFIGURACIÓN DE ESCENAS ---
    *** !!! IMPORTANTE: Toda escena nueva debe registrarse en 'File > Build Settings'. 
        El orden en la lista determina el índice de carga en el Modo Historia. ***
    --------------------------------------------------------------------------------- */
// ==============================================================================

public class G3_GameManager : MonoBehaviour
{
    // Singleton: permite llamarlo desde cualquier script con G3_GameManager.Instance
    public static G3_GameManager Instance;

    [Header("UI")]
    public TextMeshProUGUI textoVidas;      // Texto que muestra las vidas en pantalla
    public TextMeshProUGUI textoPuntos;     // Texto que muestra los puntos en pantalla
    public TextMeshProUGUI textoResultado;  // Texto que cambia entre "¡HAS GANADO!" y "¡HAS PERDIDO!"

    [Header("Panel Final")]
    public GameObject panelFinal;           // Panel único para victoria y derrota
    public GameObject botonContinuar;       // Solo visible en modo historia al ganar
    public GameObject botonJugarDeNuevo;    // Siempre visible
    public GameObject botonSalir;           // Siempre visible

    private int _puntosTotales = 0;         // Puntos acumulados en este minijuego

    void Awake()
    {
        // Singleton: guardamos la referencia a este GameManager
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Panel oculto al inicio
        if (panelFinal != null)
            panelFinal.SetActive(false);

        ActualizarUI();
        
        // DEBUG: muestra en consola si estamos en modo historia o no
        if (MainManager.Instance != null)
            Debug.Log("DEBUG: Modo historia " + (MainManager.Instance.modoHistoriaActivo ? "ON" : "OFF"));
    }

    // Suma puntos al matar un enemigo y avisa al MainManager
    public void SumarPuntos(int cantidad)
    {
        _puntosTotales += cantidad;
        ActualizarUI();

        if (MainManager.Instance != null)
        {
            MainManager.Instance.SumarPuntoTemporal(cantidad);
        }
    }

    // Actualiza el texto de vidas desde el Player
    public void ActualizarVidas(int vidas)
    {
        if (textoVidas != null)
            textoVidas.text = "Vidas: " + vidas;
    }

    // Actualiza los textos de la UI con los valores actuales
    private void ActualizarUI()
    {
        if (textoPuntos != null)
            textoPuntos.text = "Puntos: " + _puntosTotales;
    }

    // Método central que muestra el panel final según si has ganado o perdido
    private void MostrarPanelFinal(bool ganado)
    {
        Time.timeScale = 0f; // Pausamos el juego

        if (panelFinal != null)
        {
            panelFinal.SetActive(true);

            // Cambiamos el texto según el resultado
            if (textoResultado != null)
                textoResultado.text = ganado ? "¡HAS GANADO!" : "¡HAS PERDIDO!";

            // Continuar solo aparece en modo historia Y si has ganado
            bool historia = MainManager.Instance != null && MainManager.Instance.modoHistoriaActivo;
            botonContinuar.SetActive(historia && ganado);

            // Estos siempre visibles
            botonJugarDeNuevo.SetActive(true);
            botonSalir.SetActive(true);
        }
    }

    // Se llama cuando el jugador se queda sin vidas
    public void PerderPartida()
    {
        Debug.Log("Game Over");
        MostrarPanelFinal(false);
    }

    // Se llama cuando el jugador derrota todas las oleadas
    public void GanarPartida()
    {
        Debug.Log("Victoria");
        MostrarPanelFinal(true);
    }

    // Se llama al pulsar el botón continuar (modo historia)
    public void BotonContinuarPulsado()
    {
        Time.timeScale = 1f;
        if (MainManager.Instance != null)
            MainManager.Instance.FinalizarEscenaActual();
    }

    // Reinicia la escena actual para jugar de nuevo
    public void BotonJugarDeNuevo()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }

    // Se llama al pulsar el botón de salir
    public void BotonSalirPulsado()
    {
        Debug.Log("BOTON SALIR PULSADO");
        Time.timeScale = 1f;
        if (MainManager.Instance != null)
            MainManager.Instance.FinalizarEscenaActual();
    }
    
    void Update()
    {
        // DEBUG: Pulsa G para ganar, P para perder
        if (Input.GetKeyDown(KeyCode.G))
            GanarPartida();
    
        if (Input.GetKeyDown(KeyCode.P))
            PerderPartida();
    }
}