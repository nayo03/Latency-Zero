using UnityEngine;

public class G3_BackgroundFit : MonoBehaviour
{
    void Start()
    {
        AjustarTamaño();
    }

    void AjustarTamaño()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;

        // Tamaño del mundo visible por la cámara
        float alturaMundo = Camera.main.orthographicSize * 2f;
        float anchoMundo = alturaMundo * Camera.main.aspect;

        // Tamaño del sprite
        float anchoSprite = sr.sprite.bounds.size.x;
        float altoSprite = sr.sprite.bounds.size.y;

        // Calculamos qué escala necesita para cubrir TODA la pantalla
        // Usamos el mayor de los dos para que no queden franjas
        float escalaX = anchoMundo / anchoSprite;
        float escalaY = alturaMundo / altoSprite;
        float escala = Mathf.Max(escalaX, escalaY);

        // Aplicamos la misma escala en X e Y para no deformarlo
        transform.localScale = new Vector3(escala, escala, 1f);
    }
}