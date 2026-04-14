using System.Collections;
using UnityEngine;

// ==============================================================================
// >>> G2_SPAWNER: Generador dinámico de obstáculos y coleccionables
// ==============================================================================
public class G2_Spawner : MonoBehaviour
{
    // ----------- IDs DE LOS OBJETOS (KEYS DEL DICCIONARIO) -----------
    private string[] asteroidTags = { "G2_Asteroid01", "G2_Asteroid02", "G2_Asteroid03", "G2_Asteroid04", "G2_Asteroid05" };
    private string starTag = "G2_Star";  // ID para pedir estrellas al Pool
    private string breadTag = "G2_Bread"; // ID para pedir panes al Pool

    // ----------- TIEMPOS DE ESPERA -----------
    [Header("Tiempos de Generación")]
    [SerializeField] private float asteroidSpawnRate = 2f; // Segundos entre asteroides
    [SerializeField] private float starSpawnRate = 4f;     // Segundos entre estrellas
    [SerializeField] private float breadSpawnRate = 7f;    // Segundos entre panes

    // ----------- ZONAS DE SPAWN COLLECTIBLES -----------
    [Header("Zonas de Spawn: COLECCIONABLES")] // (rangos calculados desde el centro de la pantalla)
    [Tooltip("Nivel 1: Muy cerca del centro")]
    [SerializeField] private float colRangoNivel1 = 0.30f;
    [Tooltip("Nivel 2: Apertura media")]
    [SerializeField] private float colRangoNivel2 = 0.55f;
    [Tooltip("Nivel 3: Casi toda la pantalla")]
    [SerializeField] private float colRangoNivel3 = 0.80f;

    // ----------- ZONAS DE SPAWN ASTEROIDES -----------
    [Header("Zonas de Spawn: ASTEROIDES")]
    [Tooltip("Límite máximo pegado al borde (0.95 = casi toca el techo/suelo)")]
    [SerializeField] private float astLimiteBordes = 0.95f;

    [Header("Zonas seguras del jugador (Los asteroides evitan esta zona)")]
    [Tooltip("Nivel 1: Zona segura ancha")]
    [SerializeField] private float zonaSeguraNivel1 = 0.50f;
    [Tooltip("Nivel 2: Zona segura intermedia")]
    [SerializeField] private float zonaSeguraNivel2 = 0.30f;
    [Tooltip("Nivel 3: Zona segura estrecha")]
    [SerializeField] private float zonaSeguraNivel3 = 0.15f;

    // ----------- VARIABLES DE CONTROL -----------
    [Header("Conexión con GameManager")] 
    [SerializeField] private G2_GameManager gameManager; // Referencia para consultar el nivel actual

    private float limitY; // Guardará la medida exacta desde el centro hasta el techo de tu pantalla
    private float spawnX; // Guardará la posición X exacta donde nacerán los objetos
    private bool proximoEnParteSuperior = true; // Interruptor para alternar el spawn de asteroides Arriba/Abajo 

    // ==========================================================================
    // PREPARACIÓN INICIAL (Cálculo de dimensiones y Corrutinas)
    // ==========================================================================
    void Start()
    {
        // --- CÁLCULO DINÁMICO DE PANTALLA ---
        Camera cam = Camera.main;
        if (cam != null)
        {
            limitY = cam.orthographicSize; // Obtenemos el radio vertical de la cámara
            spawnX = (cam.orthographicSize * cam.aspect) + 1f; // Calculamos el borde X + margen de salida
        }

        // --- INICIO DE CICLOS DE GENERACIÓN ---
        StartCoroutine(SpawnAsteroidsRoutine()); // Ciclo de asteroides
        StartCoroutine(SpawnStarsRoutine());     // Ciclo de estrellas
        StartCoroutine(SpawnBreadRoutine());     // Ciclo de panes
    }

    // ==========================================================================
    // CÁLCULO DE POSICIÓN Y PARA COLECCIONABLES
    // ==========================================================================
    float CalcularYCollectibles()
    {
        int nivelReal = (gameManager != null) ? gameManager.NivelActual : 1; // Consultamos nivel al Manager

        // Determinamos el porcentaje de apertura según el nivel
        float porcentaje = colRangoNivel1;                       // Nivel 1
        if (nivelReal == 2) porcentaje = colRangoNivel2;         // Nivel 2
        else if (nivelReal >= 3) porcentaje = colRangoNivel3;    // Nivel 3

        float maxY = limitY * porcentaje; // Escalamos el porcentaje al tamaño real de la cámara
        return Random.Range(-maxY, maxY); // Devolvemos un punto aleatorio entre el rango positivo y negativo
    }

    // =========================================================
    // RUTINA DE GENERACIÓN: ESTRELLAS 
    // =========================================================
    IEnumerator SpawnStarsRoutine()
    {
        while (true)
        {            
            yield return new WaitForSeconds(starSpawnRate); // Espera los segundos configurados en 'starSpawnRate'

            // Le pedimos al PoolManager que nos preste una Estrella que no se esté usando
            GameObject star = G2_PoolManager.Instance.GetObjectFromPool(starTag);

            // ---------- GENERACIÓN DE ESTRELLA ----------
            if (star != null)
            {                
                float randomY = CalcularYCollectibles(); // Calculamos a qué altura saldrá esta estrella específica

                // La teletransportamos al borde derecho (spawnX) y a la altura elegida (randomY)
                star.transform.position = new Vector2(spawnX, randomY);
                
                star.SetActive(true); // La activamos
            }
        }
    }

    // =========================================================
    // RUTINA DE GENERACIÓN: PAN CÓSMICO
    // =========================================================
    IEnumerator SpawnBreadRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(breadSpawnRate); // Espera los segundos configurados en 'breadSpawnRate'

            // Le pedimos al PoolManager que nos preste un Pan que no se esté usando
            GameObject bread = G2_PoolManager.Instance.GetObjectFromPool(breadTag);

            // ---------- GENERACIÓN DE PAN ----------
            if (bread != null)
            {
                float randomY = CalcularYCollectibles(); // Calculamos a qué altura saldrá este pan específico

                // Lo teletransportamos al borde derecho (spawnX) y a la altura elegida (randomY)
                bread.transform.position = new Vector2(spawnX, randomY);

                bread.SetActive(true); // Lo activamos
            }
        }
    }

    // =========================================================
    // RUTINA DE GENERACIÓN: ASTEROIDES 
    // =========================================================
    IEnumerator SpawnAsteroidsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(asteroidSpawnRate);

            // ----------- DETERMINAR DIFICULTAD ACTUAL -----------
            // 1. OBTENCIÓN DE NIVEL (Valor por defecto 1)
            int currentLevel = (gameManager != null) ? gameManager.NivelActual : 1;

            // 2. RANGO DE ASTEROIDES SEGÚN DIFICULTAD
            int minIndex = 0, maxIndex = 3; // Configuración base (Nivel 1)

            if (currentLevel == 2) { minIndex = 1; maxIndex = 4; } // Configuración Nivel 2
            else if (currentLevel >= 3) { minIndex = 2; maxIndex = 5; } // Configuración Nivel 3

            // Pedimos al Pool el ID aleatorio según el rango de dificultad
            string randomTag = asteroidTags[Random.Range(minIndex, maxIndex)];
            GameObject asteroid = G2_PoolManager.Instance.GetObjectFromPool(randomTag);

            // ---------- GENERACIÓN DE ASTEROIDE ----------
            if (asteroid != null)
            {
                // Seleccionamos la zona segura correspondiente al nivel
                float zonaSegura = zonaSeguraNivel1; // Empezamos asumiendo Nivel 1
                if (currentLevel == 2) zonaSegura = zonaSeguraNivel2; // Nivel 2
                else if (currentLevel >= 3) zonaSegura = zonaSeguraNivel3; // Nivel 3

                float minY = limitY * zonaSegura;     // Límite interno (cerca del centro)
                float maxY = limitY * astLimiteBordes; // Límite externo (cerca del borde)

                // Calculamos altura y alternamos entre superior e inferior
                float randomY = Random.Range(minY, maxY);
                if (!proximoEnParteSuperior) randomY *= -1; // Si toca abajo, invertimos el signo

                proximoEnParteSuperior = !proximoEnParteSuperior; // Cambiamos para el siguiente turno

                // Posicionamos y activamos
                asteroid.transform.position = new Vector2(spawnX, randomY);
                asteroid.SetActive(true);
            }
        }
    }
}