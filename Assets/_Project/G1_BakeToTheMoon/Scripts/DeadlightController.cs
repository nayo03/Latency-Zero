using UnityEngine;
using System.Collections;
using System;
using UnityEngine.InputSystem;

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

    [Header("Ajustes de Tiempo")]
    [SerializeField] private float timeShowResult = 1.0f;
    [SerializeField] private float timePreparation = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialSpeed = speed; // Store the initial speed value

    }

    // Update is called once per frame
    void Update()
    {
        if (!gameActive) return;

        if (isMoving)
        {
            //Rotation calculation for the needle
            float rotationThisFrame = speed * Time.deltaTime;

            // Add the rotation to the needle
            myNeedle.transform.Rotate(Vector3.back, rotationThisFrame);

            // Update the angle variable to keep track of the current rotation
            angle += rotationThisFrame;
            angle %= 360f; // Keep the angle between 0 and 360 degrees

            bool inputDetected = false;

            //Read user input to stop the needle rotation
            if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
                inputDetected = true;

            if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
                inputDetected = true;

            // Start the coroutine to handle the result of the player's attempt
            if (inputDetected)
            {
                StartCoroutine(ResultRoutine());
            }
        }
        
    }
    // Coroutine to handle the result of the player's attempt, including stopping the needle, checking the result, and resetting for the next attempt
    private IEnumerator ResultRoutine()
    {
        speed = 0f;
        isMoving = false;

        CheckResult();

        yield return new WaitForSeconds(timeShowResult);

        angle = 0f;
        myNeedle.transform.localRotation = Quaternion.identity; // Reset the needle's rotation

        yield return new WaitForSeconds(timePreparation);

        speed = initialSpeed; // Reset the speed to its initial value
        isMoving = true;
    }

    // Method to check the result of the player's attempt and calculate the punctuation based on the angle of the needle
    private void CheckResult()
    {
        int punctuation = 0;

        if (angle >= perfectZone.minAngle && angle <= perfectZone.maxAngle)
        {
            punctuation += 100;

        }
        else if (angle >= goodZone.minAngle && angle <= goodZone.maxAngle)
        {
            punctuation += 50;
        }
        else
        {
            punctuation -= 75;
        }
        Debug.Log(punctuation);

        OnTryComplete?.Invoke(punctuation);

    }

    // Method to switch off the needle, stopping its movement and preventing further attempts until reactivated
    public void SwitchOffNeedle()
    {
        gameActive = false;
        speed = 0f;
    }

    // Method to switch on the needle, allowing it to move and enabling player attempts
    public void switchDifficulty(float newSpeed, float minP, float maxP, float minG, float maxG)
    {
        speed = newSpeed;
        initialSpeed = newSpeed;

        perfectZone.minAngle = minP;
        perfectZone.maxAngle = maxP;
        goodZone.minAngle = minG;
        goodZone.maxAngle = maxG;

        Debug.Log("Dificultad actualizada: " + newSpeed + " | Perfect Zone: " + perfectZone.minAngle + " - " + perfectZone.maxAngle + " | Good Zone: " + goodZone.minAngle + " - " + goodZone.maxAngle);

    }
}
