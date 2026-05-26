using System.Collections.Generic;
using UnityEngine;

// =========================================================================
// >>> MAINDATAMANAGER: El "Disco Duro" del juego
// Este archivo guarda los datos de forma permanente en el proyecto (.asset)
// =========================================================================

[CreateAssetMenu(fileName = "MainManager", menuName = "Sistema/Main Manager")]
public class MainDataManager : ScriptableObject
{
    // Vista del Inspector
    [Header("Progreso Global")]
    public int puntosTotales = 0;
    public string nombreJugadorActual = "";

    [Header("Ajustes de Partida")]
    public bool modoHistoriaActivo = false;

    // Estructura para serializar cada fila del Scoreboard
    [System.Serializable]
    public class FilaScoreboard
    {
        public string nombre;
        public int puntuacion;

        public FilaScoreboard(string nom, int pts)
        {
            nombre = nom;
            puntuacion = pts;
        }
    }

    // Lista auxiliar para guardar en PlayerPrefs
    [System.Serializable]
    private class ListaContenedor
    {
        public List<FilaScoreboard> lista = new List<FilaScoreboard>();
    }

    // =========================================================================
    //                            FUNCIONES DE CONTROL
    // =========================================================================

    // Borra la puntuación acumulada (se usa al volver al menú principal o empezar de cero)
    public void ResetearProgreso()
    {
        puntosTotales = 0;
    }

    // Añade los puntos ganados en un minijuego al total global
    public void SumarPuntos(int puntosNivel)
    {
        puntosTotales += puntosNivel;
    }

    // =========================================================================
    //                                 SCOREBOARD
    // =========================================================================
    public void GuardarEnScoreboard()
    {
        // >>> NUEVO: Si por un cambio de escena el nombre quedó vacío en RAM, lo rescatamos de PlayerPrefs
        if (string.IsNullOrEmpty(nombreJugadorActual))
        {
            if (PlayerPrefs.HasKey("NombrePilotoTemporal"))
            {
                nombreJugadorActual = PlayerPrefs.GetString("NombrePilotoTemporal");
                Debug.Log("[Scoreboard] Nombre rescatado con éxito de PlayerPrefs: " + nombreJugadorActual);
            }
        }

        // De aquí en adelante el código sigue exactamente igual...
        Debug.Log($"[Scoreboard] Intentando guardar partida. Nombre: {nombreJugadorActual}, Puntos: {puntosTotales}");

        if (string.IsNullOrEmpty(nombreJugadorActual))
        {
            Debug.LogWarning("[Scoreboard] No se puede guardar: El nombre del jugador está vacío.");
            return;
        }

        List<FilaScoreboard> listaScore = CargarScoreboard();
        listaScore.Add(new FilaScoreboard(nombreJugadorActual, puntosTotales));
        listaScore.Sort((x, y) => y.puntuacion.CompareTo(x.puntuacion));

        if (listaScore.Count > 5)
        {
            listaScore.RemoveRange(5, listaScore.Count - 5);
        }

        ListaContenedor contenedor = new ListaContenedor();
        contenedor.lista = listaScore;
        string json = JsonUtility.ToJson(contenedor);
        PlayerPrefs.SetString("ScoreboardData", json);
        PlayerPrefs.Save();

        Debug.Log("[Scoreboard] ¡Partida guardada con éxito en PlayerPrefs! JSON generado: " + json);
    }

    
    // Método para obtener la lista desde cualquier sitio (como el menú principal)
    public List<FilaScoreboard> CargarScoreboard()
    {
        if (PlayerPrefs.HasKey("ScoreboardData"))
        {
            string json = PlayerPrefs.GetString("ScoreboardData");
            ListaContenedor contenedor = JsonUtility.FromJson<ListaContenedor>(json);
            return contenedor.lista;
        }
        else
        {
            // >>> NUEVO: Si no hay datos (primera vez que se abre el juego), creamos el Top 5 de fábrica
            List<FilaScoreboard> listaPorDefecto = new List<FilaScoreboard>()
            {
                new FilaScoreboard("Luke_Skywalker", 500),
                new FilaScoreboard("Chewbacca", 400),
                new FilaScoreboard("Han_Solo", 300),
                new FilaScoreboard("R2D2", 200),
                new FilaScoreboard("C3PO", 100)
            };

            // Los guardamos inmediatamente en PlayerPrefs para que ya existan de forma permanente
            ListaContenedor contenedor = new ListaContenedor();
            contenedor.lista = listaPorDefecto;
            string json = JsonUtility.ToJson(contenedor);
            PlayerPrefs.SetString("ScoreboardData", json);
            PlayerPrefs.Save();

            return listaPorDefecto;
        }
    }
}