using UnityEngine;

public class G4_AsteroidSpawner : MonoBehaviour
{
    public GameObject[] prefabsAsteroides;
    public Transform camaraJugador; // XR Origin Camera

    public float radioSpawn = 5f;
    public string velocidadSpawn = "Lenta";

    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        float tiempoEspera = ObtenerVelocidad();

        if (timer >= tiempoEspera)
        {
            Spawnear();
            timer = 0f;
        }
    }

    float ObtenerVelocidad()
    {
        if (velocidadSpawn == "Rapida") return 0.8f;
        if (velocidadSpawn == "Moderada") return 1.5f;
        return 3f; // Lenta
    }

    void Spawnear()
    {
        if (prefabsAsteroides.Length == 0 || camaraJugador == null) return;

        GameObject prefab = prefabsAsteroides[Random.Range(0, prefabsAsteroides.Length)];
        Vector3 direccion = Random.onUnitSphere;
        Vector3 posicion = camaraJugador.position + (direccion * radioSpawn);

        Instantiate(prefab, posicion, Random.rotation);
    }
}