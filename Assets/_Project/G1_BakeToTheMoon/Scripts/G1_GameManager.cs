using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// ==============================================================================
// >>> G1_GAMEMANAGER: Controlador específico del Minijuego 1
// Este es el "cerebro" local de vuestro nivel. Se encarga de contar los intentos
// y avisar al MainManager (el motor global) cuando ganamos.
/* ---------------------------------------------------------------------------------
   NOTAS BÁSICAS  
   --- PUNTOS Y DATOS (MainManager) ---
   - MainManager.Instance.SumarPuntoTemporal(int) -> Suma puntos SOLO en vuestro nivel. 
     Si el jugador abandona la escena o reinicia, este valor se limpia. Solo se guarda 
     en la base de datos al llamar a 'FinalizarEscenaActual()' y si está en modo historia.
   - MainManager.Instance.modoHistoriaActivo      -> (Bool) Para saber si es modo Historia o Libre.

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
public class G1_GameManager : MonoBehaviour
{
    // VARIABLES
    [Header("Configuración de Reglas")]
    public int intentosMaximos = 5;

    [Header("Referencias del Nivel")]
    public DeadlightController controladorAguja;
    public TextMeshProUGUI textoPuntuaciónFinal;

    [Header("Interfaz Local")]
    public TextMeshProUGUI textoUI;
    public GameObject panelVictoria;
    public GameObject botonContinuar;
    public GameObject botonSalir;

    private int intentosActuales = 0;
    private int puntosTotales = 0;

    private void OnEnable()
    {
        // Sincronization with DeadlightController: Subscribe to the OnTryComplete event to receive updates on attempts
        DeadlightController.OnTryComplete += ProcesarIntento;
    }
    private void OnDisable()
    {
        // Unsynchronization with DeadlightController: Unsubscribe from the OnTryComplete event to prevent memory leaks and unintended behavior when this object is disabled
        DeadlightController.OnTryComplete -= ProcesarIntento;
    }

    // 
    private void ProcesarIntento(int punctuation)
    {
        intentosActuales++;
        puntosTotales += punctuation;

        if (textoUI != null)
        {
            textoUI.text = "Puntos: " + puntosTotales;
        }

        // Sent to MainManager the points obtained in this attempt
        if (MainManager.Instance != null)
        {
            MainManager.Instance.SumarPuntoTemporal(punctuation);
        }

        if (intentosActuales >= intentosMaximos)
        {
            TerminarMinijuego();
        }
    }

    private void TerminarMinijuego()
    {
        if (controladorAguja != null)
        {
            controladorAguja.SwitchOffNeedle();
        }

        if (textoPuntuaciónFinal != null)
        {
            textoPuntuaciónFinal.text = "Puntuación Final: " + puntosTotales;
        }

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true); // Habilita el canvas de resultados
            // Gestión del Cursor (del ratón): Se habilita para permitir la interacción con la UI
            panelVictoria.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None; // Estado del ratón: se mueve de forma normal (None)

            // DETERMINACIÓN DEL FLUJO DE SALIDA SEGÚN EL MODO DE JUEGO
            if (MainManager.Instance != null)
            {
                if (MainManager.Instance.modoHistoriaActivo) // El modo historia está activo? Aparece el botón continuar y salir.
                {
                    botonContinuar.SetActive(true);
                    botonSalir.SetActive(true);
                }
                else
                {
                    botonContinuar.SetActive(false); // El modo historia está activo? Se desactiva el botón continuar y se deja activo salir.
                    botonSalir.SetActive(true);
                }
            }
        }
    }
}