using UnityEngine;

public class SonidoPasos : MonoBehaviour
{
    [Header("Configuración de Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clipsDePasos; // Lista de sonidos para que no sea repetitivo

    [Header("Ajustes de Ritmo")]
    [SerializeField] private float tiempoEntrePasos = 0.5f; // Qué tan rápido camina (en segundos)
    [SerializeField] private float velocidadMinima = 0.1f;  // Umbral para detectar movimiento

    private float timerPasos;
    private Vector3 posicionPrevia;

    void Start()
    {
        posicionPrevia = transform.position;

        // Si olvidaste arrastrar el AudioSource, el script intenta buscarlo solo
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        // 1. Calcular la velocidad real del jugador en este frame
        Vector3 movimientoEnEsteFrame = transform.position - posicionPrevia;
        // Ignoramos el movimiento vertical (por si cae o salta)
        movimientoEnEsteFrame.y = 0;

        float velocidadActual = movimientoEnEsteFrame.magnitude / Time.deltaTime;

        // 2. Si se está moviendo lo suficiente, avanzamos el temporizador del paso
        if (velocidadActual > velocidadMinima)
        {
            timerPasos += Time.deltaTime;

            if (timerPasos >= tiempoEntrePasos)
            {
                ReproducirSonidoPaso();
                timerPasos = 0f; // Reiniciar el ritmo
            }
        }
        else
        {
            // Si se detiene, reiniciamos el temporizador para que el siguiente paso suene al instante al caminar
            timerPasos = tiempoEntrePasos;
        }

        // Guardar la posición para el siguiente frame
        posicionPrevia = transform.position;
    }

    void ReproducirSonidoPaso()
    {
        if (audioSource == null || clipsDePasos.Length == 0) return;

        // Elegir un sonido aleatorio de la lista para que suene natural
        int indiceAleatorio = Random.Range(0, clipsDePasos.Length);
        audioSource.clip = clipsDePasos[indiceAleatorio];

        // Añadir una ligera variación de tono (pitch) para que cada paso sea único
        audioSource.pitch = Random.Range(0.85f, 1.15f);

        audioSource.Play();
    }
}