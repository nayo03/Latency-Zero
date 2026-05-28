using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Collections; // >>> NUEVO: Necesario para usar Corrutinas

public class ContenedorScoreboard : MonoBehaviour
{
    [Header("Referencias de UI")]
    public GameObject filaPrefab;
    public Transform contenedorFilas;
    public GameObject panelRegistroNombre; // >>> NUEVO: Arrastra aquí tu 'Panel_RegistroNombre'

    private void OnEnable()
    {
        // >>> NUEVO: Forzamos a que el panel de nombre se encienda al arrancar el menú
        if (panelRegistroNombre != null)
        {
            panelRegistroNombre.SetActive(true);
        }

        StartCoroutine(EsperarYPintar());
    }

    private IEnumerator EsperarYPintar()
    {
        // Espera a que termine el primer frame para que MainManager ejecute su Awake()
        yield return new WaitForEndOfFrame();
        PintarScoreboard();
    }

    public void PintarScoreboard()
    {
        // 1. Limpieza de seguridad
        foreach (Transform hijo in contenedorFilas)
        {
            Destroy(hijo.gameObject);
        }

        if (MainManager.Instance == null || MainManager.Instance.baseDeDatos == null)
        {
            Debug.LogWarning("Scoreboard en espera: El MainManager aún no se ha inicializado.");
            return;
        }

        // 2. Cargamos la lista (da igual cómo venga de la base de datos)
        List<MainDataManager.FilaScoreboard> listaMejores = MainManager.Instance.baseDeDatos.CargarScoreboard();

        // >>> ¡EL TRUCO SUPREMO!: Forzamos el orden correcto AQUÍ, milisegundos antes de pintarlo
        // Ordenamos de menor a mayor y luego invertimos para asegurar que el más grande vaya arriba
        listaMejores.Sort((x, y) => x.puntuacion.CompareTo(y.puntuacion));
        

        // 3. Generamos las filas en la UI ya ordenadas sí o sí
        foreach (MainDataManager.FilaScoreboard dato in listaMejores)
        {
            GameObject nuevaFila = Instantiate(filaPrefab, contenedorFilas);

            TMP_Text textoNombre = nuevaFila.transform.Find("Texto_Nombre").GetComponent<TMP_Text>();
            TMP_Text textoPuntos = nuevaFila.transform.Find("Texto_Puntos").GetComponent<TMP_Text>();

            if (textoNombre != null) textoNombre.text = dato.nombre;
            if (textoPuntos != null) textoPuntos.text = dato.puntuacion.ToString();
        }
    }
}