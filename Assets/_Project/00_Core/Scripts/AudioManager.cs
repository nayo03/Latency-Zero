using UnityEngine;
using System.Collections.Generic;

// =========================================================================
// >>> AUDIOMANAGER: Gestión global de música de fondo y efectos de sonido
// ** Control centralizado de audio. Los sonidos se añaden a la librería. **
// =========================================================================

public class AudioManager : MonoBehaviour
{
    // REFERENCIAS
    public static AudioManager Instance; // Instancia global para acceso desde cualquier script

    // =========================================================================
    //                      CONFIGURACIÓN DE CANALES
    // =========================================================================
    [Header("Configuración de Canales")]
    public AudioSource musicSource; // Canal dedicado a la música (Loop activo)
    public AudioSource sfxSource;   // Canal dedicado a efectos cortos (PlayOneShot)

    // =========================================================================
    //                      LIBRERÍA DE SONIDOS (SFX)
    // =========================================================================
    [Header("Librería de Sonidos")]
    public List<SoundEffect> sfxLibrary; // Lista de efectos configurables desde el Inspector

    [System.Serializable]
    public struct SoundEffect
    {
        public string nombre;  // Identificador del sonido (ej: "Explosion")
        public AudioClip clip; // Archivo de audio asignado
    }

    private void Awake()
    {
        // =====================================================================
        // NÚCLEO DEL SISTEMA (SINGLETON): Supervivencia entre escenas
        // =====================================================================
        if (Instance == null)
        {
            Instance = this;
            // Nota: Se recomienda que el objeto padre tenga el DontDestroyOnLoad
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // =========================================================================
    //                      GESTIÓN DE REPRODUCCIÓN
    // =========================================================================

    // Busca un efecto por su nombre en la librería y lo reproduce en el canal SFX
    public void PlaySFX(string nombreSonido)
    {
        // Buscamos la estructura que coincida con el nombre proporcionado
        SoundEffect sfx = sfxLibrary.Find(s => s.nombre == nombreSonido);

        // Verificación de seguridad: solo reproduce si el clip existe
        if (sfx.clip != null)
        {
            sfxSource.PlayOneShot(sfx.clip);
        }
        else
        {
            // Aviso en consola para depuración si el sonido no se encuentra o está vacío
            // Debug.LogWarning("AudioManager: El sonido '" + nombreSonido + "' no está asignado o no existe en la librería.");
        }
    }

    // Función para cambiar la música de fondo de forma dinámica
    public void PlayMusic(AudioClip nuevaMusica)
    {
        if (musicSource.clip == nuevaMusica) return;

        musicSource.clip = nuevaMusica;
        musicSource.Play();
    }
}