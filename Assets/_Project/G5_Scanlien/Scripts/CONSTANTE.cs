using UnityEngine;

public class ConstantLookMove : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float speed = 2.0f;
    public bool useGravity = true;

    [Header("Referencias")]
    public Transform cameraTransform;

    // AÑADIDO: Referencia al script de escaneo
    private GazeInteraction gazeScript;
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // AÑADIDO: Buscamos el script GazeInteraction en la cámara principal
        if (cameraTransform != null)
        {
            gazeScript = cameraTransform.GetComponent<GazeInteraction>();
        }
    }

    void Update()
    {
        if (cameraTransform == null) return;

        // AÑADIDO: Si el script de escaneo existe y está escaneando, salimos del Update
        // Esto frena en seco el avance del personaje, manteniendo la gravedad si cae.
        if (gazeScript != null && gazeScript.EstaEscaneando)
        {
            // Opcional: Mantener la gravedad activa incluso quietos si no está en el suelo
            if (useGravity && characterController != null && !characterController.isGrounded)
            {
                Vector3 fallMovement = new Vector3(0, Physics.gravity.y * Time.deltaTime, 0);
                characterController.Move(fallMovement);
            }
            return; // Detiene el código de avance que viene abajo
        }

        // --- El resto del movimiento se queda igual ---
        Vector3 forwardDirection = cameraTransform.forward;
        forwardDirection.y = 0;
        forwardDirection.Normalize();

        Vector3 movement = forwardDirection * speed * Time.deltaTime;

        if (useGravity && characterController != null && !characterController.isGrounded)
        {
            movement.y += Physics.gravity.y * Time.deltaTime;
        }

        if (characterController != null)
        {
            characterController.Move(movement);
        }
        else
        {
            transform.Translate(movement, Space.World);
        }
    }
}