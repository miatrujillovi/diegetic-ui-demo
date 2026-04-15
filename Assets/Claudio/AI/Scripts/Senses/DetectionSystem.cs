using UnityEngine;

public class DetectionSystem : MonoBehaviour
{
    [Header("Valores")]
    public float detection = 0f;
    public float maxDetection = 100f;

    [Header("Velocidades")]
    public float increaseClose = 60f;   // ?? rápido
    public float increaseFar = 25f;     // ?? lento
    public float decreaseRate = 20f;    // ?? baja con el tiempo

    public bool IsFullyDetected => detection >= maxDetection;
    public bool IsSuspicious => detection > 0f;

    public void IncreaseDetection(bool isClose)
    {
        float amount = isClose ? increaseClose : increaseFar;
        detection += amount * Time.deltaTime;
        detection = Mathf.Clamp(detection, 0, maxDetection);
    }

    public void DecreaseDetection()
    {
        detection -= decreaseRate * Time.deltaTime;
        detection = Mathf.Clamp(detection, 0, maxDetection);
    }

    public void ResetDetection()
    {
        detection = 0;
    }
}
