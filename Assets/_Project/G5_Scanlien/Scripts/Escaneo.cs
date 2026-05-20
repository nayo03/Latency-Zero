using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GazeInteraction : MonoBehaviour
{
    public float maxDistance = 2.0f;
    public float gazeDuration = 3.0f;
    public float gazeDurationSec = 1.5f;

    [Header("UI de Progresión (Círculo)")]
    public Image circuloProgresoAzul;
    public TextMeshProUGUI textoPorcentaje;
    public GameObject contenedorUI;

    [Header("Efectos de Escaneo")]
    public ParticleSystem particulasExito;

    [Header("Efectos de Audio")]
    [SerializeField] private AudioSource audioSourceBucle; 
    [SerializeField] private AudioSource audioSourceExito;
    [SerializeField] private AudioClip sonidoEscaneando; 
    [SerializeField] private AudioClip sonidoExito;       

    private float timer = 0f;
    private GameObject currentTarget;
    public bool EstaEscaneando => currentTarget != null;
    void Start()
    {
        if (contenedorUI != null) contenedorUI.SetActive(false);

        if (audioSourceBucle != null && sonidoEscaneando != null)
        {
            audioSourceBucle.clip = sonidoEscaneando;
        }
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.collider.CompareTag("Interactable"))
            {
                if (currentTarget == hit.collider.gameObject)
                {
                    timer += Time.deltaTime;
                    ActualizarBarra();

                    if (timer >= gazeDuration)
                    {
                        Interact(hit.collider.gameObject);
                    }
                }
                else
                {
                    ResetTimer(hit.collider.gameObject);
                }
            }
            else if (hit.collider.CompareTag("InteractableSec"))
            {
                if (currentTarget == hit.collider.gameObject)
                {
                    timer += Time.deltaTime;
                    ActualizarBarra();

                    if (timer >= gazeDurationSec)
                    {
                        InteractSec(hit.collider.gameObject);
                    }
                }
                else
                {
                    ResetTimer(hit.collider.gameObject);
                }
            }
            else { ResetTimer(null); }
        }
        else
        {
            ResetTimer(null);
        }
    }

    void ActualizarBarra()
    {
        if (currentTarget != null)
        {
            if (contenedorUI != null && !contenedorUI.activeSelf)
                contenedorUI.SetActive(true);

            if (audioSourceBucle != null && !audioSourceBucle.isPlaying)
            {
                audioSourceBucle.Play();
            }

            float progresoActual = 0f;

            if (currentTarget.CompareTag("Interactable"))
                progresoActual = timer / gazeDuration;
            else if (currentTarget.CompareTag("InteractableSec"))
                progresoActual = timer / gazeDurationSec;

            progresoActual = Mathf.Clamp01(progresoActual);

            if (circuloProgresoAzul != null) circuloProgresoAzul.fillAmount = progresoActual;

            if (textoPorcentaje != null)
            {
                int porcentajeEntero = Mathf.RoundToInt(progresoActual * 100f);
                textoPorcentaje.text = porcentajeEntero + "%";
            }
        }
    }

    void Interact(GameObject target)
    {
        G5_GameManager manager = Object.FindAnyObjectByType<G5_GameManager>();
        if (manager != null) manager.ItemRecogido();

        ReproducirSonidoExito();

        DesvanecerObjetoConHumo(target);
        timer = 0f;
        currentTarget = null;
    }

    void InteractSec(GameObject target)
    {
        G5_GameManager manager = Object.FindAnyObjectByType<G5_GameManager>();
        if (manager != null) manager.ItemSecundarioRecogido();

        ReproducirSonidoExito();

        DesvanecerObjetoConHumo(target);
        timer = 0f;
        currentTarget = null;
    }

    void ReproducirSonidoExito()
    {
        if (audioSourceExito != null && sonidoExito != null)
        {
            audioSourceExito.PlayOneShot(sonidoExito);
        }
    }

    void DesvanecerObjetoConHumo(GameObject target)
    {
        if (particulasExito != null)
        {
            ParticleSystem fx = Instantiate(particulasExito, target.transform.position, Quaternion.identity);
            fx.Play();
            Destroy(fx.gameObject, 3f);
        }

        ParticleSystem humo = target.GetComponentInChildren<ParticleSystem>();
        if (humo != null)
        {
            humo.Stop();
            humo.transform.SetParent(null);
            Destroy(humo.gameObject, 5f);
        }

        if (contenedorUI != null) contenedorUI.SetActive(false);

        if (audioSourceBucle != null) audioSourceBucle.Stop();

        Destroy(target);
    }

    void ResetTimer(GameObject newTarget)
    {
        timer = 0f;
        currentTarget = newTarget;

        if (circuloProgresoAzul != null) circuloProgresoAzul.fillAmount = 0f;
        if (textoPorcentaje != null) textoPorcentaje.text = "0%";
        if (contenedorUI != null) contenedorUI.SetActive(false);

        if (audioSourceBucle != null && audioSourceBucle.isPlaying)
        {
            audioSourceBucle.Stop();
        }
    }
}