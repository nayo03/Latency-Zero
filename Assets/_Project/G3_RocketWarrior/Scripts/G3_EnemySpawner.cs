using UnityEngine;
using System.Collections;

public class G3_Spawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject prefabEnemigo; // Prefab del enemigo a instanciar

    [Header("Configuración de Oleadas")]
    public int enemigosOleada1 = 3;          // Número de enemigos en la primera oleada
    public int enemigosOleada2 = 5;          // Número de enemigos en la segunda oleada
    public float separacionEntreEnemigos = 2f; // Espacio horizontal entre enemigos al spawnear
    public float tiempoMaxOleada = 15f;      // Tiempo máximo antes de forzar la siguiente oleada
                                             // aunque queden enemigos vivos

    [Header("Posición de Spawn")]
    public float posYSpawn = 5f; // Altura en la que aparecen los enemigos

    private int _enemigosVivos = 0; // Contador de enemigos vivos en la oleada actual

    void Start()
    {
        // Arrancamos la corrutina que gestiona el flujo de oleadas
        StartCoroutine(GestionarOleadas());
    }

    private IEnumerator GestionarOleadas()
    {
        // Pequeña espera inicial para dar tiempo al jugador a prepararse
        yield return new WaitForSeconds(1f);

        // ---- OLEADA 1 ----
        SpawnOleada(enemigosOleada1);

        // Esperamos hasta que:
        // - Todos los enemigos estén muertos (_enemigosVivos <= 0)
        // - O haya pasado el tiempo máximo (tiempoMaxOleada)
        // Lo que ocurra primero fuerza el paso a la siguiente oleada
        yield return new WaitUntil(() => _enemigosVivos <= 0 ||
            Time.timeSinceLevelLoad > tiempoMaxOleada);

        // Pausa entre oleadas para que el jugador se prepare
        yield return new WaitForSeconds(2f);

        // ---- OLEADA 2 ----
        SpawnOleada(enemigosOleada2);

        // Misma lógica que antes pero con el doble de tiempo máximo
        yield return new WaitUntil(() => _enemigosVivos <= 0 ||
            Time.timeSinceLevelLoad > tiempoMaxOleada * 2);

        yield return new WaitForSeconds(2f);

        // ---- BOSS ----
        // TODO: Aquí irá el spawn del boss
        // De momento avisamos al GameManager de victoria
        G3_GameManager.Instance.GanarPartida();
    }

    private void SpawnOleada(int cantidad)
    {
        // Reiniciamos el contador de enemigos vivos para esta oleada
        _enemigosVivos = cantidad;

        // Calculamos el punto de inicio para centrar los enemigos horizontalmente
        float anchoTotal = (cantidad - 1) * separacionEntreEnemigos;
        float startX = -anchoTotal / 2f;

        // Instanciamos cada enemigo separado horizontalmente
        for (int i = 0; i < cantidad; i++)
        {
            float posX = startX + i * separacionEntreEnemigos;
            Vector3 posSpawn = new Vector3(posX, posYSpawn, 0);
            Instantiate(prefabEnemigo, posSpawn, Quaternion.identity);
        }
    }

    // El enemigo llama a este método cuando muere para actualizar el contador
    public void EnemigoDerrotado()
    {
        _enemigosVivos--;
    }
}