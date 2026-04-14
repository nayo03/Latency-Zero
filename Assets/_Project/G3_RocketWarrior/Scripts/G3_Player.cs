using UnityEngine;
using System.Collections;

public class G3_Player : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;

    [Header("Disparo")]
    public GameObject prefabBala;             // Arrastra aquí el prefab BalaJugador
    public float tiempoEntreDisparos = 0.3f;  // Segundos entre cada disparo automático
    public Transform puntoDisparo;            // Punto desde donde sale la bala

    [Header("Vidas")]
    public int vidas = 3;                     // Vidas iniciales del jugador

    private Vector2 _minBounds;               // Límite inferior-izquierdo de la pantalla
    private Vector2 _maxBounds;               // Límite superior-derecho de la pantalla
    private float _timerDisparo;              // Contador regresivo para el disparo
    private bool _invencible = false;         // Evita recibir daño varias veces seguidas
    private float _tiempoInvencible = 1.5f;   // Segundos de invencibilidad tras recibir daño

    void Start()
    {
        // Calculamos los límites de la pantalla
        Camera cam = Camera.main;
        float margen = 0.3f;
        _minBounds = cam.ViewportToWorldPoint(new Vector2(0, 0));
        _maxBounds = cam.ViewportToWorldPoint(new Vector2(1, 1));
        _minBounds += Vector2.one * margen;
        _maxBounds -= Vector2.one * margen;

        // Empezamos el timer a 0 para que dispare nada más empezar
        _timerDisparo = 0f;
    }

    void Update()
    {
        // MOVIMIENTO
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 mov = new Vector3(x, y, 0).normalized;
        transform.position += mov * velocidad * Time.deltaTime;

        // Limitamos a los bordes de la pantalla
        float clampX = Mathf.Clamp(transform.position.x, _minBounds.x, _maxBounds.x);
        float clampY = Mathf.Clamp(transform.position.y, _minBounds.y, _maxBounds.y);
        transform.position = new Vector3(clampX, clampY, 0);

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
            // Instanciamos la bala en el punto de disparo o en la posición del jugador
            Vector3 pos = puntoDisparo != null ? puntoDisparo.position : transform.position;
            Instantiate(prefabBala, pos, Quaternion.identity);
        }
    }

    public void RecibirDaño()
    {
        // Si está en periodo de invencibilidad no recibe daño
        if (_invencible) return;

        vidas--;
        G3_GameManager.Instance.ActualizarVidas(vidas);
        Debug.Log("Vidas restantes: " + vidas);

        if (vidas <= 0)
        {
            // Sin vidas — avisamos al GameManager
            G3_GameManager.Instance.PerderPartida();
        }
        else
        {
            // Activamos invencibilidad temporal
            StartCoroutine(PeriodoInvencibilidad());
        }
    }

    private IEnumerator PeriodoInvencibilidad()
    {
        _invencible = true;
        // Esperamos el tiempo de invencibilidad
        yield return new WaitForSeconds(_tiempoInvencible);
        _invencible = false;
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        // Recibe daño si le toca una bala enemiga o un enemigo directamente
        if (otro.CompareTag("BalaEnemigo") || otro.CompareTag("Enemy"))
        {
            RecibirDaño();
        }
    }
}