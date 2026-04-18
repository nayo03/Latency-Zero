using TMPro;
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
    public TextMeshProUGUI textoPuntuacionFinal;

    [Header("Interfaz Local")]
    public TextMeshProUGUI textoUI;
    public GameObject panelVictoria;
    public GameObject botonContinuar;
    public GameObject botonSalir;

    private int intentosActuales = 0;
    private int puntosTotales = 0;

    [System.Serializable]
    public struct DificultadIntento
    {
        public string nombre;
        public float velocidad;
        public float minPerfect;
        public float maxPerfect;
        public float minGood;
        public float maxGood;
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
                configActual.minPerfect,
                configActual.maxPerfect,
                configActual.minGood,
                configActual.maxGood
            );
        }

    }

    [Header("Paramentros Nivel Secreto")]
    public TextMeshProUGUI textoTimer;
    public float tiempoSmasher = 5.0f;
    public GameObject finalSecreto;
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

        finalSecreto.SetActive(true);
        maxSmasherActivado = true;
        clicksSmasher = 0;

        float timer = tiempoSmasher;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            textoTimer.text = "�Pressiona el espacio R�pido!" + timer.ToString("F1") + "s";

            if (Input.GetKeyDown(KeyCode.Space))
            {
                RegistrarClickSmasher();
            }
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    RegistrarClickSmasher();
                }
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
            }

            if (textoUI != null)
            {
                textoUI.text = "Puntos: " + puntosTotales;
            }

        }
    }

    private void FinalizarNivelSecreto()
    {
        maxSmasherActivado = false;
        finalSecreto.SetActive(false);

        Debug.Log("Puntos extra por clicks: " + clicksSmasher * puntosPorSmash);
        TerminarMinijuego();
    }

    private void TerminarMinijuego()
    {
        if (controladorAguja != null)
        {
            controladorAguja.SwitchOffNeedle();
        }

        if (textoPuntuacionFinal != null)
        {
            textoPuntuacionFinal.text = "Puntuaci�n Final: " + puntosTotales;
        }

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true); // Habilita el canvas de resultados
            // Gestion del Cursor (del rat�n): Se habilita para permitir la interacci�n con la UI
            panelVictoria.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None; // Estado del rat�n: se mueve de forma normal (None)

            // DETERMINACI�N DEL FLUJO DE SALIDA SEG�N EL MODO DE JUEGO
            if (MainManager.Instance != null)
            {
                if (MainManager.Instance.modoHistoriaActivo) // El modo historia est� activo? Aparece el bot�n continuar y salir.
                {
                    botonContinuar.SetActive(true);
                    botonSalir.SetActive(true);
                }
                else
                {
                    botonContinuar.SetActive(false); // El modo historia est� activo? Se desactiva el bot�n continuar y se deja activo salir.
                    botonSalir.SetActive(true);
                }
            }
        }
    }
}