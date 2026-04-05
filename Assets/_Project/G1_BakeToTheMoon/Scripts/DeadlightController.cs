using UnityEngine;
using System.Collections;
using System;

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
    private bool isMoving = true;
    private float initialSpeed;
    public static event Action<int> OnTryComplete; // Event to notify when the player tries to complete the action, passing the punctuation as an argument
    private bool gameActive = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialSpeed = speed; // Store the initial speed value

    }

    // Update is called once per frame
    void Update()
    {
        if (!gameActive) return;

        //Rotation calculation for the needle
        float rotationThisFrame = speed * Time.deltaTime;

        // Add the rotation to the needle
        myNeedle.transform.Rotate(Vector3.forward, rotationThisFrame);

        // Update the angle variable to keep track of the current rotation
        angle += rotationThisFrame;
        angle %= 360f; // Keep the angle between 0 and 360 degrees

        //Read user input to stop the needle rotation
        if (Input.GetKeyDown(KeyCode.Space) && isMoving)
        {
            //Corroutine which 
            StartCoroutine(ResultRoutine());
            
        }
    }
    private IEnumerator ResultRoutine()
    {
        speed = 0f;
        isMoving = false;

        CheckResult();

        yield return new WaitForSeconds(1.5f);

        angle = 0f;
        myNeedle.transform.localRotation = Quaternion.identity; // Reset the needle's rotation

        speed = initialSpeed; // Reset the speed to its initial value
        isMoving = true;
    }

    private void CheckResult()
    {
        int punctuation = 0;

        if (angle >= perfectZone.minAngle && angle <= perfectZone.maxAngle)
        {
            punctuation += 200;

        }
        else if (angle >= goodZone.minAngle && angle <= goodZone.maxAngle)
        {
            punctuation += 100;
        }
        else
        {
            punctuation -= 150;
        }
        Debug.Log(punctuation);

        OnTryComplete?.Invoke(punctuation);

    }

    public void SwitchOffNeedle()
    {
        gameActive = false;
        speed = 0f;
    }
}
