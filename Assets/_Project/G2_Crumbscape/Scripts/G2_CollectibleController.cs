using UnityEngine;

public class G2_CollectibleController : MonoBehaviour
{
    [Header("Configuración")]
    public float speedX = 3f;
    public GameObject StarsEffectPrefab;
    private float deactivateAtX = -15f;

    [Header("Puntuación")]
    public int points = 5;
    public bool isCosmicBread = false;

    void Update()
    {
        transform.Translate(Vector2.left * speedX * Time.deltaTime, Space.World);

        if (transform.position.x <= deactivateAtX)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MainManager.Instance.SumarPuntoTemporal(points);

            G2_GameManager gameManager = Object.FindAnyObjectByType<G2_GameManager>();
            if (gameManager != null)
            {
                gameManager.ItemRecogido();
            }

            if (StarsEffectPrefab != null)
            {
                Instantiate(StarsEffectPrefab, transform.position, Quaternion.identity);
            }

            gameObject.SetActive(false); // Lo apagamos
        }
    }
}