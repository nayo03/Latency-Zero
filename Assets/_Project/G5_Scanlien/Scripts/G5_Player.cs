using UnityEngine;
using UnityEngine.InputSystem;

// ==============================================================================
// >>> G5_PLAYER: Controlador de movimiento (Minijuego 5 - VR)
// Gestiona el movimiento de la bola/jugador usando OnMove (Input System).
/* ---------------------------------------------------------------------------------
    NOTAS DE MOVIMIENTO
    --- CONFIGURACIÓN ---
    - X = Movimiento lateral (Izquierda/Derecha).
    - Z = Movimiento longitudinal (Adelante/Atrás) -> mapeado desde direccionInput.y.
    - Este script mueve el objeto directamente por Transform (ideal para VR suave).
    --------------------------------------------------------------------------------- */
// ==============================================================================

public class G5_Player : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float velocidad = 5f;

    private Vector2 direccionInput;

    // Recibe el mensaje "Move" desde el componente PlayerInput (SendMessage)
    private void OnMove(InputValue value)
    {
        direccionInput = value.Get<Vector2>();
    }

    void Update()
    {
        // En un entorno 3D (VR), el eje Y del joystick mueve el eje Z del mundo (adelante)
        Vector3 movimiento = new Vector3(direccionInput.x, 0, direccionInput.y);

        // Movimiento relativo al tiempo real
        transform.position += movimiento * velocidad * Time.deltaTime;
    }
}