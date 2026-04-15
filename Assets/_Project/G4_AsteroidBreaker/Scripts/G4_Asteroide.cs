using UnityEngine;

public class G4_Asteroide : MonoBehaviour
{
    public int puntos = 10; 
    public float tiempoDeVida = 6f; // Un poco más de tiempo porque ahora se mueven
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
        // Aquí se podra instanciar un sistema de partículas de explosión
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