using UnityEngine;
using TMPro;

// =========================================================================
// >>> FINALSCENECONTROLLER: Gestiona todo lo que pasa al terminar el juego
// =========================================================================

public class FinalSceneController : MonoBehaviour
{
    [Header("Referencias de UI")]
    public TextMeshProUGUI textoPuntos;
    public TextMeshProUGUI textoFraseFinal;

    void Start()
    {
        // Al arrancar, le pedimos los puntos al MainManager
        if (MainManager.Instance != null)
        {
            // Mostramos el resultado final
            int puntuacion = MainManager.Instance.puntosTotalesVisualizar;

            textoPuntos.text = "Puntuación Final: " + puntuacion;
            textoFraseFinal.text = ObtenerFraseFinal(puntuacion);
        }
    }

    string ObtenerFraseFinal(int puntos)
    {
        if (puntos < 1000)
        {
            return "¡Superviviente galáctico!\nHas llegado al final, aunque con más harina en la cara que puntos en el marcador.";
        }
        else if (puntos < 2000)
        {
            return "¡Explorador galáctico!\nNo todos los héroes llevan capa; algunos llevan rodillo y harina.";
        }
        else if (puntos < 3000)
        {
            return "¡Guardián galáctico!\nLa galaxia duerme tranquila sabiendo que tu horno sigue encendido.";
        }
        else if (puntos < 4000)
        {
            return "¡Capitán galáctico!\nHas dejado la galaxia calentita, crujiente y bastante libre de alienígenas.";
        }
        else
        {
            return "¡Leyenda galáctica!\nLos aliens han firmado la paz solo para conseguir tu receta de galletas.";
        }
    }
}