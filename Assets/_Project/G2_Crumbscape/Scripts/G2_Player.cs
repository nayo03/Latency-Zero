using UnityEngine;
using UnityEngine.InputSystem;

// ==============================================================================
// >>> G2_PLAYER: Controlador de movimiento y detección (Minijuego 2)
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
    - Se ha añadido un Error Log específico por si falta el GameManager en la escena.
    --------------------------------------------------------------------------------- */
// ==============================================================================

public class G2_Player : MonoBehaviour
{
    // VARIABLES
    public float velocidad = 5f;

    private G2_GameManager gameManager;
    private PlayerInput playerInput;

    void Start()
    {
        // ASIGNACIÓN DE COMPONENTES:
        // Buscamos el Input System local (el que configuraste en el prefab/personaje).
        playerInput = GetComponent<PlayerInput>();

        // Buscamos al "cerebro" específico de este nivel (G2).
        gameManager = Object.FindAnyObjectByType<G2_GameManager>();
    }

    void Update()
    {
        // CONTROL DE SEGURIDAD: Evita errores si el Input no está listo.
        if (playerInput == null) return;

        // LECTURA DE MOVIMIENTO: Capturamos el valor del mapa de acciones "Move".
        Vector2 inputMovimiento = playerInput.actions["Move"].ReadValue<Vector2>();

        // CÁLCULO DE POSICIÓN: Movimiento en los ejes X e Y (plano 2D).
        Vector3 movimiento = new Vector3(inputMovimiento.x, inputMovimiento.y, 0);
        transform.position += movimiento * velocidad * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        // DETECCIÓN DE RECOLECTABLES:
        // Buscamos objetos con el Tag "Item".
        if (otro.CompareTag("Item"))
        {
            Destroy(otro.gameObject); // Eliminamos el objeto recogido

            // COMUNICACIÓN CON EL MANAGER LOCAL:
            if (gameManager != null)
            {
                gameManager.ItemRecogido();
            }
            else
            {
                // Aviso crítico para el equipo si el nivel no está bien montado.
                Debug.LogError("<color=red>¡ERROR!</color> El Player no encuentra el G2_GameManager en esta escena.");
            }
        }
    }
}