using UnityEngine;

public class G3_BalaBoss : MonoBehaviour
{
    private Vector2 _direccion;    // Dirección en la que se mueve la bala
    public float velocidad = 4f;   // Velocidad de la bala

    // El Boss llama a este método al instanciar la bala para asignarle dirección
    public void SetDireccion(Vector2 direccion)
    {
        _direccion = direccion.normalized;
    }

    void Update()
    {
        // La bala se mueve en la dirección asignada
        transform.position += (Vector3)_direccion * velocidad * Time.deltaTime;

        // Si sale de la pantalla la destruimos
        if (transform.position.y < -7f || Mathf.Abs(transform.position.x) > 10f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        // Si toca al jugador se destruye
        if (otro.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}