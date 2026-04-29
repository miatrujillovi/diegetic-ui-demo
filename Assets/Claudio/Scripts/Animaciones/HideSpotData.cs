using UnityEngine;

[CreateAssetMenu(fileName = "NuevoEscondite", menuName = "Ritual Night/Datos de Escondite")]
public class HideSpotData : ScriptableObject
{
    public string nombreEscondite;
    public string triggerAnimacion; // El nombre exacto en el Animator
    public string triggerSalida = "Exit_Hide";
}