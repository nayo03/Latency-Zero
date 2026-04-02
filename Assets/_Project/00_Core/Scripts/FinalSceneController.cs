using UnityEngine;
using TMPro;

// =========================================================================
// >>> FINALSCENECONTROLLER: Gestiona todo lo que pasa al terminar el juego
// =========================================================================

public class FinalSceneController : MonoBehaviour
{
    [Header("Referencias de UI")]
    public TextMeshProUGUI textoPuntos;

    void Start()
    {
        // Al arrancar, le pedimos los puntos al MainManager
        if (MainManager.Instance != null)
        {
            // Mostramos el resultado final
            textoPuntos.text = "PUNTUACIÓN TOTAL: " + MainManager.Instance.puntosTotalesVisualizar;
        }

        // Aquí se podrían añadir más cosas de "Final de Juego":
        // - Lanzar fuegos artificiales.
        // - Reproducir una música de victoria.
        // - Activar un trofeo si han sacado más de X puntos.
    }
}