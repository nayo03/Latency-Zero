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

    [Header("Ajustes de Partida")]
    public bool modoHistoriaActivo = false;

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
}