using UnityEngine;

public class G3_Enemy : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadDescenso = 2f;  // Velocidad a la que baja el enemigo
    public float amplitudZigzag = 2f;     // Amplitud del movimiento zigzag
    public float frecuenciaZigzag = 2f;   // Frecuencia del movimiento zigzag
    public float limiteYAtaque = -3f;     // Altura a partir de la cual el enemigo
                                          // deja el zigzag y va directo al jugador

    [Header("Disparo")]
    public GameObject prefabBala;          // Prefab de la bala que dispara el enemigo
    public float tiempoEntreDisparos = 2f; // Segundos entre cada disparo

    private float _posXInicial;    // Posición X inicial para calcular el zigzag
    private float _timerDisparo;   // Contador regresivo para el disparo
    private bool _modoAtaque;      // True cuando el enemigo va directo al jugador
    private Transform _jugador;    // Referencia al transform del jugador

    void Start()
    {
        // Guardamos la X inicial para que el zigzag oscile desde ahí
        _posXInicial = transform.position.x;

        // Empezamos el timer al máximo para no disparar nada más aparecer
        _timerDisparo = tiempoEntreDisparos;

        // Buscamos al jugador en la escena por su tag
        GameObject jugadorObj = GameObject.FindGameObjectWithTag("Player");
        if (jugadorObj != null) _jugador = jugadorObj.transform;
    }

    void Update()
    {
        if (_modoAtaque)
        {
            // MODO ATAQUE: el enemigo va directo al jugador al doble de velocidad
            // Se activa cuando llega demasiado abajo sin ser destruido
            if (_jugador != null)
            {
                Vector3 direccion = (_jugador.position - transform.position).normalized;
                transform.position += direccion * velocidadDescenso * 2f * Time.deltaTime;
            }
        }
        else
        {
            // MODO NORMAL: movimiento zigzag hacia abajo
            // Mathf.Sin genera oscilación entre -1 y 1 multiplicada por la amplitud
            float nuevaX = _posXInicial + Mathf.Sin(Time.time * frecuenciaZigzag) * amplitudZigzag;
            float nuevaY = transform.position.y - velocidadDescenso * Time.deltaTime;
            transform.position = new Vector3(nuevaX, nuevaY, 0);

            // Si llega al límite inferior activamos el modo ataque
            if (transform.position.y < limiteYAtaque)
            {
                _modoAtaque = true;
            }
        }

        // DISPARO AUTOMÁTICO
        _timerDisparo -= Time.deltaTime;
        if (_timerDisparo <= 0f)
        {
            Disparar();
            _timerDisparo = tiempoEntreDisparos;
        }
    }

    private void Disparar()
    {
        if (prefabBala != null)
        {
            Instantiate(prefabBala, transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        // IMPACTO DE BALA DEL JUGADOR:
        // Destruimos la bala, sumamos puntos y destruimos el enemigo
        if (otro.CompareTag("BalaJugador"))
        {
            Destroy(otro.gameObject);

            if (G3_GameManager.Instance != null)
                G3_GameManager.Instance.SumarPuntos(30); // 30 pts por enemigo según GDD

            // Avisamos al Spawner que este enemigo ha muerto
            G3_Spawner spawner = Object.FindAnyObjectByType<G3_Spawner>();
            if (spawner != null) spawner.EnemigoDerrotado();

            Destroy(gameObject);
        }

        // COLISIÓN CON EL JUGADOR:
        // El enemigo muere al chocar con el jugador (el daño lo gestiona G3_Player)
        if (otro.CompareTag("Player"))
        {
            G3_Spawner spawner = Object.FindAnyObjectByType<G3_Spawner>();
            if (spawner != null) spawner.EnemigoDerrotado();

            Destroy(gameObject);
        }
    }
}