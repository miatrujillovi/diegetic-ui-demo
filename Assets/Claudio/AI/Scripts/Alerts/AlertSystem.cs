using UnityEngine;
using System;

public enum AlertLevel
{
    Normal,
    Suspicious,
    Combat
}

public class AlertSystem : MonoBehaviour
{
    public AlertLevel currentLevel;

    public static Action<Transform, AlertLevel> OnAlertRaised;

    public void SetAlertLevel(AlertLevel newLevel)
    {
        if (newLevel > currentLevel)
        {
            currentLevel = newLevel;
            OnAlertRaised?.Invoke(transform, newLevel);
        }
    }

    public static void RaiseAlert(Transform source, AlertLevel level)
    {
        OnAlertRaised?.Invoke(source, level);
    }
}