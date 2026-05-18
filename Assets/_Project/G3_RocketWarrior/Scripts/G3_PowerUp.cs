using UnityEngine;

public class G3_PowerUp : MonoBehaviour
{
    // Tipos de power-up disponibles
    public enum TipoPowerUp { Vida, Bonus }

    [Header("Configuración")]
    public TipoPowerUp tipo;            // Tipo del power-up (asignar en cada prefab)
    public float velocidadCaida = 2f;   // Velocidad a la que cae el power-up
    public int puntosBonus = 100;       // Puntos que da el power-up Bonus
    public int puntosVida = 80;         // Puntos que da el power-up Vida según GDD

    void Update()
    {
        // El power-up cae lentamente hacia abajo
        transform.position += Vector3.down * velocidadCaida * Time.deltaTime;

        // Si sale de la pantalla lo destruimos
        if (transform.position.y < -7f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        // Si lo recoge el jugador aplicamos su efecto
        if (otro.CompareTag("Player"))
        {
            AplicarEfecto(otro.gameObject);
            Destroy(gameObject);
        }
    }

    private void AplicarEfecto(GameObject jugador)
    {
        G3_Player player = jugador.GetComponent<G3_Player>();
        if (player == null) return;

        if (tipo == TipoPowerUp.Vida)
        {
            // Recupera una vida (sin superar el máximo) y suma puntos
            player.RecuperarVida();
            G3_GameManager.Instance.SumarPuntos(puntosVida);
        }
        else if (tipo == TipoPowerUp.Bonus)
        {
            // Solo suma puntos
            G3_GameManager.Instance.SumarPuntos(puntosBonus);
        }
    }
}