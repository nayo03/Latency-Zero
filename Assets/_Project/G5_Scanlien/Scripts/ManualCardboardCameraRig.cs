using UnityEngine;

public class ManualCardboardCameraRig : MonoBehaviour
{
    [Header("Cámaras")]
    public Camera leftEye;
    public Camera rightEye;

    [Header("Ajustes VR Manual")]
    public float separacionOjos = 0.03f;
    public float alturaCamara = 1.7f;

    [Header("Ajuste de orientación")]
    public float correccionYaw = 0f;
    public float correccionPitch = 90f;
    public float correccionRoll = 0f;

    private bool gyroDisponible;
    private Quaternion rotacionInicial;

    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        ConfigurarCamaras();

        gyroDisponible = SystemInfo.supportsGyroscope;

        if (gyroDisponible)
        {
            Input.gyro.enabled = true;
            rotacionInicial = transform.rotation;
        }
        else
        {
            Debug.LogWarning("G5 VR Manual: giroscopio no disponible.");
        }
    }

    void Update()
    {
        if (!gyroDisponible) return;

        Quaternion rotacionGyro = GyroToUnity(Input.gyro.attitude);

        Quaternion correccion = Quaternion.Euler(correccionPitch, correccionYaw, correccionRoll);

        transform.rotation = rotacionInicial * correccion * rotacionGyro;
    }

    private void ConfigurarCamaras()
    {
        if (leftEye != null)
        {
            leftEye.rect = new Rect(0f, 0f, 0.5f, 1f);
            leftEye.transform.localPosition = new Vector3(-separacionOjos / 2f, alturaCamara, 0f);
            leftEye.transform.localRotation = Quaternion.identity;
            leftEye.stereoTargetEye = StereoTargetEyeMask.None;
            leftEye.fieldOfView = 70f;
        }

        if (rightEye != null)
        {
            rightEye.rect = new Rect(0.5f, 0f, 0.5f, 1f);
            rightEye.transform.localPosition = new Vector3(separacionOjos / 2f, alturaCamara, 0f);
            rightEye.transform.localRotation = Quaternion.identity;
            rightEye.stereoTargetEye = StereoTargetEyeMask.None;
            rightEye.fieldOfView = 70f;
        }
    }

    private Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }
}