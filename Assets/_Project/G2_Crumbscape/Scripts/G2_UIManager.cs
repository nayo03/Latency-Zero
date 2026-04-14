using UnityEngine;
using TMPro;

// ==============================================================================
// >>> G2_UIMANAGER: Gestión de alertas y eventos de interfaz 
// ==============================================================================
public class G2_UIManager : MonoBehaviour
{
    [Header("Elementos de Texto Principales")]
    public TextMeshProUGUI G2_ScoreText; // Texto para los puntos (Score: 000)
    public TextMeshProUGUI G2_TimerText; // Texto para el tiempo (00:00)

    [Header("Mensajes de Alerta")]
    public TextMeshProUGUI warningText; // Texto debajo para mensajes de muerte

    // ==========================================================================
    // PREPARACIÓN INICIAL (Se ejecuta al nacer el objeto)
    // ==========================================================================
    void Start()
    {
        if (warningText != null) warningText.text = ""; // Limpiamos el texto al empezar la escena
    }

    // ==========================================================================
    // ACTUALIZACIÓN DE MARCADORES (Puntos y Tiempo)
    // ==========================================================================
    public void ActualizarInterfaz(int puntos, float tiempo)
    {
        // 1. ACTUALIZAR PUNTOS
        if (G2_ScoreText != null)
        {
            G2_ScoreText.text = "Score: " + puntos; // Actualizamos el marcador de puntos
        }

        // 2. ACTUALIZAR RELOJ
        if (G2_TimerText != null)
        {
            int minutos = Mathf.FloorToInt(tiempo / 60); // Calculamos los minutos restantes
            int segundos = Mathf.FloorToInt(tiempo % 60); // Calculamos los segundos restantes

            G2_TimerText.text = string.Format("{0:00}:{1:00}", minutos, segundos); // Formateamos el texto a 00:00

            // Efecto visual: si queda poco tiempo, el texto se pone rojo
            if (tiempo <= 10f) G2_TimerText.color = Color.cyan; // Cambiamos color cuando quedan 10 sec
            else G2_TimerText.color = Color.white; // Mantenemos blanco si hay tiempo suficiente
        }
    }

    // ==========================================================================
    // TEXTOS DE MUERTE
    // ==========================================================================

    // ------------------ MUERTE POR CAÍDA ------------------
    public void MostrarMensajeMuerteCaida()
    {
        if (warningText != null)
        {
            warningText.text = "You’ve been toasted…"; // Escribimos el mensaje de quemado
        }
    }

    // ------------------ MUERTE POR CHOQUE ------------------
    public void MostrarMensajeMuerteChoque()
    {
        if (warningText != null)
        {
            warningText.text = "Don't break the crust!"; // Escribimos el mensaje de impacto
        }
    }

    // ==========================================================================
    // BOTONES DEL G2_LevelPanel
    // ==========================================================================
    // ------------------ BOTÓN START ------------------
    public void Boton_Start()
    {
        if (G2_GameManager.Instance != null)
        {
            G2_GameManager.Instance.IniciarJuego();
        }
    }

    // ------------------ BOTÓN NEXT LEVEL ------------------
    public void Boton_NextLevel()
    {
        // Buscamos el GameManager para ejecutar la transición de nivel (Nivel 1 -> 2, etc.)
        G2_GameManager gameManager = FindAnyObjectByType<G2_GameManager>(); // Localizamos el gestor en la escena
        if (gameManager != null)
        {
            gameManager.NextLevel(); // Llamamos a la función de cambio de nivel
        }
    }
}