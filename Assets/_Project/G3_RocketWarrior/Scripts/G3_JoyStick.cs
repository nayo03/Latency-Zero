using UnityEngine;
using UnityEngine.EventSystems;

// El joystick implementa estas interfaces para detectar cuando el dedo
// toca la pantalla, se mueve y se levanta
public class G3_Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Configuración")]
    public RectTransform background;  // El círculo grande de fondo
    public RectTransform handle;      // El círculo pequeño que se mueve
    public float radioMaximo = 75f;   // Hasta dónde puede moverse el handle

    // El valor del joystick entre -1 y 1 en cada eje
    // Lo lee G3_Player para mover la nave
    public Vector2 InputJoystick { get; private set; }

    private RectTransform _canvasRect;

    void Start()
    {
        // Buscamos el RectTransform del Canvas para calcular posiciones
        _canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    // Se llama cuando el dedo toca el joystick
    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    // Se llama mientras el dedo se mueve por la pantalla
    public void OnDrag(PointerEventData eventData)
    {
        // Convertimos la posición del dedo a coordenadas locales del background
        Vector2 posicion;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background, eventData.position, eventData.pressEventCamera, out posicion);

        // Limitamos el movimiento al radio máximo
        posicion = Vector2.ClampMagnitude(posicion, radioMaximo);

        // Movemos el handle a esa posición
        handle.localPosition = posicion;

        // Calculamos el input normalizado entre -1 y 1
        InputJoystick = posicion / radioMaximo;
    }

    // Se llama cuando el dedo se levanta de la pantalla
    public void OnPointerUp(PointerEventData eventData)
    {
        // Volvemos el handle al centro y reseteamos el input
        handle.localPosition = Vector2.zero;
        InputJoystick = Vector2.zero;
    }
}