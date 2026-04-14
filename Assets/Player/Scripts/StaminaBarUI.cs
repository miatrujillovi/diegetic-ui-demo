using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    [Header("Referencias")]
    public FPSController player; // tu script del player
    public Slider staminaSlider;

    void Start()
    {
        // Auto buscar player (por si se te olvida asignarlo)
        if (player == null)
            player = FindFirstObjectByType<FPSController>();
    }

    void Update()
    {
        UpdateStaminaUI();
    }

    void UpdateStaminaUI()
    {
        if (player == null || staminaSlider == null)
        {
            Debug.LogWarning("Faltan referencias en StaminaBarUI");
            return;
        }

        float value = player.currentStamina / player.maxStamina;
        staminaSlider.value = value;
    }
}