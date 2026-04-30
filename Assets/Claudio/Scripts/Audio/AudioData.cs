using UnityEngine;

[CreateAssetMenu(fileName = "NuevoSonido", menuName = "Ritual Night/Audio/AudioData")]
public class AudioData : ScriptableObject
{
    public AudioClip[] clips;
    [Range(0, 1)] public float volumen = 1f;
    [Range(0.1f, 2f)] public float pitchMin = 0.9f;
    [Range(0.1f, 2f)] public float pitchMax = 1.1f;

    [Header("Configuración Espacial")]
    public bool es3D = true; // Si es falso, será 2D (sonido global)
    public float distanciaMinima = 1f;
    public float distanciaMaxima = 15f;

    public AudioClip GetRandomClip()
    {
        if (clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }
}