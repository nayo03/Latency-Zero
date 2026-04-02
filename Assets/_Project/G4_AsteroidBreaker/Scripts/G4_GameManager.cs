using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.XR.Management;

// ==============================================================================
// >>> G4_GAMEMANAGER: Controlador específico del Minijuego 4 (AR)
// Este es el "cerebro" local de vuestro nivel. Se encarga de contar los items,
// gestionar la cámara AR y avisar al MainManager cuando ganamos.
/* ---------------------------------------------------------------------------------
    NOTAS BÁSICAS (COMUNES A TODOS LOS NIVELES)
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

    ---------------------------------------------------------------------------------
    NOTAS ESPECÍFICAS DEL NIVEL 4 (AR)
    - panelCargandoAR: Se desactiva automáticamente tras 1.5s cuando la cámara está lista.
    - objetoARSession: Debe contener el AR Session y el Origin para que el script los gestione.
    ** No le he centrado la camara
    ** No he arreglado la pequeña espera que hay para que cargue la camara AR

    --------------------------------------------------------------------------------- */

public class G4_GameManager : MonoBehaviour
{
    [Header("Configuración AR")]
    public GameObject objetoARSession; // He arrastrado aquí AR Session 

    [Header("Configuración de Juego")]
    public int itemsParaGanar = 2;
    public TextMeshProUGUI textoPuntos;
    public GameObject panelVictoria;

    [Header("Botones de Victoria")]
    public GameObject botonContinuar;
    public GameObject botonSalir;

    private int itemsActuales = 0;
    private int puntosTotales = 0;

    void Start()
    {
        // Forzamos reinicio de AR para evitar "pantallazos negros"
        if (objetoARSession != null) objetoARSession.SetActive(false);
        StartCoroutine(ReactivarAR());
    }

    IEnumerator ReactivarAR()
    {
        // Verificación de los subsistemas de Unity XR
        if (XRGeneralSettings.Instance.Manager.activeLoader != null)
        {
            XRGeneralSettings.Instance.Manager.StartSubsystems();
        }
        else
        {
            yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
            if (XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                XRGeneralSettings.Instance.Manager.StartSubsystems();
            }
        }

        yield return new WaitForSeconds(0.5f);
        if (objetoARSession != null) objetoARSession.SetActive(true);
    }

    public void ItemRecogido()
    {
        itemsActuales++;
        puntosTotales += 20;

        if (textoPuntos != null)
            textoPuntos.text = "Puntos: " + puntosTotales;

        // Mandamos los puntos al registro temporal del MainManager
        if (MainManager.Instance != null)
            MainManager.Instance.SumarPuntoTemporal(20);

        if (itemsActuales >= itemsParaGanar)
        {
            GanarMinijuego();
        }
    }

    private void GanarMinijuego()
    {
        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);

            // Liberamos el cursor del ratón (importante para emulador de AR en PC)
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (MainManager.Instance != null)
            {
                bool modoHistoria = MainManager.Instance.modoHistoriaActivo;
                if (botonContinuar != null) botonContinuar.SetActive(modoHistoria);
                if (botonSalir != null) botonSalir.SetActive(true);
            }
        }
    }
}