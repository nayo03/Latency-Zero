using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class AutoWalkCardboard : MonoBehaviour
{
    [Header("Referencias")]
    public Transform vrCamera;

    [Header("Movimiento")]
    public float velocidad = 1.5f;
    public float gravedad = 9.81f;

    [Header("Detección")]
    public float distanciaDeteccion = 10f;
    public LayerMask capasDetectables = ~0;

    private CharacterController controller;
    private Vector3 velocidadVertical;
    private bool mirandoObjetoInteractuable;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (vrCamera == null && Camera.main != null)
        {
            vrCamera = Camera.main.transform;
        }
    }

    void Update()
    {
        if (vrCamera == null) return;

        G5_GameManager manager = Object.FindAnyObjectByType<G5_GameManager>();
        if (manager != null && manager.juegoTerminado) return;

        ComprobarMirada();
        AplicarMovimiento();
    }

    void ComprobarMirada()
    {
        mirandoObjetoInteractuable = false;

        Ray ray = new Ray(vrCamera.position, vrCamera.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, distanciaDeteccion, capasDetectables))
        {
            if (hit.collider.CompareTag("Interactable") || hit.collider.CompareTag("InteractableSec"))
            {
                mirandoObjetoInteractuable = true;
            }
        }
    }

    void AplicarMovimiento()
    {
        Vector3 movimientoHorizontal = Vector3.zero;

        if (!mirandoObjetoInteractuable)
        {
            Vector3 direccion = vrCamera.forward;
            direccion.y = 0f;
            direccion.Normalize();

            movimientoHorizontal = direccion * velocidad;
        }

        if (controller.isGrounded)
        {
            velocidadVertical.y = -1f;
        }
        else
        {
            velocidadVertical.y -= gravedad * Time.deltaTime;
        }

        Vector3 movimientoFinal = movimientoHorizontal + velocidadVertical;

        controller.Move(movimientoFinal * Time.deltaTime);
    }
}