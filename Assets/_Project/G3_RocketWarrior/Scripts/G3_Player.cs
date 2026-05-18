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
    
    [Header("Penalización")]
    public int penalizacionPorBala = 25;

    private Vector2 _minBounds;               // Límite inferior-izquierdo de la pantalla
    private Vector2 _maxBounds;               // Límite superior-derecho de la pantalla
    private float _timerDisparo;              // Contador regresivo para el disparo
    private bool _invencible = false;         // Evita recibir daño varias veces seguidas
    private float _tiempoInvencible = 1.5f;   // Segundos de invencibilidad tras recibir daño
    private G3_Joystick _joystick;            // Referencia al joystick virtual (móvil)

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

        // Buscamos el joystick en la escena — puede no existir en PC y es correcto
        _joystick = Object.FindAnyObjectByType<G3_Joystick>();
    }

    void Update()
    {
        // MOVIMIENTO
        // Leemos input del teclado por defecto
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        // Si el joystick existe y tiene input, lo usamos en lugar del teclado
        // Esto permite que funcione tanto en PC (teclado) como en móvil (joystick)
        if (_joystick != null && _joystick.InputJoystick.magnitude > 0)
        {
            x = _joystick.InputJoystick.x;
            y = _joystick.InputJoystick.y;
        }

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

        // Restamos una vida y actualizamos la UI
        vidas--;
        G3_GameManager.Instance.ActualizarVidas(vidas);

        // Penalizamos puntos por cada bala recibida
        // El valor de penalización se configura desde el Inspector
        G3_GameManager.Instance.SumarPuntos(-penalizacionPorBala);

        Debug.Log("Vidas restantes: " + vidas);

        if (vidas <= 0)
        {
            // Sin vidas — avisamos al GameManager para mostrar Game Over
            G3_GameManager.Instance.PerderPartida();
        }
        else
        {
            // Activamos invencibilidad temporal para no recibir daño en cadena
            StartCoroutine(PeriodoInvencibilidad());
        }
    }

    private IEnumerator PeriodoInvencibilidad()
    {
        _invencible = true;

        // Buscamos el SpriteRenderer del jugador para hacerlo parpadear
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        float tiempoTranscurrido = 0f;
        float intervaloFlash = 0.1f; // Cada cuánto cambia la visibilidad

        // Parpadea durante todo el tiempo de invencibilidad
        while (tiempoTranscurrido < _tiempoInvencible)
        {
            // Alterna la visibilidad del sprite
            sprite.enabled = !sprite.enabled;

            // Esperamos el intervalo antes de cambiar de nuevo
            yield return new WaitForSeconds(intervaloFlash);
            tiempoTranscurrido += intervaloFlash;
        }

        // Nos aseguramos de dejar el sprite visible al acabar
        sprite.enabled = true;
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
    
    public void RecuperarVida()
    {
        // Recupera una vida sin superar el máximo de 3
        //if (vidas < 3)
        {
            vidas++;
            G3_GameManager.Instance.ActualizarVidas(vidas);
        }
    }
}