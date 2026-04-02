using UnityEngine;
using UnityEngine.InputSystem;

// ==============================================================================
// >>> G1_PLAYER: Controlador de movimiento y detección del jugador (Minijuego 1)
// Este script gestiona el movimiento mediante el nuevo Input System y detecta
// cuando el jugador colisiona con los items para avisar al GameManager local.
/* ---------------------------------------------------------------------------------
    NOTAS DE MOVIMIENTO (Input System)
    --- CONFIGURACIÓN ---
    - PlayerInput: El componente debe estar en el MISMO objeto que este script.
    - Acciones: Usa la acción "Move" definida en vuestro Input Action Asset.
    
    --- INTERACCIÓN ---
    - Tags: Los objetos recolectables DEBEN tener el Tag "Item" y el IsTrigger activo
      en su Collider2D para que la recolección funcione.
    
    --- SEGURIDAD ---
    - Si el PlayerInput falla o no está asignado, el script se detiene para evitar 
      que la consola se llene de errores (NullReference).
    --------------------------------------------------------------------------------- */
// ==============================================================================

public class G1_Player : MonoBehaviour
{
    // VARIABLES
    public float velocidad = 5f;

    private G1_GameManager gameManager;
    private PlayerInput playerInput;

    void Start()
    {
        // ASIGNACIÓN DE COMPONENTES:
        // Buscamos el PlayerInput que configuramos en el personaje (ahora es local).
        playerInput = GetComponent<PlayerInput>();

        // Buscamos al "cerebro" del nivel para enviarle los puntos después.
        gameManager = Object.FindAnyObjectByType<G1_GameManager>();
    }

    void Update()
    {
        // CONTROL DE SEGURIDAD: Si no hay sistema de Input listo, salimos del Update.
        if (playerInput == null) return;

        // LECTURA DE ENTRADA: Capturamos el Vector2 de la acción "Move" (WASD / Joystick).
        Vector2 inputMovimiento = playerInput.actions["Move"].ReadValue<Vector2>();

        // APLICACIÓN DE MOVIMIENTO: Cálculo del desplazamiento en el plano 2D.
        Vector3 movimiento = new Vector3(inputMovimiento.x, inputMovimiento.y, 0);
        transform.position += movimiento * velocidad * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        // DETECCIÓN DE RECOLECTABLES:
        // Si chocamos con algo con el Tag "Item", se destruye y sumamos puntos.
        if (otro.CompareTag("Item"))
        {
            Destroy(otro.gameObject);

            // Comunicación con el manager local para procesar el punto.
            if (gameManager != null)
            {
                gameManager.ItemRecogido();
            }
        }
    }
}