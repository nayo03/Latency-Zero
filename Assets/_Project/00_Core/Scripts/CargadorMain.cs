using UnityEngine;
using UnityEngine.SceneManagement;

// =========================================================================
// >>> CARGADORINICIAL: El "arranque" del motor del juego
// =========================================================================

public class CargadorMain : MonoBehaviour
{
    void Start()
    {
        // 1. Esperamos al Start para que el MainManager (Awake) ya exista
        // 2. Saltamos a la primera escena real del usuario
        SceneManager.LoadScene("MainMenu");
    }
}