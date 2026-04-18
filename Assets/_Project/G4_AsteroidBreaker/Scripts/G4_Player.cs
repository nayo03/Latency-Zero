using UnityEngine;
using UnityEngine.InputSystem;

public class G4_Player : MonoBehaviour
{
    public Camera camaraAR; 
    private G4_GameManager gameManager;

    void Start()
    {
        gameManager = Object.FindAnyObjectByType<G4_GameManager>();
        if (camaraAR == null) camaraAR = Camera.main;
    }

    void Update()
    {
        // 1. Detectar clic de ratón (Para probar en Unity)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            LanzarRayo(Mouse.current.position.ReadValue());
        }
        
        // 2. Detectar toque táctil (Para móvil)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            LanzarRayo(Touchscreen.current.primaryTouch.position.ReadValue());
        }
    }

    private void LanzarRayo(Vector2 screenPos)
    {
        if (camaraAR == null) return;

        Ray ray = camaraAR.ScreenPointToRay(screenPos);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 1f);

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("He tocado algo: " + hit.collider.name);

            if (hit.collider.CompareTag("Item"))
            {
                G4_Asteroide asteroide = hit.collider.GetComponent<G4_Asteroide>();
                if (asteroide != null)
                {
                    if (gameManager != null) gameManager.SumarPuntos(asteroide.puntos);
                    asteroide.Explotar();
                }
            }
        }
        else
        {
            Debug.Log("El rayo no ha tocado nada");
            if (gameManager != null) gameManager.RomperCombo();
        }
    }
}