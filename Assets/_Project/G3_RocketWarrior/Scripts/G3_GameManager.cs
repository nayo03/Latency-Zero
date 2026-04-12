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
    public int itemsParaGanar = 2;
    public TextMeshProUGUI textoUI;
    public GameObject panelVictoria;

    [Header("Botones de Victoria")]
    public GameObject botonContinuar;
    public GameObject botonSalir;

    private int itemsActuales = 0;
    private int puntosTotales = 0;

    public void ItemRecogido()
    {
        itemsActuales++;
        puntosTotales += 5;

        if (textoUI != null)
        {
            textoUI.text = "Puntos: " + puntosTotales;
        }

        if (MainManager.Instance != null)
        {
            MainManager.Instance.SumarPuntoTemporal(5);
        }

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
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (MainManager.Instance != null)
            {
                bool historia = MainManager.Instance.modoHistoriaActivo;
                botonContinuar.SetActive(historia);
                botonSalir.SetActive(true);
            }
        }
    }
}