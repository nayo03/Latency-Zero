using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ==============================================================================
// >>> UIMAINMANAGER: Control de botones, pausa y configuración de audio
// Este script actúa como "puente" entre la UI física y la lógica del MainManager.
//
// ** NOTA DE INTEGRACIÓN: Lo mantengo como prefab por seguridad. Mejor que cada uno
// haga su propio UIManager para que este esté seguro.
//
// ** NOTA DE ESTRUCTURA: No está dentro de Global_SYSTEM de forma rígida porque 
//    al ser un componente de UI, requiere referencias directas con objetos de la 
//    escena local (botones, paneles). Mantenerlo externo evita que los eventos 
//    'OnClick' se desconecten al cambiar de nivel.
// ==============================================================================

// ---------------------------------------------------------------------
//                     ACCIONES DE AUDIO PARA FUTURO
// ---------------------------------------------------------------------

public class UIMainManager : MonoBehaviour
{

    public void ReproducirSonidoBoton()
    {
        // Llama al sonido de "Click" de tu librería cada vez que pulses algo
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("Click");
        }
    }

    // ---------------------------------------------------------------------
    //                 BOTONES PARA EL MENÚ PRINCIPAL 
    // ---------------------------------------------------------------------

    
    public void Boton_IniciarHistoria() // EN USO EN MAINMENU
    {

        ReproducirSonidoBoton();

        if (MainManager.Instance != null)
        {
            MainManager.Instance.modoHistoriaActivo = true;

            // Usamos ContinuarHistoria para que el MainManager decida qué escena toca
            MainManager.Instance.ContinuarHistoria();
        }
        else
        {
            Debug.LogError("<color=red>Error:</color> No se encuentra el MainManager en la escena.");
        }
    }

    public void Boton_IrASeleccionNiveles() // EN USO EN MAINMENU
    {
        ReproducirSonidoBoton();
        if (MainManager.Instance != null) MainManager.Instance.modoHistoriaActivo = false;
        SceneManager.LoadScene("MenuSeleccionJuegos");
    }

    public void Boton_SalirDelJuego() // EN USO EN MAINMENU
    {
        ReproducirSonidoBoton();
        Application.Quit();
    }

    // ---------------------------------------------------------------------
    //                  PARA EL SELECTOR DE JUEGOS 
    // ---------------------------------------------------------------------

    public void CargarNivelSuelto(string nombreEscena) // EN USO EN SLECTOR DE JUEGOS
    {
        ReproducirSonidoBoton();
        if (MainManager.Instance != null)
        {
            MainManager.Instance.modoHistoriaActivo = false;
        }
        SceneManager.LoadScene(nombreEscena);
    }

    public void Boton_JugarG1() => CargarNivelSuelto("G1_Inicial"); // EN USO EN SELECTOR DE JUEGOS G1
    public void Boton_JugarG2() => CargarNivelSuelto("G2_Inicial"); // EN USO EN SELECTOR DE JUEGOS G2
    public void Boton_JugarG3() => CargarNivelSuelto("G3_Inicial"); // EN USO EN SELECTOR DE JUEGOS G3
    public void Boton_JugarG4() => CargarNivelSuelto("G4_Inicial"); // EN USO EN SELECTOR DE JUEGOS G4
    public void Boton_JugarG5() => CargarNivelSuelto("G5_Inicial"); // EN USO EN SELECTOR DE JUEGOS G5



    // ---------------------------------------------------------------------
    //                     VOLVER ATRÁS Y CIERRE
    // ---------------------------------------------------------------------

    public void Boton_AbandonarPartida() // EN USO EN CADA MINIJUEGO AL FINAL - VOLVER
    {
        ReproducirSonidoBoton();

        if (MainManager.Instance != null)
        {

            MainManager.Instance.AbandonarPartida();
        }
        else
        {
            // Por si acaso falla el Manager
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void Boton_VolverAlMainMenu() // EN USO EN SELECTOR DE JUEGOS
    {
        ReproducirSonidoBoton();

        if (MainManager.Instance != null)
        {
            MainManager.Instance.VolverAlMainMenu();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    // EN USO BOTÓN DE SIGUIENTE DE LOS NIVELES G1, G2, G3 y G4 EN MODO HISTORIA
    public void Boton_SiguienteNormal()
    {
        ReproducirSonidoBoton();
        if (MainManager.Instance != null) MainManager.Instance.FinalizarEscenaActual();
    }

    // EN USO BOTÓN DE SIGUIENTE SOLO DEL NIVEL G5 EN MODO HISTORIA
    public void Boton_SiguienteFinal()
    {
        ReproducirSonidoBoton();
        if (MainManager.Instance != null)
        {
            // Esta función ya la tienes en tu MainManager, solo la llamamos
            MainManager.Instance.PrepararFinalDelJuego();
        }
    }
    // EN USO BOTÓN DE VOLVER DE ESCENA PUNTUACIÓNFINAL: Para cuando se acaba el modo historia y sale la puntuación.
    public void Boton_ResetTotal()
    {
        ReproducirSonidoBoton();
        if (MainManager.Instance != null)
        {
            MainManager.Instance.ResetTotal();
        }
    }


}