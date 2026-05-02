using UnityEngine;

public class AmbientEmitter : MonoBehaviour
{
    [Header("Configuración del Sonido")]
    public AudioData audioData;
    public bool playOnAwake = true;
    public bool is3D = false; // ¿Es un sonido general o viene de un punto (como la tele)?

    private AudioSource source;

    void Start()
    {
        if (playOnAwake) Play();
    }

    public void Play()
    {
        if (audioData == null) return;

        source = gameObject.AddComponent<AudioSource>();
        source.clip = audioData.GetRandomClip();
        source.volume = audioData.volumen;
        source.pitch = Random.Range(audioData.pitchMin, audioData.pitchMax);

        source.loop = true;
        source.spatialBlend = is3D ? 1f : 0f; // 1 = 3D (tele), 0 = 2D (viento)

        // Configuramos para que no sea molesto si es 3D
        if (is3D)
        {
            source.minDistance = 1f;
            source.maxDistance = 10f;
            source.rolloffMode = AudioRolloffMode.Linear;
        }

        source.Play();
    }
}