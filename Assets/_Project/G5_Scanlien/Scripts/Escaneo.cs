using UnityEngine;
using UnityEngine.UI;

public class GazeInteraction : MonoBehaviour
{
    public float maxDistance = 2.0f;    // Distancia máxima (1 metro)
    public float gazeDuration = 3.0f;   // Tiempo necesario (3 segundos)
    public float gazeDurationSec = 1.5f; // Tiempo necesario (1.5 segundos)

    [Header("UI de Progresión")]
    public Slider barraProgreso;

    private float timer = 0f;
    private GameObject currentTarget;

    void Start()
    {
        // Asegurarnos de que la barra empiece oculta
        if (barraProgreso != null) barraProgreso.gameObject.SetActive(false);
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

                    // Actualizar la barra de progreso
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

                    // Actualizar la barra de progreso
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
        if (barraProgreso != null)
        {
            // Activamos la barra si estamos mirando un objetivo
            if (!barraProgreso.gameObject.activeSelf) barraProgreso.gameObject.SetActive(true);

            if (currentTarget.CompareTag("Interactable")) barraProgreso.value = timer / gazeDuration;

            if (currentTarget.CompareTag("InteractableSec")) barraProgreso.value = timer / gazeDurationSec;
            // Calculamos el porcentaje (de 0 a 1)

        }
    }

    void Interact(GameObject target)
    {
        G5_GameManager manager = Object.FindAnyObjectByType<G5_GameManager>();
        if (manager != null)
        {
            manager.ItemRecogido();
        }

        Destroy(target);
        timer = 0f;
        currentTarget = null;
    }

    void InteractSec(GameObject target)
    {
        G5_GameManager manager = Object.FindAnyObjectByType<G5_GameManager>();
        if (manager != null)
        {
            manager.ItemSecundarioRecogido();
        }

        Destroy(target);
        timer = 0f;
        currentTarget = null;
    }

    void ResetTimer(GameObject newTarget)
    {
        timer = 0f;
        currentTarget = newTarget;

        // Ocultar y resetear la barra al dejar de mirar
        if (barraProgreso != null)
        {
            barraProgreso.value = 0f;
            barraProgreso.gameObject.SetActive(false);
        }
    }
}