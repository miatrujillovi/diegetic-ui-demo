using UnityEngine;

public class ObjectShaderLogic : MonoBehaviour
{
    [Header("Outline Settings")]
    [SerializeField] private float maxEmissive = 5f;
    [SerializeField] private string emissiveOutline = "Emissive";

    private Renderer rend;
    private Material materialInstance;
    private PickUpObject myObject;

    private void OnEnable()
    {
        PlayerPickDropInteraction.onPickingObject += SetProgress;
    }

    private void OnDisable()
    {
        PlayerPickDropInteraction.onPickingObject -= SetProgress;
    }

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        materialInstance = rend.material;
        myObject = GetComponent<PickUpObject>();
    }

    public void SetProgress(PickUpObject obj, float t)
    {
        if (obj != myObject) return;

        t = Mathf.Clamp01(t);

        float emissive = Mathf.Lerp(0f, maxEmissive, Mathf.SmoothStep(0f, 1f, t));
        materialInstance.SetFloat(emissiveOutline, emissive);
    }

    public void ResetOutline()
    {
        materialInstance.SetFloat(emissiveOutline, 0f);
    }
}
