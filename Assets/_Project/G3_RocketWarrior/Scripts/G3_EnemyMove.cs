using UnityEngine;

public class G3_Enemy : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadDescenso = 2f;  // Qué tan rápido baja el enemigo hacia el jugador
    public float amplitudZigzag = 2f;     // Qué tan ancho oscila de lado a lado
    public float frecuenciaZigzag = 2f;   // Qué tan rápido oscila de lado a lado

    [Header("Disparo")]
    public GameObject prefabBala;         // Arrastra aquí el prefab BalaEnemigo
    public float tiempoEntreDisparos = 2f; // Segundos entre cada disparo

    private float _posXInicial;           // Posición X inicial para calcular el zigzag
    private float _timerDisparo;          // Contador regresivo para el disparo

    void Start()
    {
        // Guardamos la X inicial para que el zigzag oscile desde ahí
        _posXInicial = transform.position.x;

        // Empezamos el timer al máximo para que no dispare nada más aparecer
        _timerDisparo = tiempoEntreDisparos;
    }

    void Update()
    {
        // ZIGZAG HACIA ABAJO
        float nuevaX = _posXInicial + Mathf.Sin(Time.time * frecuenciaZigzag) * amplitudZigzag;
        float nuevaY = transform.position.y - velocidadDescenso * Time.deltaTime;
        transform.position = new Vector3(nuevaX, nuevaY, 0);

        // DISPARO AUTOMÁTICO
        _timerDisparo -= Time.deltaTime;
        if (_timerDisparo <= 0f)
        {
            Disparar();
            _timerDisparo = tiempoEntreDisparos;
        }

        // Si sale por abajo de la pantalla lo destruimos
        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }

    private void Disparar()
    {
        // Solo disparamos si el prefab está asignado en el Inspector
        if (prefabBala != null)
        {
            Instantiate(prefabBala, transform.position, Quaternion.identity);
        }
    }
    
    // Cuando la bala del jugador toca al enemigo
    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("BalaJugador"))
        {
            // Destruimos la bala
            Destroy(otro.gameObject);
        
            // Sumamos puntos al GameManager
            if (G3_GameManager.Instance != null)
            {
                G3_GameManager.Instance.SumarPuntos(30); // 30 puntos por enemigo según el GDD
            }
        
            // Destruimos el enemigo
            Destroy(gameObject);
        }
    }
}