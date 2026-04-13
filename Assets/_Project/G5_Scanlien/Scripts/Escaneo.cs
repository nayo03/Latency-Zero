using UnityEngine;

public class GazeInteraction : MonoBehaviour
{
    public float maxDistance = 1.0f;    // Distancia máxima (1 metro)
    public float gazeDuration = 3.0f;   // Tiempo necesario (3 segundos)

    private float timer = 0f;
    private GameObject currentTarget;
    private int score = 0;              // Tu contador

    void Update()
    {
        // Lanzamos un rayo desde el centro de la cámara hacia adelante
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            // Verificamos si el objeto tiene el tag "Interactable" (o el que prefieras)
            if (hit.collider.CompareTag("Interactable"))
            {
                // Si es el mismo objeto que el frame anterior, aumentamos el tiempo
                if (currentTarget == hit.collider.gameObject)
                {
                    timer += Time.deltaTime;

                    if (timer >= gazeDuration)
                    {
                        Interact(hit.collider.gameObject);
                    }
                }
                else
                {
                    // Si miramos a un objeto interactuable nuevo, reiniciamos
                    ResetTimer(hit.collider.gameObject);
                }
            }
            else
            {
                // Si el rayo toca algo que NO es interactuable
                ResetTimer(null);
            }
        }
        else
        {
            // Si el rayo no toca nada en el rango de 1 metro
            ResetTimer(null);
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

    void ResetTimer(GameObject newTarget)
    {
        timer = 0f;
        currentTarget = newTarget;
    }
}