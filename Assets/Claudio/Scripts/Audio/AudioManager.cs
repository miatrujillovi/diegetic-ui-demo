using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Reproduce un sonido 2D (UI, música, susurros en la cabeza)
    public void PlaySound2D(AudioData data)
    {
        GameObject obj = new GameObject("Sound2D_" + data.name);
        AudioSource source = obj.AddComponent<AudioSource>();
        ConfigurarSource(source, data);
        source.spatialBlend = 0; // 2D
        source.Play();
        Destroy(obj, data.GetRandomClip().length + 0.1f);
    }

    // Reproduce un sonido 3D (Ventanas, puertas, pasos)
    public void PlaySound3D(AudioData data, Vector3 posicion)
    {
        GameObject obj = new GameObject("Sound3D_" + data.name);
        obj.transform.position = posicion;
        AudioSource source = obj.AddComponent<AudioSource>();
        ConfigurarSource(source, data);
        source.spatialBlend = 1; // 3D
        source.minDistance = 2f;
        source.maxDistance = 15f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.Play();
        Destroy(obj, data.GetRandomClip().length + 0.1f);
    }

    private void ConfigurarSource(AudioSource source, AudioData data)
    {
        source.clip = data.GetRandomClip();
        source.volume = data.volumen;
        source.pitch = Random.Range(data.pitchMin, data.pitchMax);
    }
}

