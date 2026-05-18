using UnityEngine;

public class G4_Asteroide : MonoBehaviour
{
    [Header("Configuración del Asteroide")]
    public int puntos = 10; 
    public float tiempoDeVida = 10f; 
    public float velocidadMin = 1f;
    public float velocidadMax = 3f;

    [Header("Audio")]
    [Tooltip("explosión")]
    public AudioClip clipExplosion;

    private Vector3 direccionMovimiento;
    private float velocidadAleatoria;

    void Start()
    {
        direccionMovimiento = Random.onUnitSphere;
        velocidadAleatoria = Random.Range(velocidadMin, velocidadMax);
        Destroy(gameObject, tiempoDeVida);
    }

    void Update()
    {
        transform.Translate(direccionMovimiento * velocidadAleatoria * Time.deltaTime, Space.World);
        transform.Rotate(Vector3.up * 50f * Time.deltaTime);
    }

    public void Explotar()
    {
        // CHIVATO 1: Para saber si el juego detecta que has tocado el asteroide
        Debug.Log("¡ENTRANDO EN EXPLOOTAR! El juego ha detectado la destrucción del asteroide.");

        if (clipExplosion != null)
        {
            // CHIVATO 2: Para confirmar que el archivo de audio está bien cargado en la casilla
            Debug.Log("Sonido detectado con éxito: " + clipExplosion.name);

            GameObject altavozTemporal = new GameObject("Altavoz_Explosion");
            AudioSource sourceTemporal = altavozTemporal.AddComponent<AudioSource>();
        
            sourceTemporal.clip = clipExplosion;
            sourceTemporal.spatialBlend = 0f; 
            sourceTemporal.volume = 1f; 
        
            sourceTemporal.Play();
        
            // CHIVATO 3: Para confirmar que el altavoz virtual le ha dado al Play
            Debug.Log("¡Altavoz creado y sonido reproduciéndose en 2D!");

            Destroy(altavozTemporal, clipExplosion.length);
        }
        else
        {
            // CHIVATO ERROR: Esto brillará en amarillo si se te olvidó arrastrar el sonido al Prefab
            Debug.LogWarning("¡Alerta! 'clipExplosion' está VACÍO en el Inspector de este asteroide.");
        }

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (gameObject.scene.isLoaded)
        {
            G4_GameManager gm = Object.FindAnyObjectByType<G4_GameManager>();
            if (gm != null) gm.RomperCombo();
        }
    }
}