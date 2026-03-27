using UnityEngine;

public class DeadlightController : MonoBehaviour
{
    [SerializeField] private GameObject myNeedle;
    [SerializeField] private float speed = 100f;

    
    [System.Serializable] // Serialize the struct to show it in the Unity Inspector
    public struct SuccessZone
    {
        public string name;
        public float minAngle;
        public float maxAngle;
    };

    [SerializeField] private SuccessZone perfectZone;
    [SerializeField] private SuccessZone goodZone;
    
    private float angle = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Rotation calculation for the needle
        float rotationThisFrame = speed * Time.deltaTime;

        // Add the rotation to the needle
        myNeedle.transform.Rotate(Vector3.forward, rotationThisFrame);

        // Update the angle variable to keep track of the current rotation
        angle += rotationThisFrame;
        angle %= 360f; // Keep the angle between 0 and 360 degrees

        //Read user input to stop the needle rotation
        if (Input.GetKeyDown(KeyCode.Space))
        {
            speed = 0f; // Stop the needle rotation
            CheckResult(); // Check the result based on the current angle
        }
    }

    private void CheckResult()
    {
        if (angle >= perfectZone.minAngle && angle <= perfectZone.maxAngle)
        {
            Debug.Log("Perfect!");
        }
        else if (angle >= goodZone.minAngle && angle <= goodZone.maxAngle)
        {
            Debug.Log("Good!");
        }
        else
        {
            Debug.Log("Bad!");
        }

    }
}
