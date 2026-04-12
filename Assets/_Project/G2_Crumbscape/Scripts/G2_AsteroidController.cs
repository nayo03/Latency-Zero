using UnityEngine;

public class G2_AsteroidController : MonoBehaviour
{
    [Header("Movimiento horizontal")]
    public float speedX = 3f;

    [Header("Movimiento vertical")]
    public float speedY = 0f;
    public float verticalRange = 2f;
    public bool hasVerticalMovement = false;

    [Header("Rotación")]
    public float rotationSpeed = 50f;

    private float deactivateAtX = -15f;
    private Vector2 startPosition;
    private Transform visualTransform;

    void Awake()
    {
        visualTransform = transform.Find("VisualAsteroid");
        if (visualTransform == null) visualTransform = transform;
    }

    void OnEnable()
    {
        startPosition = transform.position;
        float currentSpeed = Mathf.Abs(rotationSpeed);
        rotationSpeed = Random.Range(0, 2) == 0 ? currentSpeed : -currentSpeed;
    }

    void Update()
    {
        transform.Translate(Vector2.left * speedX * Time.deltaTime, Space.World);

        if (hasVerticalMovement)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * speedY) * verticalRange;
            transform.position = new Vector2(transform.position.x, newY);
        }

        if (visualTransform != null)
        {
            visualTransform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
        }

        // En lugar de destruir, apagamos para reciclar
        if (transform.position.x <= deactivateAtX)
        {
            gameObject.SetActive(false);
        }
    }
}
