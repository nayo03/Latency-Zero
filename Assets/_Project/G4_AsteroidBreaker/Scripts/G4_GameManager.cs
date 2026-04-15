using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.XR.Management;

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

    void Start()
    {
        if (objetoARSession != null) objetoARSession.SetActive(false);
        StartCoroutine(ReactivarAR());
    }

    IEnumerator ReactivarAR()
    {
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
        
        // Iniciamos el juego tras cargar el AR
        juegoActivo = true;
        ActualizarUI();
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
            puntosTotales += bonus; // Bonus por racha del GDD
            comboActual = 0;
            Debug.Log("¡Combo de 5! +50 Puntos");
        }

        // Mandamos SOLO los puntos recién ganados (incluyendo el bonus si lo hay) al MainManager
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
}