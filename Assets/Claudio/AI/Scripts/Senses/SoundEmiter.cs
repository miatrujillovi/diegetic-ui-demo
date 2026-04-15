using UnityEngine;
using System;

public class SoundEmitter : MonoBehaviour
{
    public static Action<Vector3, float> OnSound;

    public void EmitSound(float intensity)
    {
        OnSound?.Invoke(transform.position, intensity);
    }
}
