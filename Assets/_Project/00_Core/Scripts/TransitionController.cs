using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// =========================================================================
// >>> TRANSITIONCONTROLLER: Solo se encarga de la VISUALIZACIÓN y TIEMPO
// =========================================================================

public class TransitionController : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI textoUI; // Texto de la historia
    public Image imagenFondo; // Imagen de fondo (Sprites 2D)

    private bool cambiandoDeEscena = false;

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

            if (!MainManager.Instance.RequiereInputManual())
            {
                StartCoroutine(EsperarYPasar());
            }
            else
            {
                Debug.Log("[TransitionController] Fase agrupada manual activa. Esperando click o espacio de forma indefinida...");
            }
        }
        else
        {
            Debug.LogWarning("No hay MainManager en la escena.");
        }
    }

    void Update()
    {
        if (cambiandoDeEscena || MainManager.Instance == null) return;

        // Lectura de inputs unificada (Espacio en teclado o Click/Toque en pantalla)
        bool solicitarAvance = false;

        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
            solicitarAvance = true;

        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
            solicitarAvance = true;

        if (solicitarAvance)
        {
            AvanzarFlujo();
        }
    }

    // Controla el tiempo que el usuario tiene para leer antes de cargar el siguiente minijuego permitiendo que salga todo bien por pantalla.
    IEnumerator EsperarYPasar()
    {
        yield return new WaitForSeconds(3f);
        if (!cambiandoDeEscena) AvanzarFlujo();
    }

    private void AvanzarFlujo()
    {
        cambiandoDeEscena = true;
        MainManager.Instance.ContinuarHistoria();
    }
}