using System.Collections.Generic;
using UnityEngine;

// ==============================================================================
// >>> G2_POOLMANAGER: Sistema de optimización mediante Diccionario de Listas
// ==============================================================================
public class G2_PoolManager : MonoBehaviour
{
    public static G2_PoolManager Instance; // Instancia única para acceder desde cualquier script

    [Header("Moldes de Asteroides")]
    public GameObject[] asteroidPrefabs; // Array que contiene los diferentes tipos de asteroides
    public int asteroidsAmount = 3;      // Cantidad de copias que crearemos de cada tipo

    [Header("Moldes de Coleccionables")]
    public GameObject starPrefab;  // Prefab de la estrella
    public int starsAmount = 2;    // Cantidad de estrellas en el pool

    public GameObject breadPrefab; // Prefab del pan
    public int breadsAmount = 3;   // Cantidad de panes en el pool

    // Estructura de datos: La 'Key' es el ID (string) y el 'Value' es la Lista de clones
    private Dictionary<string, List<GameObject>> poolDictionary;

    // ==========================================================================
    // PREPARACIÓN INICIAL (Singleton y Fabricación)
    // ==========================================================================
    void Awake()
    {
        // SINGLETON: Nos asegura que solo haya un PoolManager activo
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // Si hay otro, este se destruye para no duplicar

        // Inicializamos el Diccionario donde guardaremos todas las listas de objetos
        poolDictionary = new Dictionary<string, List<GameObject>>();

        // 1. REGISTRO Y CREACIÓN DE ASTEROIDES POR LISTA (Automatizado)
        for (int i = 0; i < asteroidPrefabs.Length; i++)
        {
            // Paso A: Creamos un ID único (string) combinando texto con el número de posición
            // Ejemplo: i=0 resulta en "G2_Asteroid01"
            string autoTag = "G2_Asteroid0" + (i + 1);

            // Paso B: Enviamos el ID, el molde (prefab) y la cantidad a CreatePool para la instanciación
            CreatePool(autoTag, asteroidPrefabs[i], asteroidsAmount);
        }

        // 2. REGISTRO Y CREACIÓN DE COLECCIONABLES CON ID FIJO (Manual)
        CreatePool("G2_Star", starPrefab, starsAmount);
        CreatePool("G2_Bread", breadPrefab, breadsAmount);
    }

    // ==========================================================================
    // FUNCIÓN AUXILIAR: Instanciación y registro en el Diccionario
    // ==========================================================================
    private void CreatePool(string tag, GameObject prefab, int amount)
    {
        if (prefab == null) return; // Filtro de seguridad: si no hay prefab asignado, salimos

        List<GameObject> objectPool = new List<GameObject>(); // Creamos una lista nueva para este ID

        for (int i = 0; i < amount; i++) // Ejecutamos el bucle según el stock deseado
        {
            // Instanciamos el objeto fuera de cámara y lo desactivamos para optimizar
            GameObject obj = Instantiate(prefab, new Vector3(20f, 0f, 0f), Quaternion.identity);

            obj.SetActive(false); // Desactivado por defecto: no consume recursos de CPU/GPU
            obj.transform.SetParent(this.transform); // Organización jerárquica dentro del Manager
            objectPool.Add(obj); // Añadimos la instancia a la lista de este ID
        }

        poolDictionary.Add(tag, objectPool); // Registramos la lista completa en el Diccionario usando su ID (tag)
    }

    // ==========================================================================
    // MÉTODO PÚBLICO: Recuperación de objetos del Pool
    // ==========================================================================
    public GameObject GetObjectFromPool(string tag)
    {
        // FILTRO 1: Comprobamos si el ID (Key) solicitado existe en el Diccionario
        if (!poolDictionary.ContainsKey(tag))
        {
            return null;
        }

        // FILTRO 2: Buscamos en la lista asociada a ese ID una instancia que esté inactiva
        for (int i = 0; i < poolDictionary[tag].Count; i++)
        {
            if (!poolDictionary[tag][i].activeInHierarchy) // Si el objeto está libre...
            {
                return poolDictionary[tag][i]; // Retornamos la instancia para su uso
            }
        }

        // Si todas las instancias de la lista están activas, no devolvemos nada
        return null;
    }
}