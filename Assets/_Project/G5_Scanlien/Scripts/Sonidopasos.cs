using UnityEngine;

public class SonidoPasos : MonoBehaviour
{
    [Header("Configuración de Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] clipsDePasos; 

    [Header("Ajustes de Ritmo")]
    [SerializeField] private float tiempoEntrePasos = 0.5f; 
    [SerializeField] private float velocidadMinima = 0.1f;  

    private float timerPasos;
    private Vector3 posicionPrevia;

    void Start()
    {
        posicionPrevia = transform.position;

        
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Vector3 movimientoEnEsteFrame = transform.position - posicionPrevia;
        movimientoEnEsteFrame.y = 0;

        float velocidadActual = movimientoEnEsteFrame.magnitude / Time.deltaTime;

        if (velocidadActual > velocidadMinima)
        {
            timerPasos += Time.deltaTime;

            if (timerPasos >= tiempoEntrePasos)
            {
                ReproducirSonidoPaso();
                timerPasos = 0f; 
            }
        }
        else
        {
            timerPasos = tiempoEntrePasos;
        }

        posicionPrevia = transform.position;
    }

    void ReproducirSonidoPaso()
    {
        if (audioSource == null || clipsDePasos.Length == 0) return;

        int indiceAleatorio = Random.Range(0, clipsDePasos.Length);
        audioSource.clip = clipsDePasos[indiceAleatorio];

        audioSource.pitch = Random.Range(0.85f, 1.15f);

        audioSource.Play();
    }
}