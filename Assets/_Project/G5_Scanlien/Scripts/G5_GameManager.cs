using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.XR.Management;

// ==============================================================================
// >>> G5_GAMEMANAGER: Controlador específico del Minijuego 5 (VR)
// Este es el "cerebro" local de vuestro nivel final. Se encarga de la VR,
// contar los items y avisar al MainManager cuando el jugador gana.
/* ---------------------------------------------------------------------------------
    NOTAS BÁSICAS (COMUNES A TODOS LOS NIVELES)
    --- PUNTOS Y DATOS (MainManager) ---
    - MainManager.Instance.SumarPuntoTemporal(int) -> Suma puntos SOLO en vuestro nivel. 
    - MainManager.Instance.modoHistoriaActivo      -> (Bool) Para saber si es Historia o Libre.

    --- INTERFAZ Y NAVEGACIÓN (UIMainManager) ---
    - UIMainManager.Instance.Boton_FinalDelJuego() -> Guarda puntos y avanza (Botón Continuar).
    - UIMainManager.Instance.Boton_AbandonarPartida() -> Retorno al menú con limpieza.

    ---------------------------------------------------------------------------------
    NOTAS ESPECÍFICAS DEL NIVEL 5 (VR)
    - LIMPIEZA: El Awake elimina cámaras intrusas del simulador para que no haya conflictos.
    - REACTIVAR XR: Corrutina que fuerza el encendido del casco VR al entrar en la escena.
    - Cuidao con los botones para salir, en tu caso el continuar es "FinalDelJuego" y el salir es "AbandonarPartida")
    - 
    --------------------------------------------------------------------------------- */
// ==============================================================================

public class G5_GameManager : MonoBehaviour
{
    [Header("Configuración de Juego")]
    public int itemsParaGanar = 2;
    public TextMeshProUGUI textoPuntos;
    public GameObject panelVictoria;

    [Header("Configuración de Tiempo")]
    public float tiempoRestante = 180f; // 3 minutos en segundos
    public TextMeshProUGUI textoCronometro;
    public GameObject panelDerrota; // Panel extra por si se acaba el tiempo

    [Header("Botones de Victoria (UI)")]
    public GameObject botonContinuar;
    public GameObject botonSalir;

    private int itemsActuales = 0;
    private int puntosTemporalesG5 = 0;
    private bool juegoTerminado = false;

    private void Awake()
    {
        // Limpieza de cámaras sobrantes del simulador XR para evitar errores de renderizado
        string[] intrusos = { "SimulationCamera", "XR Simulation Data" };
        foreach (string nombre in intrusos)
        {
            GameObject obj = GameObject.Find(nombre);
            if (obj != null) DestroyImmediate(obj);
        }
    }

    void Start()
    {
        // Forzamos el inicio de los subsistemas de VR
        StartCoroutine(ReactivarXR());

        if (panelVictoria != null) panelVictoria.SetActive(false);
        if (panelDerrota != null) panelDerrota.SetActive(false);
    }

    void Update()
    {
        if (juegoTerminado) return;

        // Gestión del tiempo
        if (tiempoRestante > 0)
        {
            tiempoRestante -= Time.deltaTime;
            ActualizarInterfazTiempo();
        }
        else
        {
            tiempoRestante = 0;
            PerderPorTiempo();
        }
    }

    void ActualizarInterfazTiempo()
    {
        if (textoCronometro != null)
        {
            int minutos = Mathf.FloorToInt(tiempoRestante / 60);
            int segundos = Mathf.FloorToInt(tiempoRestante % 60);
            textoCronometro.text = string.Format("{0:00}:{1:00}", minutos, segundos);

            // Opcional: Poner el texto en rojo si queda poco tiempo
            if (tiempoRestante < 30) textoCronometro.color = Color.red;
        }
    }

    IEnumerator ReactivarXR()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        // Verificación de seguridad en cascada
        if (XRGeneralSettings.Instance != null && XRGeneralSettings.Instance.Manager != null)
        {
            if (XRGeneralSettings.Instance.Manager.activeLoader == null)
            {
                yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
            }

            if (XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                XRGeneralSettings.Instance.Manager.StartSubsystems();
            }
        }
    }

    public void ItemRecogido()
    {
        itemsActuales++;
        puntosTemporalesG5 += 20;

        if (textoPuntos != null)
            textoPuntos.text = "Puntos: " + puntosTemporalesG5;

        // Registro global de puntos
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

            // Liberamos el ratón para el menú de victoria
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (MainManager.Instance != null)
            {
                bool esHistoria = MainManager.Instance.modoHistoriaActivo;
                if (botonContinuar != null) botonContinuar.SetActive(esHistoria);
                if (botonSalir != null) botonSalir.SetActive(true);
            }
        }
    }

    private void PerderPorTiempo()
    {
        juegoTerminado = true;
        MostrarMenuFinal(panelDerrota);
    }

    private void MostrarMenuFinal(GameObject panelAActivar)
    {
        if (panelAActivar != null) panelAActivar.SetActive(true);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (MainManager.Instance != null)
        {
            bool esHistoria = MainManager.Instance.modoHistoriaActivo;
            if (botonContinuar != null) botonContinuar.SetActive(esHistoria && panelAActivar == panelVictoria);
            if (botonSalir != null) botonSalir.SetActive(true);
        }
    }
}