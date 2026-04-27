using UnityEngine;
using DG.Tweening;

public class ObjectShakeLogic : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float baseShakeIntensity = 1f;
    [SerializeField] private float maxShakeIntensity = 7f;
    [SerializeField] private float shakeDuration = 0.1f;

    private PickUpObject myObject;
    private Tween shakeTween;
    private Vector3 startLocalPos;

    private void OnEnable()
    {
        PlayerPickDropInteraction.onPickingObject += SetShake;
    }

    private void OnDisable()
    {
        PlayerPickDropInteraction.onPickingObject -= SetShake;
    }

    private void Awake()
    {
        myObject = GetComponent<PickUpObject>();
        startLocalPos = transform.localPosition;
    }

    private void SetShake(PickUpObject obj, float t)
    {
        if (obj != myObject) return;

        if (myObject.IsHeld())
        {
            ResetShake();
            return;
        }

        t = Mathf.Clamp01(t);

        float intensity = Mathf.Lerp(baseShakeIntensity, maxShakeIntensity, t);

        StartShake(intensity);
    }

    private void StartShake(float intensity)
    {
        shakeTween?.Kill();

        shakeTween = DOTween.To(
            () => 0f,
            x =>
            {
                float time = Time.time * 30f;
                float offsetX = Mathf.PerlinNoise(time, 0f) - 0.5f;
                float offsetY = Mathf.PerlinNoise(0f, time) - 0.5f;
                float offsetZ = Mathf.PerlinNoise(time, time) - 0.5f;

                transform.localPosition = startLocalPos + new Vector3(offsetX, offsetY, offsetZ) * intensity;
            },
            1f,
            shakeDuration
        )
        .SetLoops(-1, LoopType.Restart);
    }

    private void ResetShake()
    {
        shakeTween?.Kill();
        //transform.localPosition = startLocalPos;
    }

    private void OnTransformParentChanged()
    {
        shakeTween?.Kill();

        transform.localPosition = Vector3.zero;
        startLocalPos = transform.localPosition;
    }
}
