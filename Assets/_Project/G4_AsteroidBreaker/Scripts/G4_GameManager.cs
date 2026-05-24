using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.XR.Management;
using UnityEngine.Android;

public class G4_GameManager : MonoBehaviour
{
    [Header("Configuración AR")]
    public GameObject objetoARSession;

    [Header("UI del Juego")]
    public TextMeshProUGUI textoPuntos;
    public TextMeshProUGUI textoTiempo;
    public GameObject panelVictoria;
    public GameObject botonContinuar;
    public GameObject botonSalir;

    [Header("Referencias")]
    public G4_AsteroidSpawner spawner;

    private float tiempoRestante = 120f;
    private int puntosTotales = 0;
    private int comboActual = 0;
    private bool juegoActivo = false;

    void Start()
    {
        if (objetoARSession != null)
        {
            objetoARSession.SetActive(false);
        }

        StartCoroutine(ReactivarAR());
    }

    IEnumerator ReactivarAR()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
    {
        Permission.RequestUserPermission(Permission.Camera);
        yield return new WaitForSeconds(1.5f);
    }

    if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
    {
        Debug.LogError("G4 AR - Permiso de cámara denegado");
        yield break;
    }
#endif

        if (XRGeneralSettings.Instance == null)
        {
            Debug.LogError("XRGeneralSettings.Instance es NULL");
            yield break;
        }

        if (XRGeneralSettings.Instance.Manager == null)
        {
            Debug.LogError("XR Manager es NULL");
            yield break;
        }

        var manager = XRGeneralSettings.Instance.Manager;

        if (manager.isInitializationComplete)
        {
            manager.StopSubsystems();
            manager.DeinitializeLoader();
        }

        yield return manager.InitializeLoader();

        if (manager.activeLoader == null)
        {
            Debug.LogError("No se pudo inicializar ningún loader XR.");
            yield break;
        }

        manager.StartSubsystems();

        yield return new WaitForSeconds(0.5f);

        if (objetoARSession != null)
        {
            objetoARSession.SetActive(true);
        }

        ApagarCamarasIntrusas();

        juegoActivo = true;
        ActualizarUI();
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

        if (spawner != null)
            spawner.gameObject.SetActive(false);

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (MainManager.Instance != null)
            {
                bool modoHistoria = MainManager.Instance.modoHistoriaActivo;

                if (botonContinuar != null)
                    botonContinuar.SetActive(modoHistoria);

                if (botonSalir != null)
                    botonSalir.SetActive(true);
            }
        }
    }
}