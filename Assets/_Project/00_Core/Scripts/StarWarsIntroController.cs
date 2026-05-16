using UnityEngine;
using TMPro; // Elemento obligatorio para usar TextMeshPro
using System.Collections;
using UnityEngine.InputSystem;

public class StarWarsIntroController : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] private RectTransform contenedorTexto;
    [SerializeField] private TextMeshProUGUI componenteTexto;

    [Header("Configuración del Efecto")]
    [SerializeField] private float velocidadMovimiento = 60f;
    [SerializeField] private float tiempoDuracion = 20f;

    private bool cambiandoDeEscena = false;

    void Start()
    {
        // 1. Exportamos el texto automáticamente del director de orquesta
        if (MainManager.Instance != null && componenteTexto != null)
        {
            componenteTexto.text = MainManager.Instance.ObtenerTextoHistoria();
        }

        StartCoroutine(ContadorAutoCierre());
    }

    void Update()
    {
        if (contenedorTexto != null)
        {
            contenedorTexto.anchoredPosition += Vector2.up * (velocidadMovimiento * Time.deltaTime);
        }

        bool saltarIntro = false;
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) saltarIntro = true;
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame) saltarIntro = true;

        if (saltarIntro && !cambiandoDeEscena) AvanzarAHistorial();
    }

    private IEnumerator ContadorAutoCierre()
    {
        yield return new WaitForSeconds(tiempoDuracion);
        if (!cambiandoDeEscena) AvanzarAHistorial();
    }

    private void AvanzarAHistorial()
    {
        cambiandoDeEscena = true;
        if (MainManager.Instance != null) MainManager.Instance.ContinuarHistoria();
    }
}