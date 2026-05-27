using UnityEngine;

// ==============================================================================
// >>> PAUSE MENU: Menú de pausa reutilizable para todos los minijuegos
// Se mete como prefab en cada escena. Al pulsar el botón de pausa
// se congela el juego y aparecen las opciones.
// Al salir, se recarga la escena desde el inicio.
// ==============================================================================

public class PauseMenu : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panelPausa; // Panel que contiene los botones de pausa

    private bool _enPausa = false;

    void Start()
    {
        // El panel empieza oculto
        if (panelPausa != null)
            panelPausa.SetActive(false);
    }

    void Update()
    {
        // En PC se puede pausar con ESC (en móvil se usa el botón en pantalla)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AlternarPausa();
        }
    }

    // Alterna entre pausar y reanudar el juego
    // Se llama desde el botón de pausa en pantalla o con ESC
    public void AlternarPausa()
    {
        _enPausa = !_enPausa;

        if (panelPausa != null)
            panelPausa.SetActive(_enPausa);

        // Congelamos o reanudamos el tiempo del juego
        Time.timeScale = _enPausa ? 0f : 1f;
    }

    // Botón "Reanudar" — cierra el menú y continúa el juego
    public void BotonReanudar()
    {
        _enPausa = false;

        if (panelPausa != null)
            panelPausa.SetActive(false);

        Time.timeScale = 1f;
    }

    // Botón "Abandonar" — sale al menú correspondiente
    // Si estás en modo historia va al MainMenu
    // Si estás en modo libre va al MenuSeleccionJuegos
    public void BotonAbandonar()
    {
        Time.timeScale = 1f;

        if (MainManager.Instance != null)
        {
            MainManager.Instance.AbandonarPartida();
        }
    }
}
