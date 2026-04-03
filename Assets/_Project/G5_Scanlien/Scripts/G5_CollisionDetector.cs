using UnityEngine;

// ==============================================================================
// >>> G5_COLLISIONDETECTOR: Sensor de triggers para VR
// Script auxiliar que se coloca en el Jugador para detectar colisiones 3D.
/* ---------------------------------------------------------------------------------
    NOTAS DE INTERACCIÓN
    --- IMPORTANTE (OnTriggerEnter) ---
    - Este nivel es 3D. Los items DEBEN tener Sphere/Box Collider con 'Is Trigger'.
    - Tag: Los items deben llamarse "Item" exactamente.
    --------------------------------------------------------------------------------- */
// ==============================================================================

public class G5_CollisionDetector : MonoBehaviour
{
    private G5_GameManager manager;

    void Start()
    {
        // Buscamos al manager del nivel para avisarle de los puntos
        manager = Object.FindAnyObjectByType<G5_GameManager>();
    }

    private void OnTriggerEnter(Collider other) // DETECCIÓN 3D
    {
        if (other.CompareTag("Item"))
        {
            // Evitamos errores de doble colisión desactivando el trigger
            other.enabled = false;

            if (manager != null)
            {
                manager.ItemRecogido();
                Destroy(other.gameObject);
            }
            else
            {
                Debug.LogWarning("G5_CollisionDetector: No se encuentra el G5_GameManager.");
            }
        }
    }
}