using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

// ==============================================================================
// >>> G1_GAMEMANAGER: Controlador espec�fico del Minijuego 1
// Este es el "cerebro" local de vuestro nivel. Se encarga de contar los intentos
// y avisar al MainManager (el motor global) cuando ganamos.
/* ---------------------------------------------------------------------------------
   NOTAS B�SICAS  
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
public class G1_GameManager : MonoBehaviour
{
    // VARIABLES
    [Header("Configuraci�n de Reglas")]
    public int intentosMaximos = 5;

    [Header("Referencias del Nivel")]
    public DeadlightController controladorAguja;
    public G1_UI_Controller uiLocal;

    private int intentosActuales = 0;
    private int puntosTotales = 0;

    [System.Serializable]
    public struct DificultadIntento
    {
        public string nombre;
        public float velocidad;
        public float iAnglePerfect;
        public float fAnglePerfect;
        public float iAngleGood;
        public float fAngleGood;
    }

    [Header("Configuraci�n de Progresi�n")]
    public DificultadIntento[] nivelesDificultad;

    private void Start()
    {
        intentosMaximos = nivelesDificultad.Length;
        ActualizarDificultadJuego();
    }
    private void ActualizarDificultadJuego()
    {
        if (intentosActuales < nivelesDificultad.Length)
        {
            DificultadIntento configActual = nivelesDificultad[intentosActuales];

            controladorAguja.switchDifficulty(
                configActual.velocidad,
                configActual.iAnglePerfect,
                configActual.fAnglePerfect,
                configActual.iAngleGood,
                configActual.fAngleGood
            );

            uiLocal.SincronizarDial(
                configActual.iAnglePerfect,
                configActual.fAnglePerfect, 
                configActual.iAngleGood,
                configActual.fAngleGood
            );
        }

    }

    [Header("Paramentros Nivel Secreto")]
    public float tiempoSmasher = 5.0f;
    public int puntosPorSmash = 100;

    private int clicksSmasher = 0;
    private bool maxSmasherActivado = false;
    public int puntosParaNivelSecreto = 200;

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

    private void OnPress()
    {

    }
    private void ProcesarIntento(int punctuation)
    {
        int calidadObtenida = 0;
        if (punctuation >= 100) calidadObtenida = 2;
        else if (punctuation >= 50) calidadObtenida = 1;

        if (calidadObtenida == 0) uiLocal.IniciarFlashFallo();

        uiLocal.IniciarVibracion();
        uiLocal.RenderizarPiezaCohete(intentosActuales, calidadObtenida);

        intentosActuales++;
        puntosTotales += punctuation;

        // Sent to MainManager the points obtained in this attempt
        if (MainManager.Instance != null)
        {
            MainManager.Instance.SumarPuntoTemporal(punctuation);
        }

        uiLocal.ActualizarTextoPuntos(puntosTotales);

        if (intentosActuales < intentosMaximos)
        {
            ActualizarDificultadJuego();
        }
        else if (intentosActuales == intentosMaximos && puntosTotales >= puntosParaNivelSecreto)
        {
            StartCoroutine(ActivarNivelSecreto());
        }
        else
        {
            TerminarMinijuego();
        }

        
    }

    private IEnumerator ActivarNivelSecreto()
    {
        controladorAguja.SwitchOffNeedle();
        yield return new WaitForSeconds(1f);
        uiLocal.ActivarInterfazSmasher(true);
        maxSmasherActivado = true;
        clicksSmasher = 0;

        float timer = tiempoSmasher;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            uiLocal.ActualizarCronometroSmasher(timer);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                RegistrarClickSmasher();
                uiLocal.FeedbackPulsacionBoton();
                uiLocal.IniciarVibracion();
            }
            yield return null;
        }
        FinalizarNivelSecreto();
    }

    private void RegistrarClickSmasher()
    {
        if (maxSmasherActivado)
        {
            clicksSmasher++;
            puntosTotales += puntosPorSmash;

            if (MainManager.Instance != null)
            {
                MainManager.Instance.SumarPuntoTemporal(puntosPorSmash);
                uiLocal.ActualizarTextoPuntos(puntosTotales);
            }
        }
    }

    private void FinalizarNivelSecreto()
    {
        maxSmasherActivado = false;
        uiLocal.ActivarInterfazSmasher(false);
        TerminarMinijuego();
    }

    private void TerminarMinijuego()
    {
        if (controladorAguja != null)
        {
            controladorAguja.SwitchOffNeedle();
            bool isHistory = MainManager.Instance != null && MainManager.Instance.modoHistoriaActivo;
            uiLocal.MostrarPantallaFinal(puntosTotales, isHistory);
        }
    }
}