using UnityEngine;
using System.Collections;

public class G3_Spawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject prefabEnemigo; // Prefab del enemigo a instanciar
    public GameObject prefabBoss;    // Prefab del boss final

    [Header("Configuración de Oleadas")]
    public int enemigosOleada1 = 3;            // Número de enemigos en la primera oleada
    public int enemigosOleada2 = 5;
    public int enemigosOleada3 = 7;// Número de enemigos en la segunda oleada
    public float separacionEntreEnemigos = 2f; // Espacio horizontal entre enemigos al spawnear
    public float tiempoMaxOleada = 15f;        // Tiempo máximo antes de forzar la siguiente oleada
                                               // aunque queden enemigos vivos

    [Header("Posición de Spawn")]
    public float posYSpawn = 5f; // Altura en la que aparecen los enemigos y el boss

    private int _enemigosVivos = 0; // Contador de enemigos vivos en la oleada actual
    
    [Header("Power-ups")]
    public GameObject prefabPowerUpVida;    // Prefab del power-up Vida
    public GameObject prefabPowerUpBonus;   // Prefab del power-up Bonus
    public int totalPowerUpsVida = 1;       // Cuántos PowerUpVida soltar en toda la partida
    public int totalPowerUpsBonus = 2;      // Cuántos PowerUpBonus soltar en toda la partida

    private int _powerUpsVidaSoltados = 0;  // Contador de cuántos PowerUpVida se han soltado
    private int _powerUpsBonusSoltados = 0; // Contador de cuántos PowerUpBonus se han soltado
    private int _enemigosTotales = 0;       // Total de enemigos que aparecen en la partida
    private int _enemigosMuertos = 0;       // Cuántos enemigos han muerto

    void Start()
    {
        
        // Arrancamos la corrutina que gestiona el flujo de oleadas
        // Calculamos el total de enemigos para distribuir los power-ups
       _enemigosTotales = enemigosOleada1 + enemigosOleada2 + enemigosOleada3;
        StartCoroutine(GestionarOleadas());
    }
    
    private IEnumerator GestionarOleadas()
    {
        yield return new WaitForSeconds(1f);

        // ---- OLEADA 1 ----
        SpawnOleada(enemigosOleada1);
        yield return new WaitUntil(() => _enemigosVivos <= 0 ||
                                         Time.timeSinceLevelLoad > tiempoMaxOleada);
        yield return new WaitForSeconds(2f);

        // ---- OLEADA 2 ----
        SpawnOleada(enemigosOleada2);
        yield return new WaitUntil(() => _enemigosVivos <= 0 ||
                                         Time.timeSinceLevelLoad > tiempoMaxOleada * 2);
        yield return new WaitForSeconds(2f);

        // ---- OLEADA 3 ----
        SpawnOleada(enemigosOleada3);
        yield return new WaitUntil(() => _enemigosVivos <= 0 ||
                                         Time.timeSinceLevelLoad > tiempoMaxOleada * 3);
        yield return new WaitForSeconds(2f);

        // ---- BOSS ----
        SpawnBoss();
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

    public void SpawnBoss()
    {
        // Destruimos enemigos que queden vivos
        foreach (GameObject enemigo in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemigo);
        }

        // Instanciamos el boss
        if (prefabBoss != null)
        {
            GameObject boss = Instantiate(prefabBoss, new Vector3(0, posYSpawn, 0), prefabBoss.transform.rotation);

            // Soltamos los power-ups pendientes en la posición del boss
            SoltarPowerUpsPendientes(boss.transform.position);
        }
    }

    private void SoltarPowerUpsPendientes(Vector3 posicion)
    {
        // Soltamos todos los PowerUpVida que falten
        while (_powerUpsVidaSoltados < totalPowerUpsVida)
        {
            // Pequeño offset aleatorio para que no caigan exactamente en el mismo punto
            Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0, 0);
            Instantiate(prefabPowerUpVida, posicion + offset, Quaternion.identity);
            _powerUpsVidaSoltados++;
        }

        // Soltamos todos los PowerUpBonus que falten
        while (_powerUpsBonusSoltados < totalPowerUpsBonus)
        {
            Vector3 offset = new Vector3(Random.Range(-1f, 1f), 0, 0);
            Instantiate(prefabPowerUpBonus, posicion + offset, Quaternion.identity);
            _powerUpsBonusSoltados++;
        }
    }

    // El enemigo llama a este método cuando muere para actualizar el contador
    public void EnemigoDerrotado()
    {
        _enemigosVivos--;
        _enemigosMuertos++;

        GameObject ultimoEnemigo = GameObject.FindGameObjectWithTag("Enemy");
        Vector3 posSpawn = ultimoEnemigo != null ? ultimoEnemigo.transform.position : Vector3.zero;

        IntentarSoltarPowerUp(posSpawn);
    }

    private void IntentarSoltarPowerUp(Vector3 posicion)
    {
        int enemigosRestantes = _enemigosTotales - _enemigosMuertos;

        if (_powerUpsVidaSoltados < totalPowerUpsVida)
        {
            if (enemigosRestantes <= 0 || Random.Range(0, enemigosRestantes + 1) == 0)
            {
                if (prefabPowerUpVida != null)
                {
                    Instantiate(prefabPowerUpVida, posicion, Quaternion.identity);
                    _powerUpsVidaSoltados++;
                    return;
                }
            }
        }

        if (_powerUpsBonusSoltados < totalPowerUpsBonus)
        {
            if (enemigosRestantes <= 0 || Random.Range(0, enemigosRestantes + 1) == 0)
            {
                if (prefabPowerUpBonus != null)
                {
                    Instantiate(prefabPowerUpBonus, posicion, Quaternion.identity);
                    _powerUpsBonusSoltados++;
                }
            }
        }
    }
}