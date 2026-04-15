using UnityEngine;
using System.Collections;

public class G3_Spawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject prefabEnemigo; // Arrastra aquí el prefab Enemy desde el Project

    [Header("Configuración de Oleadas")]
    public int enemigosOleada1 = 3; // Enemigos en la primera oleada
    public int enemigosOleada2 = 5; // Enemigos en la segunda oleada
    public float separacionEntreEnemigos = 2f; // Espacio horizontal entre enemigos
    public float tiempoEntreOleadas = 3f; // Segundos de espera entre oleadas

    [Header("Posición de Spawn")]
    public float posYSpawn = 5f; // Altura a la que aparecen los enemigos

    void Start()
    {
        // Arrancamos la corrutina que gestiona las oleadas en orden
        StartCoroutine(GestionarOleadas());
    }

    private IEnumerator GestionarOleadas()
    {
        // Esperamos un segundo antes de empezar para dar tiempo al jugador
        yield return new WaitForSeconds(1f);

        // Spawneamos la primera oleada
        SpawnOleada(enemigosOleada1);

        // Esperamos a que pasen los segundos configurados antes de la siguiente
        yield return new WaitForSeconds(tiempoEntreOleadas);

        // Spawneamos la segunda oleada
        SpawnOleada(enemigosOleada2);
    }

    private void SpawnOleada(int cantidad)
    {
        // Calculamos el punto de inicio para centrar los enemigos en pantalla
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
}