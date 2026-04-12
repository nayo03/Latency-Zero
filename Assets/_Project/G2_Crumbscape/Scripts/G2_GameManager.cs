using UnityEngine;
using TMPro;

// ==============================================================================
// >>> G2_GAMEMANAGER: Controlador específico del Minijuego 2
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

public class G2_GameManager : MonoBehaviour
{
    // VARIABLES DE CONFIGURACIÓN
    [Header("Ajustes del Nivel")]
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
        // Incremento de contadores locales
        itemsActuales++;
        puntosTotales += 5;

        // Actualización del marcador visual del minijuego
        if (textoUI != null)
        {
            textoUI.text = "Score: " + puntosTotales;
        }

        // COMUNICACIÓN CON EL CORE:
        // Se añade al registro temporal del MainManager (no se guarda en BD hasta ganar).
        if (MainManager.Instance != null)
        {
            MainManager.Instance.SumarPuntoTemporal(5);
        }

        // VERIFICACIÓN: ¿Se han recogido todos los objetos necesarios?
        if (itemsActuales >= itemsParaGanar)
        {
            GanarMinijuego();
        }
    }

    // =========================================================================
    // >>> GANAR MINIJUEGO
    // =========================================================================
    private void GanarMinijuego()
    {
        if (panelVictoria != null)
        {
            // ACTIVACIÓN DE INTERFAZ:
            panelVictoria.SetActive(true);

            // GESTIÓN DEL CURSOR: Se libera para que el jugador pueda clicar botones.
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // -------- PAUSAMOS EL JUEGO -----------
            Time.timeScale = 0f;

            // LÓGICA DE NAVEGACIÓN:
            // El MainManager nos dice si estamos en "Historia" para mostrar el botón adecuado.
            if (MainManager.Instance != null)
            {
                if (MainManager.Instance.modoHistoriaActivo)
                {
                    // Modo Historia: Puede seguir al siguiente nivel o salir al inicio.
                    botonContinuar.SetActive(true);
                    botonSalir.SetActive(true);
                }
                else
                {
                    // Modo Libre: Solo puede salir (vuelve al selector de minijuegos).
                    botonContinuar.SetActive(false);
                    botonSalir.SetActive(true);
                }
            }
        }
    }
    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}