using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [Header("Referencias de UI")]
    [SerializeField] private GameObject panelOpciones;
    [SerializeField] private Slider sliderMusic;
    [SerializeField] private Slider sliderEffects;
    [Header("Mute")]
    [SerializeField] private Image imagenBotonMute; // Image del botón mute
    [SerializeField] private Sprite spriteMute;     // Sprite que se mostrará cuando esté muteado

    private Sprite spriteOriginalMute;              // Guarda automáticamente el sprite original del botón

    private float volumenPrevioMusica = 0.5f;
    private float volumenPrevioEfectos = 0.5f;
    private bool estaMuteado = false;
    private bool estaAbierto = false;

    private void Start()
    {
        if (panelOpciones != null)
        {
            panelOpciones.SetActive(false);
            estaAbierto = false;
        }

        ConfigurarSliders();

        // Guardamos el sprite original que ya tiene el botón en el Inspector. Será el icono de sonido activado.
        if (imagenBotonMute != null)
        {
            spriteOriginalMute = imagenBotonMute.sprite;
        }

        ActualizarIconoMute();
    }

    private void ConfigurarSliders()
    {
        if (sliderMusic != null)
        {
            sliderMusic.minValue = 0f;
            sliderMusic.maxValue = 1f;

            if (AudioManager.Instance != null && AudioManager.Instance.musicSource != null)
                sliderMusic.value = AudioManager.Instance.musicSource.volume;

            sliderMusic.onValueChanged.AddListener(CambiarVolumenMusica);
        }

        if (sliderEffects != null)
        {
            sliderEffects.minValue = 0f;
            sliderEffects.maxValue = 1f;

            if (AudioManager.Instance != null && AudioManager.Instance.sfxSource != null)
                sliderEffects.value = AudioManager.Instance.sfxSource.volume;

            sliderEffects.onValueChanged.AddListener(CambiarVolumenEfectos);
        }
    }

    public void AlternarMenuOpciones()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("ui_clicknormal");

        estaAbierto = !estaAbierto;

        if (panelOpciones != null)
            panelOpciones.SetActive(estaAbierto);
    }

    public void CambiarVolumenMusica(float valor)
    {
        if (estaMuteado) return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(valor);
    }

    public void CambiarVolumenEfectos(float valor)
    {
        if (estaMuteado) return;

        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(valor);
    }

    public void Boton_AlternarMute()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("ui_clicknormal");

        estaMuteado = !estaMuteado;

        if (estaMuteado)
        {
            volumenPrevioMusica = sliderMusic != null ? sliderMusic.value : 0.5f;
            volumenPrevioEfectos = sliderEffects != null ? sliderEffects.value : 0.5f;

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMusicVolume(0f);
                AudioManager.Instance.SetSFXVolume(0f);
            }

            if (sliderMusic != null) sliderMusic.SetValueWithoutNotify(0f);
            if (sliderEffects != null) sliderEffects.SetValueWithoutNotify(0f);
        }
        else
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMusicVolume(volumenPrevioMusica);
                AudioManager.Instance.SetSFXVolume(volumenPrevioEfectos);
            }

            if (sliderMusic != null) sliderMusic.SetValueWithoutNotify(volumenPrevioMusica);
            if (sliderEffects != null) sliderEffects.SetValueWithoutNotify(volumenPrevioEfectos);
        }
        ActualizarIconoMute();
    }

    public void Boton_Volver()
    {
        if (estaAbierto)
            AlternarMenuOpciones();
    }

    // Cambia el icono del botón mute según si el juego está muteado o no.
    private void ActualizarIconoMute()
    {
        if (imagenBotonMute == null) return;

        if (estaMuteado)
        {
            // Si está muteado, ponemos el sprite de mute.
            if (spriteMute != null)
                imagenBotonMute.sprite = spriteMute;
        }
        else
        {
            // Si NO está muteado, recuperamos el sprite original del botón.
            if (spriteOriginalMute != null)
                imagenBotonMute.sprite = spriteOriginalMute;
        }
    }
}