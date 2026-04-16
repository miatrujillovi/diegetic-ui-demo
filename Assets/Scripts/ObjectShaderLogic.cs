using UnityEngine;

public class ObjectShaderLogic : MonoBehaviour
{
    [Header("Outline Settings")]
    [SerializeField] private float maxOutlineWidth = 5f;
    [SerializeField] private string outlineProperty = "_OutlineWidth";

    private Renderer rend;
    private Material materialInstance;

    private float currentProgress = 0f;

    private void Awake()
    {
        rend = GetComponent<Renderer>();

        materialInstance = rend.material;
    }

    public void SetProgress(float t)
    {
        currentProgress = Mathf.Clamp01(t);

        float width = Mathf.Lerp(0f, maxOutlineWidth, currentProgress);
        materialInstance.SetFloat(outlineProperty, width);
    }

    public void ResetOutline()
    {
        currentProgress = 0f;
        materialInstance.SetFloat(outlineProperty, 0f);
    }
}
