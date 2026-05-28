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
        if (string.IsNullOrEmpty(nombreJugadorActual))
        {
            if (PlayerPrefs.HasKey("NombrePilotoTemporal"))
            {
                nombreJugadorActual = PlayerPrefs.GetString("NombrePilotoTemporal");
            }
        }

        if (string.IsNullOrEmpty(nombreJugadorActual)) return;

        List<FilaScoreboard> listaScore = CargarScoreboard();
        listaScore.Add(new FilaScoreboard(nombreJugadorActual, puntosTotales));

        // >>> CAMBIO SEGURO: Ordenamos de forma base y luego invertimos para asegurar que el mayor vaya arriba
        listaScore.Sort((x, y) => x.puntuacion.CompareTo(y.puntuacion));
        listaScore.Reverse(); // ¡Esto da la vuelta a la tortilla! El más alto se pone el primero.

        if (listaScore.Count > 5)
        {
            listaScore.RemoveRange(5, listaScore.Count - 5);
        }

        ListaContenedor contenedor = new ListaContenedor();
        contenedor.lista = listaScore;
        string json = JsonUtility.ToJson(contenedor);
        PlayerPrefs.SetString("ScoreboardData", json);
        PlayerPrefs.Save();
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
            // Creamos la lista base
            List<FilaScoreboard> listaPorDefecto = new List<FilaScoreboard>()
            {
                new FilaScoreboard("Luke_Skywalker", 500),
                new FilaScoreboard("Chewbacca", 400),
                new FilaScoreboard("Han_Solo", 300),
                new FilaScoreboard("R2D2", 200),
                new FilaScoreboard("C3PO", 100)
            };

            // >>> CAMBIO SEGURO: Los ordenamos e invertimos igual que antes para blindar el inicio
            listaPorDefecto.Sort((x, y) => x.puntuacion.CompareTo(y.puntuacion));
            listaPorDefecto.Reverse();

            ListaContenedor contenedor = new ListaContenedor();
            contenedor.lista = listaPorDefecto;
            string json = JsonUtility.ToJson(contenedor);
            PlayerPrefs.SetString("ScoreboardData", json);
            PlayerPrefs.Save();

            return listaPorDefecto;
        }
    }
}