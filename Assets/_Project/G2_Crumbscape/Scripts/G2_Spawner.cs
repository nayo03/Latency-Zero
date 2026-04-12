using System.Collections;
using UnityEngine;

public class G2_Spawner : MonoBehaviour
{
    // ETIQUETAS DEL POOL: Los siguientes tags deben ser los mismos que los del PoolManager
    private string[] asteroidTags = { "G2_Asteroid01", "G2_Asteroid02", "G2_Asteroid03", "G2_Asteroid04", "G2_Asteroid05" };
    private string starTag = "G2_Star";
    private string breadTag = "G2_Bread";

    [Header("Zonas de Aparición (Eje Y)")]
    public float spawnPosX = 12f;         // A qué distancia a la derecha aparecen
    public float maxY = 4.5f;             // Tope superior de la pantalla
    public float minY = -4.5f;            // Tope inferior de la pantalla

    [Tooltip("Límites del centro de la pantalla (Zona Segura)")]
    public float safeZoneMaxY = 1.5f;
    public float safeZoneMinY = -1.5f;

    [Header("Tiempos de Generación (Nivel 1)")]
    public float asteroidSpawnRate = 2f;
    public float starSpawnRate = 4f;
    public float breadSpawnRate = 7f;

    [Header("Sistema de Niveles")]
    public float timeToLevelUp = 40f; // Cada cuantos segundos se pasa de nivel
    private int currentLevel = 1;

    void Start()
    {
        // Arrancamos las tres fábricas de objetos y el reloj de niveles
        StartCoroutine(SpawnAsteroidsRoutine());
        StartCoroutine(SpawnStarsRoutine());
        StartCoroutine(SpawnBreadRoutine());
        StartCoroutine(LevelUpRoutine());
    }

    // =========================================================
    // RUTINAS DE GENERACIÓN
    // =========================================================
    IEnumerator SpawnAsteroidsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(asteroidSpawnRate);

            // Elegimos un nombre de asteroide al azar de nuestra lista
            string randomTag = asteroidTags[Random.Range(0, asteroidTags.Length)];

            // Le pedimos ese asteroide al jefe de almacén
            GameObject asteroid = G2_PoolManager.Instance.GetObjectFromPool(randomTag);

            if (asteroid != null)
            {
                // Los asteroides salen en cualquier punto de la pantalla
                float randomY = Random.Range(minY, maxY);
                asteroid.transform.position = new Vector2(spawnPosX, randomY);
                asteroid.SetActive(true); // ¡Lo encendemos!
            }
        }
    }

    IEnumerator SpawnStarsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(starSpawnRate);

            GameObject star = G2_PoolManager.Instance.GetObjectFromPool(starTag);
            if (star != null)
            {
                // La estrella sale SIEMPRE en la Zona Segura (centro)
                float randomY = Random.Range(safeZoneMinY, safeZoneMaxY);
                star.transform.position = new Vector2(spawnPosX, randomY);
                star.SetActive(true);
            }
        }
    }

    IEnumerator SpawnBreadRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(breadSpawnRate);

            GameObject bread = G2_PoolManager.Instance.GetObjectFromPool(breadTag);
            if (bread != null)
            {
                // El pan sale SIEMPRE en la Zona Peligrosa (arriba o abajo, fuera del centro)
                float randomY;
                if (Random.value > 0.5f)
                    randomY = Random.Range(safeZoneMaxY, maxY); // Arriba
                else
                    randomY = Random.Range(minY, safeZoneMinY); // Abajo

                bread.transform.position = new Vector2(spawnPosX, randomY);
                bread.SetActive(true);
            }
        }
    }

    // =========================================================
    // CONTROL DE DIFICULTAD
    // =========================================================
    IEnumerator LevelUpRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToLevelUp);
            currentLevel++;

            // Hacemos que los asteroides salgan más rápido (restando tiempo)
            // Usamos Mathf.Max para asegurarnos de que el tiempo nunca sea menor a 0.5 segundos
            asteroidSpawnRate = Mathf.Max(0.5f, asteroidSpawnRate - 0.2f);

            Debug.Log("¡NIVEL " + currentLevel + " ALCANZADO! Asteroides cada: " + asteroidSpawnRate + "s");
        }
    }
}
