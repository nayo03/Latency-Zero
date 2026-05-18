using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class G1_UI_Controller : MonoBehaviour
{

    [Header("Paneles y Botones")]
    public GameObject panelVictoria;
    public GameObject botonContinuar;
    public GameObject botonSalir;

    [Header("Referencias de Textos (TextMeshPro)")]
    public TextMeshProUGUI textoPuntos;
    public TextMeshProUGUI textoPuntuacionFinal;
    public TextMeshProUGUI textoCronometroSmasher;

    [Header("Referencias de Arcos (Imágenes FillAmount)")]
    public Image perfectZone;
    public Image goodZone;

    [Header("Smasher Visuals")]
    public GameObject contenedorDial;
    public GameObject contenedorSmasher;
    public Image imagenBotonSmasher;
    public Sprite spriteBotonReposo;
    public Sprite spriteBotonPresionado;
    private Vector3 escalaOriginalBoton;

    [System.Serializable]
    public struct PiezasNivel
    {
        public string nombreNivel;
        public Sprite perfect;
        public Sprite good;
        public Sprite bad;
    }

    [Header("Configuración del Cohete")]
    public PiezasNivel[] coleccionPiezas;
    public Image[] renderizadoresCohete;

    [Header("Efectos Visuales")]
    public Image FlashRojo;
    public Image Background;
    private Vector3 posicionOriginal;

    private void Start()
    {
        if (imagenBotonSmasher != null)
        {
            escalaOriginalBoton = imagenBotonSmasher.rectTransform.localScale;
        }

        if (Background != null)
        {
            posicionOriginal = Background.rectTransform.localPosition;
        }
    }

    // ---------------------------------------------------------------------
    //                     ACCIONES DE GESTIÓN DE PROGRESIÓN
    // ---------------------------------------------------------------------

    public void SincronizarDial(float iAnglePerfect, float fAnglePerfect, float iAngleGood, float fAngleGood)
    {
        if (perfectZone != null)
        {
            perfectZone.fillAmount = (fAnglePerfect - iAnglePerfect) / 360f;
            perfectZone.rectTransform.localRotation = Quaternion.Euler(0, 0, -iAnglePerfect);
        }
        if (goodZone != null)
        {
            goodZone.fillAmount = (fAngleGood - iAngleGood) / 360f;
            goodZone.rectTransform.localRotation = Quaternion.Euler(0, 0, -iAngleGood);
        }
    }

    public void ActualizarTextoPuntos(int puntos)
    {
        if (textoPuntos != null)
        {
            textoPuntos.text = $"Puntos: {puntos}";
        }
    }

    public void ActivarInterfazSmasher(bool estado)
    {
        if (contenedorDial != null)
        {
            contenedorDial.SetActive(!estado);
        }

        if (contenedorSmasher != null)
        {
            contenedorSmasher.SetActive(estado);
        }
    }

    public void ActualizarCronometroSmasher(float time)
    {
        if (textoCronometroSmasher != null)
        {
            float t = Mathf.Max(0, time);
            textoCronometroSmasher.text = $"¡RÁPIDO! Presiona el espacio tanto como puedas! {t:F1}s ";
        }
    }

    public void FeedbackPulsacionBoton()
    {
        StopAllCoroutines(); // Reset any ongoing pulsation effect to prevent overlap
        StartCoroutine(EfectoPulsarBoton());
    }
    private IEnumerator EfectoPulsarBoton()
    {
        // Change the button sprite to the pressed state
        imagenBotonSmasher.sprite = spriteBotonPresionado;
        // Shake effect by scaling down the button slightly
        imagenBotonSmasher.rectTransform.localScale = escalaOriginalBoton * 0.9f;

        yield return new WaitForSeconds(0.1f); // Time the button stays pressed

        // 3. Return to the normal state
        imagenBotonSmasher.sprite = spriteBotonReposo;
        imagenBotonSmasher.rectTransform.localScale = escalaOriginalBoton;
    }

    // ---------------------------------------------------------------------
    //                     ACCIONES DE GESTIÓN EFECTOS VISUALES
    // ---------------------------------------------------------------------
    public void IniciarFlashFallo()
    {
        StartCoroutine(ActivarEfectoFallo());
    }

    public IEnumerator ActivarEfectoFallo()
    {
        if (FlashRojo != null)
        {
            FlashRojo.gameObject.SetActive(true);
            FlashRojo.color = new Color(1, 0, 0, 0.5f);
            yield return new WaitForSeconds(0.2f);
            FlashRojo.gameObject.SetActive(false);
        }
    }

    public void IniciarVibracion()
    {
        StartCoroutine(EfectoVibracion());
    }

    private IEnumerator EfectoVibracion()
    {
        if (Background != null)
        {
            float elapsed = 0f;
            float duration = 0.2f;
            float magnitude = 10f;
            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;
                Background.rectTransform.localPosition = posicionOriginal + new Vector3(x, y, 0);
                elapsed += Time.deltaTime;
                yield return null;
            }
            Background.rectTransform.localPosition = posicionOriginal;
        }
    }

    // ---------------------------------------------------------------------
    //                     ACCIONES DE GESTIÓN PANTALLA FINAL
    // ---------------------------------------------------------------------

    public void MostrarPantallaFinal(int puntos, bool esModoHistoria)
    {
        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
        }
        if (textoPuntuacionFinal != null)
        {
            textoPuntuacionFinal.text = $"Puntuación Final: {puntos}";
        }
        if (botonContinuar != null)
        {
            botonContinuar.SetActive(esModoHistoria);
        }
        if (botonSalir != null)
        {
            botonSalir.SetActive(true) ;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // ---------------------------------------------------------------------
    //                     ACCIONES DE GESTIÓN DEL COHETE
    // ---------------------------------------------------------------------

    public void RenderizarPiezaCohete(int indiceNivel, int calidad)
    {
        if (indiceNivel < 0 || indiceNivel >= renderizadoresCohete.Length) return;

        Image imagenActual = renderizadoresCohete[indiceNivel];
        PiezasNivel spritesActuales = coleccionPiezas[indiceNivel];

        Sprite spriteAElegir = calidad == 2 ? spritesActuales.perfect :
                               calidad == 1 ? spritesActuales.good :
                                              spritesActuales.bad;

        if (spriteAElegir != null)
        {
            imagenActual.sprite = spriteAElegir;
            Debug.Log($"Intentando activar la imagen en el objeto: {imagenActual.gameObject.name}", imagenActual.gameObject);
            imagenActual.enabled = true;
            imagenActual.rectTransform.localScale = new Vector3(1.7f, 1.7f, 1.7f);

            imagenActual.SetNativeSize();

        }

    }

}
