using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.XR.Management;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.Android;

public class G4_GameManager : MonoBehaviour
{
    [Header("Configuración AR")]
    public GameObject objetoARSession;

    [Header("UI del Juego")]
    public TextMeshProUGUI textoPuntos;
    public TextMeshProUGUI textoTiempo; // NUEVO: Para el contador de 2 min
    public GameObject panelVictoria;
    public GameObject botonContinuar;
    public GameObject botonSalir;

    [Header("Referencias")]
    public G4_AsteroidSpawner spawner;

    // Variables internas del GDD
    private float tiempoRestante = 120f; // 2 minutos de juego
    private int puntosTotales = 0;
    private int comboActual = 0;
    private bool juegoActivo = false;

    [Header("Debug AR")]
    public TextMeshProUGUI debugTextAR;
    public ARSession arSession;
    public Camera camaraARDebug;
    private float debugTimer = 0f;

    void Start()
{
    if (debugTextAR != null)
        debugTextAR.text = "=== DEBUG AR ===";

#if UNITY_ANDROID && !UNITY_EDITOR
    DebugAR("Android detectado");

    if (Permission.HasUserAuthorizedPermission(Permission.Camera))
    {
        DebugAR("Permiso cámara: CONCEDIDO");
    }
    else
    {
        DebugAR("Permiso cámara: NO concedido. Solicitando...");
        Permission.RequestUserPermission(Permission.Camera);
    }
#else
    DebugAR("No Android o Editor");
#endif

    if (objetoARSession != null)
    {
        DebugAR("Objeto AR Session asignado: " + objetoARSession.name);
        objetoARSession.SetActive(false);
        DebugAR("Objeto AR Session desactivado por Start()");
    }
    else
    {
        DebugAR("Objeto AR Session: NULL");
    }

    StartCoroutine(ReactivarAR());
}

    IEnumerator ReactivarAR()
    {
        DebugAR("Iniciando ReactivarAR()");

        if (XRGeneralSettings.Instance == null)
        {
            DebugAR("ERROR: XRGeneralSettings.Instance es NULL");
            yield break;
        }

        DebugAR("XRGeneralSettings.Instance: OK");

        if (XRGeneralSettings.Instance.Manager == null)
        {
            DebugAR("ERROR: XR Manager es NULL");
            yield break;
        }

        DebugAR("XR Manager: OK");

        if (XRGeneralSettings.Instance.Manager.activeLoader != null)
        {
            DebugAR("Active Loader ya existe: " + XRGeneralSettings.Instance.Manager.activeLoader.name);
            XRGeneralSettings.Instance.Manager.StartSubsystems();
            DebugAR("StartSubsystems ejecutado");
        }
        else
        {
            DebugAR("Active Loader NULL. Inicializando loader...");
            yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

            if (XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                DebugAR("Loader inicializado: " + XRGeneralSettings.Instance.Manager.activeLoader.name);
                XRGeneralSettings.Instance.Manager.StartSubsystems();
                DebugAR("StartSubsystems ejecutado tras InitializeLoader");
            }
            else
            {
                DebugAR("ERROR: No se pudo inicializar ningún loader XR");
            }
        }

        if (objetoARSession != null)
        {
            objetoARSession.SetActive(true);
            DebugAR("Objeto AR Session reactivado");
        }

        for (int i = 1; i <= 10; i++)
        {
            yield return new WaitForSeconds(1f);
            DebugAR("Segundo " + i + " - ARSession.state: " + ARSession.state.ToString());
        }

#if UNITY_ANDROID && !UNITY_EDITOR
    DebugAR("Permiso cámara tras esperar: " +
        (Permission.HasUserAuthorizedPermission(Permission.Camera) ? "CONCEDIDO" : "NO CONCEDIDO"));
#endif

        DebugAR("ARSession.state: " + ARSession.state.ToString());

        if (arSession != null)
        {
            DebugAR("ARSession asignado en inspector: " + arSession.name);
            DebugAR("ARSession enabled: " + arSession.enabled);
            DebugAR("ARSession activeInHierarchy: " + arSession.gameObject.activeInHierarchy);
        }
        else
        {
            DebugAR("ARSession no asignado en inspector");
        }

        Camera cam = camaraARDebug != null ? camaraARDebug : Camera.main;

        if (cam != null)
        {
            DebugAR("Camera usada: " + cam.name);
            DebugAR("Camera enabled: " + cam.enabled);
            DebugAR("Camera activeInHierarchy: " + cam.gameObject.activeInHierarchy);

            var bg = cam.GetComponent<ARCameraBackground>();
            DebugAR("ARCameraBackground: " + (bg != null ? "EXISTE" : "NO EXISTE"));

            if (bg != null)
            {
                DebugAR("ARCameraBackground enabled: " + bg.enabled);
            }

            var manager = cam.GetComponent<ARCameraManager>();
            DebugAR("ARCameraManager: " + (manager != null ? "EXISTE" : "NO EXISTE"));

            if (manager != null)
            {
                DebugAR("ARCameraManager enabled: " + manager.enabled);
            }
        }
        else
        {
            DebugAR("ERROR: No se encontró Camera.main ni camaraARDebug");
        }

        ApagarCamarasIntrusas();

        juegoActivo = true;
        ActualizarUI();

        DebugAR("Juego activo");
    }

    private void ApagarCamarasIntrusas()
    {
        
        Camera[] todasLasCamaras = Object.FindObjectsByType<Camera>(FindObjectsInactive.Exclude);
    
        foreach (Camera cam in todasLasCamaras)
        {
        
            if (cam.gameObject.name != "Main Camera")
            {
                cam.gameObject.SetActive(false);
                Debug.Log("¡Cámara intrusa del Modo Historia desactivada con éxito!: " + cam.gameObject.name);
            }
        }
    }

    void Update()
    {
        debugTimer += Time.deltaTime;
        if (debugTimer >= 1f)
        {
            debugTimer = 0f;
            ActualizarDebugResumen();
        }

        if (!juegoActivo) return;

        tiempoRestante -= Time.deltaTime;
        ActualizarFaseDificultad();

        if (tiempoRestante <= 0)
        {
            tiempoRestante = 0;
            GanarMinijuego();
        }

        ActualizarUI();
    }

    private void ActualizarFaseDificultad()
    {
        if (spawner == null) return;

        if (tiempoRestante <= 30f) spawner.velocidadSpawn = "Rapida";
        else if (tiempoRestante <= 75f) spawner.velocidadSpawn = "Moderada";
        else spawner.velocidadSpawn = "Lenta";
    }

    public void SumarPuntos(int puntos)
    {
        if (!juegoActivo) return;

        puntosTotales += puntos;
        comboActual++;

        int bonus = 0;
        if (comboActual >= 5)
        {
            bonus = 50;
            puntosTotales += bonus; 
            comboActual = 0;
            Debug.Log("¡Combo de 5! +50 Puntos");
        }

        
        if (MainManager.Instance != null)
            MainManager.Instance.SumarPuntoTemporal(puntos + bonus);

        ActualizarUI();
    }

    public void RomperCombo()
    {
        comboActual = 0;
    }

    private void ActualizarUI()
    {
        if (textoPuntos != null) textoPuntos.text = "Puntos: " + puntosTotales;
        if (textoTiempo != null) textoTiempo.text = "Tiempo: " + Mathf.CeilToInt(tiempoRestante) + "s";
    }

    private void GanarMinijuego()
    {
        juegoActivo = false;
        if (spawner != null) spawner.gameObject.SetActive(false);

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
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
    private void DebugAR(string mensaje)
    {
        string linea = "[AR DEBUG] " + mensaje;
        Debug.Log(linea);

        if (debugTextAR != null)
        {
            debugTextAR.text += "\n" + mensaje;

            string[] lineas = debugTextAR.text.Split('\n');

            if (lineas.Length > 12)
            {
                debugTextAR.text = string.Join("\n", lineas, lineas.Length - 12, 12);
            }
        }
    }

    private void ActualizarDebugResumen()
    {
        if (debugTextAR == null) return;

        string permiso = "Editor";
#if UNITY_ANDROID && !UNITY_EDITOR
    permiso = Permission.HasUserAuthorizedPermission(Permission.Camera) ? "CONCEDIDO" : "NO CONCEDIDO";
#endif

        string loader = "NULL";
        if (XRGeneralSettings.Instance != null &&
            XRGeneralSettings.Instance.Manager != null &&
            XRGeneralSettings.Instance.Manager.activeLoader != null)
        {
            loader = XRGeneralSettings.Instance.Manager.activeLoader.name;
        }

        Camera cam = camaraARDebug != null ? camaraARDebug : Camera.main;

        string camara = cam != null && cam.enabled && cam.gameObject.activeInHierarchy ? "OK" : "ERROR";

        string background = "NO";
        if (cam != null)
        {
            ARCameraBackground bg = cam.GetComponent<ARCameraBackground>();
            background = bg != null && bg.enabled ? "OK" : "ERROR";
        }

        debugTextAR.text =
            "=== AR DEBUG ===" +
            "\nPermiso cámara: " + permiso +
            "\nLoader: " + loader +
            "\nARSession.state: " + ARSession.state +
            "\nCámara: " + camara +
            "\nAR Background: " + background +
            "\nGraphics API: " + SystemInfo.graphicsDeviceType;
    }
}