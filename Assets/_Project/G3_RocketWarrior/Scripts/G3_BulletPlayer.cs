using UnityEngine;

public class G3_BalaJugador : MonoBehaviour
{
    [Header("Configuración")]
    public float velocidad = 8f; // Velocidad a la que sube la bala

    void Update()
    {
        // La bala sube constantemente hacia arriba
        transform.position += Vector3.up * velocidad * Time.deltaTime;

        // Si sale de la pantalla por arriba la destruimos para liberar memoria
        if (transform.position.y > 7f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        // Si toca un enemigo, destruimos la bala
        // El daño al enemigo lo gestionamos desde G3_Enemy
        if (otro.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}