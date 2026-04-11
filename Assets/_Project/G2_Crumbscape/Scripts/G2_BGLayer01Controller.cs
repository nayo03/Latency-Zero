using UnityEngine;

// =========================================================================
// >>> MOVIMIENTO LAYER01 (los planetas decorativos de fondo)
// =========================================================================
public class G2_BGLayer01Controller : MonoBehaviour
{
    public GameObject objetoArriba; // Referenciar objeto con el sprite
    public GameObject objetoAbajo; // Referenciar objeto con el sprite
    public float velocidadArriba = 0.7f; // Velocidad del objeto de arriba
    public float velocidadAbajo = 0.7f; // Velocidad del objeto de abajo

    [Header("Ajustes Inicio")]
    public float margenInicio = 3f; // Margen extra para colocar los objetos fuera de la pantalla al inicio
    public float retrasoAbajo = 4f; // Retraso extra para separar el objeto de abajo del de arriba

    private float anchoCamara;

    void Start()
    {
        // Calcula la mitad del ancho visible de la cámara en unidades del mundo
        // Ejemplo:
        // OrtographicSize = 5 (la altura total sería 10), aspect = 1080/1920 = 0.56
        // anchoCamara = 5 * 0.56 = 2.8 (anchura total sería 5.6)
        anchoCamara = Camera.main.orthographicSize * Camera.main.aspect; 

        // Coloca el objeto de arriba fuera de la pantalla por la derecha
        objetoArriba.transform.position = new Vector3(
            anchoCamara + margenInicio, // Lo posiciona fuera de pantalla con un margen de inicio
            objetoArriba.transform.position.y, // mantiene la altura
            objetoArriba.transform.position.z); // mantiene la profundidad

        // Coloca el objeto de abajo aún más a la derecha (con retraso)
        objetoAbajo.transform.position = new Vector3(
            anchoCamara + margenInicio + retrasoAbajo, // Lo posiciona fuera de pantalla con un margen de inicio + un retraso para que tarde un poco más en salir
            objetoAbajo.transform.position.y, 
            objetoAbajo.transform.position.z);
    }

    void Update()
    {
        Mover(objetoArriba.transform, velocidadArriba); // Mueve el objeto de arriba con su velocidad
        Mover(objetoAbajo.transform, velocidadAbajo); // Mueve el objeto de abajo con su velocidad
    }

    // =========================================================================
    // >>> MOVIMIENTO DE LOS OBJETOS HACIA LA IZQUIERDA Y RECICLAJE (POOL)
    // =========================================================================
    void Mover(Transform obj, float vel)
    {
        obj.Translate(Vector3.left * vel * Time.deltaTime); // Mueve el objeto hacia la izquierda según su velocidad
                
        if (obj.position.x < -anchoCamara - 4f) // Si el objeto ha salido completamente por la izquierda (con margen extra)
        {            
            obj.position = new Vector3(anchoCamara + 4f, obj.position.y, obj.position.z); // Reaparece por la derecha con ese mismo margen
        }
    }
}
