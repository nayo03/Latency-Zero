using UnityEngine;
using TMPro;

// ==============================================================================
// >>> G3_GAMEMANAGER: Controlador espec�fico del Minijuego 3
// Este es el "cerebro" local de vuestro nivel. Se encarga de contar los items
// y avisar al MainManager (el motor global) cuando ganamos.
/* ---------------------------------------------------------------------------------
    NOTAS B�SICAS (COMUNES A TODOS LOS NIVELES)
    --- PUNTOS Y DATOS (MainManager) ---
    - MainManager.Instance.SumarPuntoTemporal(int) -> Suma puntos SOLO en vuestro nivel. 
      Si el jugador abandona la escena o reinicia, este valor se limpia. Solo se guarda 
      en la base de datos al llamar a 'FinalizarEscenaActual()' y si est� en modo historia.
    - MainManager.Instance.modoHistoriaActivo      -> (Bool) Para saber si es modo Historia o Libre.

    --- INTERFAZ Y NAVEGACI�N (UIMainManager) ---
    - UIMainManager.Instance.Boton_FinalDelJuego() -> Guarda puntos, limpia RAM y 
      avanza en la historia (Usadlo en el bot�n "Siguiente/Continuar" al ganar).
    - UIMainManager.Instance.Boton_AbandonarPartida() -> Retorno al men� de selecci�n 
      con limpieza de valores temporales.

    --- CONFIGURACI�N DE ESCENAS ---
    *** !!! IMPORTANTE: Toda escena nueva debe registrarse en 'File > Build Settings'. 
        El orden en la lista determina el �ndice de carga en el Modo Historia. ***
    --------------------------------------------------------------------------------- */
// ==============================================================================

public class G3_GameManager : MonoBehaviour
{
    // Singleton: permite llamarlo desde cualquier script con G3_GameManager.Instance
    public static G3_GameManager Instance;

    [Header("UI")]
    public TextMeshProUGUI textoVidas;    // Texto que muestra las vidas en pantalla
    public TextMeshProUGUI textoPuntos;   // Texto que muestra los puntos en pantalla
    public GameObject panelVictoria;      // Panel que aparece al ganar
    public GameObject panelDerrota;       // Panel que aparece al perder

    [Header("Botones")]
    public GameObject botonContinuar;     // Botón para continuar en modo historia
    public GameObject botonSalir;         // Botón para salir al menú

    private int _puntosTotales = 0;       // Puntos acumulados en este minijuego

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
        ActualizarUI();
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

    // Se llama cuando el jugador se queda sin vidas
    public void PerderPartida()
    {
        Debug.Log("Game Over");
        Time.timeScale = 0f; // Pausamos el juego

        if (panelDerrota != null)
        {
            panelDerrota.SetActive(true);
            botonSalir.SetActive(true);
        }
    }

    // Se llama cuando el jugador derrota todas las oleadas y el boss
    public void GanarPartida()
    {
        Debug.Log("Victoria");
        Time.timeScale = 0f; // Pausamos el juego

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);

            if (MainManager.Instance != null)
            {
                bool historia = MainManager.Instance.modoHistoriaActivo;
                botonContinuar.SetActive(historia);
                botonSalir.SetActive(true);
            }
        }
    }

    // Actualiza los textos de la UI con los valores actuales
    private void ActualizarUI()
    {
        if (textoPuntos != null)
            textoPuntos.text = "Puntos: " + _puntosTotales;
    }

    // Actualiza el texto de vidas desde el Player
    public void ActualizarVidas(int vidas)
    {
        if (textoVidas != null)
            textoVidas.text = "Vidas: " + vidas;
    }
    
    // Se llama al pulsar el botón de salir
    public void BotonSalirPulsado()
    {
        Debug.Log("BOTON SALIR PULSADO"); 
        // Reanudamos el tiempo por si estaba pausado
        Time.timeScale = 1f;
    
        // Usamos el MainManager para volver al menú
        if (MainManager.Instance != null)
        {
            MainManager.Instance.FinalizarEscenaActual();
            
        }
    }
}