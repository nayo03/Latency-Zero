using System.Collections.Generic;
using UnityEngine;

public class G2_PoolManager : MonoBehaviour
{
    public static G2_PoolManager Instance;

    [Header("Fábrica de Asteroides")]
    [Tooltip("Arrastra aquí tus 5 prefabs de asteroides en orden")]
    public GameObject[] asteroidPrefabs;
    public int asteroidsAmount = 3; // Cuántos de cada uno

    [Header("Fábrica de Coleccionables")]
    public GameObject starPrefab;
    public int starsAmount = 2;

    public GameObject breadPrefab;
    public int breadsAmount = 3;

    // El diccionario es nuestro "inventario" interno
    private Dictionary<string, List<GameObject>> poolDictionary;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        poolDictionary = new Dictionary<string, List<GameObject>>();

        // 1. FABRICACIÓN DE ASTEROIDES (Automatizada)
        for (int i = 0; i < asteroidPrefabs.Length; i++)
        {
            // Crea automáticamente los nombres: "G2_Asteroid01", "G2_Asteroid02", etc.
            string autoTag = "G2_Asteroid0" + (i + 1);
            CreatePool(autoTag, asteroidPrefabs[i], asteroidsAmount);
        }

        // 2. FABRICACIÓN DE COLECCIONABLES (Con sus Tags fijos)
        CreatePool("G2_Star", starPrefab, starsAmount);
        CreatePool("G2_Bread", breadPrefab, breadsAmount);
    }

    // ==============================================================================
    // >>> FUNCIÓN AUXILIAR PARA FABRICAR (Para no repetir código)
    // ==============================================================================
    private void CreatePool(string tag, GameObject prefab, int amount)
    {
        if (prefab == null) return; // Por si te olvidas de arrastrar alguno

        List<GameObject> objectPool = new List<GameObject>();
        for (int i = 0; i < amount; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(this.transform);
            objectPool.Add(obj);
        }
        poolDictionary.Add(tag, objectPool);
    }

    // ==============================================================================
    // >>> MÉTODO PARA PEDIR OBJETOS AL ALMACÉN
    // ==============================================================================
    public GameObject GetObjectFromPool(string tag)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("El PoolManager no tiene la caja: " + tag);
            return null;
        }

        for (int i = 0; i < poolDictionary[tag].Count; i++)
        {
            if (!poolDictionary[tag][i].activeInHierarchy)
            {
                return poolDictionary[tag][i];
            }
        }

        return null;
    }
}