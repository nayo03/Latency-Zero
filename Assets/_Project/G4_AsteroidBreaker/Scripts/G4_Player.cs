using UnityEngine;
using UnityEngine.InputSystem;

// ==============================================================================
// >>> G4_PLAYER: Controlador de movimiento y detección (Minijuego 4 - AR)
// Gestiona el movimiento mediante el Input System y detecta la recolección 3D.
/* ---------------------------------------------------------------------------------
    NOTAS BÁSICAS (COMUNES A TODOS LOS PLAYERS)
    --- MOVIMIENTO (Input System) ---
    - PlayerInput: El componente debe estar en el MISMO objeto que este script.
    - Acciones: Usa la acción "Move" (Vector2) de vuestro Input Action Asset.

    --- INTERACCIÓN Y PUNTOS ---
    - Tags: Los objetos recolectables DEBEN tener el Tag "Item".
    - Comunicación: El Player avisa al GameManager local, y este al MainManager.

    --- SEGURIDAD ---
    - Si el PlayerInput o el GameManager fallan, el script lanza un error en consola
      en lugar de "petar" el juego (NullReference Protection).

    ---------------------------------------------------------------------------------
    NOTAS ESPECÍFICAS DEL NIVEL 4 (AR / 3D)
    - FÍSICAS: ¡IMPORTANTE! Este nivel es 3D. Usa 'OnTriggerEnter' en el codigo
    - COLLIDERS: Los items deben tener BoxCollider/SphereCollider con 'Is Trigger'.
    --------------------------------------------------------------------------------- */
// ==============================================================================
public class G4_Player : MonoBehaviour
{
    public float velocidad = 0.5f;
    private PlayerInput playerInput;
    private G4_GameManager gameManager;

    void Start()
    {
        // Buscamos componentes de forma local
        playerInput = GetComponent<PlayerInput>();

        // Buscamos al manager con el nuevo nombre G4_GameManager
        gameManager = Object.FindAnyObjectByType<G4_GameManager>();
    }

    void Update()
    {
        if (playerInput == null) return;

        Vector2 inputMovimiento = playerInput.actions["Move"].ReadValue<Vector2>();

        // En AR movemos en el eje X e Y sobre la vista de la cámara
        Vector3 movimiento = new Vector3(inputMovimiento.x, inputMovimiento.y, 0);
        transform.position += movimiento * velocidad * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other) // DETECCIÓN 3D 
    {
        if (other.CompareTag("Item"))
        {
            // Seguridad: desactivamos el choque antes de borrar
            other.enabled = false;

            if (gameManager != null)
            {
                gameManager.ItemRecogido();
                Destroy(other.gameObject);
            }
            else
            {
                Debug.LogWarning("G4_Player: No se encuentra el G4_GameManager en la escena.");
            }
        }
    }
}