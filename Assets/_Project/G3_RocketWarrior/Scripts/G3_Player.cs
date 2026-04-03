using UnityEngine;
using UnityEngine.InputSystem;

// ==============================================================================
// >>> G3_PLAYER: Controlador de movimiento y detección (Minijuego 3)
// Gestiona el movimiento mediante el nuevo Input System y detecta la colisión 
// con items para reportar los puntos al G3_GameManager local.
/* ---------------------------------------------------------------------------------
    NOTAS DE MOVIMIENTO (Input System)
    --- CONFIGURACIÓN ---
    - PlayerInput: El componente debe estar en el MISMO objeto que este script.
    - Acciones: Lee la acción "Move" (Vector2) definida en vuestro Action Asset.
    
    --- INTERACCIÓN ---
    - Tags: Los objetos recolectables DEBEN tener el Tag "Item" y el IsTrigger activo
      en su Collider2D para que la recolección funcione.
    
    --- SEGURIDAD ---
    - Si el PlayerInput falla o no está asignado, el script se detiene (return) para 
      evitar que la consola se llene de errores de referencia nula.
    - El script busca automáticamente el G3_GameManager en la escena al iniciar.
    --------------------------------------------------------------------------------- */
// ==============================================================================

public class G3_Player : MonoBehaviour
{
    // AJUSTES
    public float velocidad = 5f;

    private PlayerInput playerInput;
    private G3_GameManager gameManager;

    void Start()
    {
        // 1. Buscamos el componente EN el propio jugador (eficiencia local)
        playerInput = GetComponent<PlayerInput>();

        // 2. Buscamos al manager del G3 para reportar puntos
        gameManager = Object.FindAnyObjectByType<G3_GameManager>();
    }

    void Update()
    {
        // SEGURIDAD: Evitamos errores si no hay Input configurado
        if (playerInput == null) return;

        // Leemos el valor del joystick o WASD
        Vector2 inputMovimiento = playerInput.actions["Move"].ReadValue<Vector2>();

        // Aplicamos el movimiento al transform (Plano 2D)
        Vector3 movimiento = new Vector3(inputMovimiento.x, inputMovimiento.y, 0);
        transform.position += movimiento * velocidad * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        // RECOLECCIÓN DE ITEMS:
        // Recordad poner el Tag "Item" a los objetos recolectables del G3.
        if (otro.CompareTag("Item"))
        {
            Destroy(otro.gameObject);

            // Si el manager existe, le avisamos del punto recogido
            if (gameManager != null)
            {
                gameManager.ItemRecogido();
            }
        }
    }
}