using UnityEngine;

public class PulseAnimation : MonoBehaviour
{
    public float speed = 6f;
    public float scaleMultiplier = 1.5f;

    private Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        float scale = 1 + Mathf.Sin(Time.time * speed) * (scaleMultiplier - 1);
        transform.localScale = baseScale * scale;
    }
}