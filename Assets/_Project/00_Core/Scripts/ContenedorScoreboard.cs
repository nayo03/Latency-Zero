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

        // 2. Comprobamos si el MainManager ya está listo
        if (MainManager.Instance == null || MainManager.Instance.baseDeDatos == null)
        {
            Debug.LogWarning("Scoreboard en espera: El MainManager aún no se ha inicializado en esta escena.");
            return;
        }

        // 3. Cargamos la lista ordenada de puntuaciones
        List<MainDataManager.FilaScoreboard> listaMejores = MainManager.Instance.baseDeDatos.CargarScoreboard();

        // 4. Generamos las filas en la UI
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