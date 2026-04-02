using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

// =========================================================================
// >>> TRANSITIONCONTROLLER: Solo se encarga de la VISUALIZACIÓN y TIEMPO
// =========================================================================

public class TransitionController : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI textoUI; // Texto de la historia
    public Image imagenFondo; // Imagen de fondo (Sprites 2D)

    void Start()
    {
        // Verificamos que el MainManager (Singleton) esté vivo para extraer los datos
        if (MainManager.Instance != null)
        {
            // Extraemos el texto correspondiente al índice de la historia actual
            textoUI.text = MainManager.Instance.ObtenerTextoHistoria();

            // Extraemos y aplicamos el Sprite de fondo (si existe en la BD)
            Sprite fondoNuevo = MainManager.Instance.ObtenerFondoActual();
            if (fondoNuevo != null) imagenFondo.sprite = fondoNuevo;

            // Iniciamos la cuenta atrás para saltar automáticamente al siguiente nivel
            StartCoroutine(EsperarYPasar());
        }
        else
        {
            Debug.LogWarning("No hay MainManager en la escena.");
        }
    }

    // Controla el tiempo que el usuario tiene para leer antes de cargar el siguiente minijuego permitiendo que salga todo bien por pantalla.
    IEnumerator EsperarYPasar()
    {
        // Tiempo de lectura ajustable 
        yield return new WaitForSeconds(3f); 

        if (MainManager.Instance != null) // Tiempo agotado, solicitando siguiente escena al MainManager
        {
            MainManager.Instance.ContinuarHistoria(); // Llama a la lógica central para decidir cuál es el siguiente nivel 
        }
    }
}