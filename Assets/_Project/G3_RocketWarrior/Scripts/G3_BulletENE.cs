using UnityEngine;

public class G3_BalaEnemigo : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidad = 5f; // Velocidad a la que baja la bala hacia el jugador

    void Update()
    {
        // La bala baja constantemente hacia el jugador
        transform.position += Vector3.down * velocidad * Time.deltaTime;

        // Si sale de la pantalla por abajo la destruimos para liberar memoria
        if (transform.position.y < -7f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        // Si toca al jugador destruimos la bala
        // El daño al jugador lo gestionaremos desde G3_GameManager
        if (otro.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}