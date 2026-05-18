using UnityEngine;
using System.Collections;

public class G3_Boss : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadMovimiento = 2f;    // Velocidad de desplazamiento lateral
    public float limiteIzquierda = -4f;       // Límite izquierdo de la pantalla
    public float limiteDerecha = 4f;          // Límite derecho de la pantalla

    [Header("Vida")]
    public int vidaMaxima = 20;               // Golpes necesarios para matar al boss
    private int _vidaActual;

    [Header("Disparo Circular")]
    public GameObject prefabBala;             // Bala del boss
    public float tiempoEntreRafagas = 3f;     // Segundos entre cada ráfaga circular
    public int balasEnCirculo = 8;            // Cantidad de balas por ráfaga circular

    [Header("Rayo Láser")]
    public GameObject prefabLaser;            // Prefab del rayo láser
    public float tiempoEntreRayos = 5f;       // Segundos entre cada rayo láser
    
    [Header("Puntos")]
    public int puntosAlMorir = 175;
    
    private SpriteRenderer _spriteRenderer; // Referencia al sprite del boss
    private Color _colorOriginal;           // Color original para restaurar tras el tinte

    private float _direccion = 1f;            // 1 = derecha, -1 = izquierda
    private float _timerRafaga;
    private float _timerRayo;

    void Start()
    {
        _vidaActual = vidaMaxima;
        _timerRafaga = tiempoEntreRafagas;
        _timerRayo = tiempoEntreRayos;
        
        // Guardamos referencia al sprite y su color original
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _colorOriginal = _spriteRenderer.color;
    }

    void Update()
    {
        // MOVIMIENTO LATERAL
        // El boss se mueve de izquierda a derecha rebotando en los límites
        transform.position += Vector3.right * _direccion * velocidadMovimiento * Time.deltaTime;

        if (transform.position.x >= limiteDerecha)
            _direccion = -1f; // Cambia dirección hacia la izquierda
        if (transform.position.x <= limiteIzquierda)
            _direccion = 1f;  // Cambia dirección hacia la derecha

        // DISPARO CIRCULAR
        _timerRafaga -= Time.deltaTime;
        if (_timerRafaga <= 0f)
        {
            DisparoCircular();
            _timerRafaga = tiempoEntreRafagas;
        }

        // RAYO LÁSER
        _timerRayo -= Time.deltaTime;
        if (_timerRayo <= 0f)
        {
            StartCoroutine(ActivarRayo());
            _timerRayo = tiempoEntreRayos;
        }
    }

    private void DisparoCircular()
    {
        // Solo disparamos hacia abajo (semicírculo inferior)
        float anguloPorBala = 180f / (balasEnCirculo - 1); // Distribuimos en semicírculo

        for (int i = 0; i < balasEnCirculo; i++)
        {
            // Ángulos entre 180 y 360 (semicírculo hacia abajo)
            float angulo = 180f + i * anguloPorBala;
            float rad = angulo * Mathf.Deg2Rad;
            Vector2 direccion = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            if (prefabBala != null)
            {
                GameObject bala = Instantiate(prefabBala, transform.position, Quaternion.identity);
                G3_BalaBoss balScript = bala.GetComponent<G3_BalaBoss>();
                if (balScript != null) balScript.SetDireccion(direccion);
            }
        }
    }

    private IEnumerator ActivarRayo()
    {
        // Instanciamos el rayo en la posición actual del boss
        if (prefabLaser != null)
        {
            GameObject rayo = Instantiate(prefabLaser, transform.position, Quaternion.identity);
            // El rayo dura 2 segundos y luego se destruye
            Destroy(rayo, 2f);
        }
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        // Recibe daño de las balas del jugador
        if (otro.CompareTag("BalaJugador"))
        {
            Destroy(otro.gameObject);
            _vidaActual--;
            
            // Activamos el efecto de tinte rojo al recibir daño
            StartCoroutine(TinteRojo());

            if (_vidaActual <= 0)
            {
                // Boss derrotado — avisamos al GameManager
                if (G3_GameManager.Instance != null)
                    G3_GameManager.Instance.SumarPuntos(puntosAlMorir);
                
                G3_GameManager.Instance.GanarPartida();
                Destroy(gameObject);
            }
        }
    }
    
    private IEnumerator TinteRojo()
    {
        // Cambiamos el color del sprite a rojo
        _spriteRenderer.color = Color.red;

        // Esperamos un breve momento
        yield return new WaitForSeconds(0.1f);

        // Restauramos el color original
        _spriteRenderer.color = _colorOriginal;
    }
}