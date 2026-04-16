using System.Collections.Generic;
using UnityEngine;

public class ObjectZoneManager : MonoBehaviour
{
    //DEBUGGING PURPOSES--------------------------------------
    [Header("Debugging")]
    [SerializeField] private bool activateDebugs;
    private string debugName = "[RitualManager]";

    private HashSet<RitualObjects> collectedObjects = new HashSet<RitualObjects>(); //HashSet doesn't allow duplicates

    public enum RitualObjects 
    { 
        Light1,
        Light2,
        Light3,
        Light4,
        Bible,
        Sword
    }

    private void OnEnable()
    {
        PickUpObject.OnObjectDropped += AddObjectToRitual;
    }

    private void OnDisable()
    {
        PickUpObject.OnObjectDropped -= AddObjectToRitual;
    }

    private void AddObjectToRitual(RitualObjects _object)
    {
        if (!collectedObjects.Contains(_object))
        {
            collectedObjects.Add(_object);
            DebugManager.instance.Log($"Added {_object} to ritual", activateDebugs, debugName);
        }

        CheckRitualCompletion();
    }

    private void CheckRitualCompletion()
    {
        if (collectedObjects.Count == System.Enum.GetValues(typeof(RitualObjects)).Length)
        {
            DebugManager.instance.Log("Ritual Completed!!", activateDebugs, debugName);
        }
    }
}
