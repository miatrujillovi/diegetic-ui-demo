using UnityEngine;

public class ObjectShaderLogic : MonoBehaviour
{
    [Header("Outline Settings")]
    [SerializeField] private float baseEmissive = 1f;
    [SerializeField] private float maxEmissive = 7f;
    [SerializeField] private string emissiveOutline = "_Emissive";
    [SerializeField] private Renderer rend;

    //private Renderer rend;
    private MaterialPropertyBlock propBlock;
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
        propBlock = new MaterialPropertyBlock();
        myObject = GetComponent<PickUpObject>();
    }

    public void SetProgress(PickUpObject obj, float t)
    {
        if (obj != myObject) return;

        t = Mathf.Clamp01(t);

        float emissive = Mathf.Lerp(baseEmissive, maxEmissive, t);

        rend.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Emissive", emissive);
        rend.SetPropertyBlock(propBlock);
    }

    public void ResetOutline()
    {
        rend.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_Emissive", baseEmissive);
        rend.SetPropertyBlock(propBlock);
    }
}
