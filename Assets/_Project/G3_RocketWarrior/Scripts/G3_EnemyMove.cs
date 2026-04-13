using UnityEngine;

public class G3_Enemy : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadDescenso = 2f;  // Qué tan rápido baja el enemigo hacia el jugador
    public float amplitudZigzag = 2f;     // Qué tan ancho oscila de lado a lado
    public float frecuenciaZigzag = 2f;   // Qué tan rápido oscila de lado a lado

    private float _posXInicial; // Guardamos la X donde apareció para calcular el zigzag desde ahí

    void Start()
    {
        // Guardamos la posición X inicial al aparecer
        _posXInicial = transform.position.x;
    }

    void Update()
    {
        // ZIGZAG:
        // Mathf.Sin devuelve un valor que oscila entre -1 y 1 con el tiempo
        // Multiplicado por la amplitud controla cuánto se mueve a los lados
        // Multiplicado por la frecuencia controla qué tan rápido oscila
        float nuevaX = _posXInicial + Mathf.Sin(Time.time * frecuenciaZigzag) * amplitudZigzag;

        // DESCENSO:
        // Restamos a Y para que baje, multiplicado por deltaTime para que sea independiente de FPS
        float nuevaY = transform.position.y - velocidadDescenso * Time.deltaTime;

        // Aplicamos la nueva posición
        transform.position = new Vector3(nuevaX, nuevaY, 0);

        // LÍMITE:
        // Si el enemigo sale por abajo de la pantalla lo destruimos
        // para no acumular objetos invisibles en memoria
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
}