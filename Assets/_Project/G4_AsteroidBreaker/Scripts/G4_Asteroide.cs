using UnityEngine;

public class G4_Asteroide : MonoBehaviour
{
    [Header("Configuración del Asteroide")]
    public int puntos = 10; 
    public float tiempoDeVida = 10f; 
    public float velocidadMin = 1f;
    public float velocidadMax = 3f;

    private Vector3 direccionMovimiento;
    private float velocidadAleatoria;

    void Start()
    {
        // 1. Elegimos una dirección aleatoria para que se mueva
        direccionMovimiento = Random.onUnitSphere;
        
        // 2. Elegimos una velocidad aleatoria
        velocidadAleatoria = Random.Range(velocidadMin, velocidadMax);

        // 3. Se destruye solo tras X segundos para no llenar la memoria
        Destroy(gameObject, tiempoDeVida);
    }

    void Update()
    {
        // Movimiento constante en la dirección elegida
        transform.Translate(direccionMovimiento * velocidadAleatoria * Time.deltaTime, Space.World);
        
        // Hacer que rote sobre sí mismo para que quede más realista
        transform.Rotate(Vector3.up * 50f * Time.deltaTime);
    }

    public void Explotar()
    {
        // Buscamos el objeto AudioManager en la escena
        GameObject audioManagerObj = GameObject.Find("AudioManager");
        
        if (audioManagerObj != null)
        {
            // Cogemos los dos componentes Audio Source del objeto
            AudioSource[] canales = audioManagerObj.GetComponents<AudioSource>();
            
            
            if (canales.Length > 1 && canales[1] != null)
            {
                canales[1].Play();
            }
        }

        // Aquí se podrá instanciar un sistema de partículas de explosión en el futuro
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        // Rompe el combo si el juego sigue activo al destruirse el asteroide por tiempo
        if (gameObject.scene.isLoaded)
        {
            G4_GameManager gm = Object.FindAnyObjectByType<G4_GameManager>();
            if (gm != null) gm.RomperCombo();
        }
    }
}