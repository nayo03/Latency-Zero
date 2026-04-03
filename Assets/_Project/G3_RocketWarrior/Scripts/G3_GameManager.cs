using UnityEngine;
using TMPro;

// ==============================================================================
// >>> G3_GAMEMANAGER: Controlador específico del Minijuego 3
// Este es el "cerebro" local de vuestro nivel. Se encarga de contar los items
// y avisar al MainManager (el motor global) cuando ganamos.
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