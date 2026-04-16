using System.Collections;
using UnityEngine;

public class PlayerPickDropInteraction : MonoBehaviour
{
    [SerializeField] private Transform holdPoint;

    private PickUpObject currentObject;
    private DropZone currentZone;

    private bool isHoldingObject = false;

    private Coroutine actionRoutine;

    //DEBUGGING PURPOSES--------------------------------------
    [Header("Debugging")]
    [SerializeField] private bool activateDebugs;
    private string debugName = "[PickDrop]";

    private void OnTriggerEnter(Collider other)
    {
        PickUpObject obj = other.GetComponent<PickUpObject>();
        DropZone zone = other.GetComponent<DropZone>();

        if (obj == null && zone == null) return;

        DebugManager.instance.Log($"Entered trigger: {other.name}", activateDebugs, debugName);

        if (obj != null)
        {
            DebugManager.instance.Log($"Found pickup object: {other.name}", activateDebugs, debugName);
            currentObject = obj;
            StartPickUp();
        }

        if (zone != null)
        {
            DebugManager.instance.Log($"Entered drop zone: {other.name}", activateDebugs, debugName);
            currentZone = zone;
            StartDrop();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        DebugManager.instance.Log($"Exited trigger: {other.name}", activateDebugs, debugName);

        if (other.GetComponent<PickUpObject>() != null)
        {
            DebugManager.instance.Log("Left pickup object range... Canceling action", activateDebugs, debugName);

            if (isHoldingObject) return;

            CancelAction();
            currentObject = null;
        }

        if (other.GetComponent<DropZone>() != null)
        {
            DebugManager.instance.Log("Left drop zone.. canceling action", activateDebugs, debugName);
            CancelAction();
            currentZone = null;
        }
    }

    private void StartPickUp()
    {
        if (currentObject == null)
        {
            DebugManager.instance.LogError("StartPickUp failed: no current object", activateDebugs, debugName);
            return;
        }

        if (currentObject.IsHeld())
        {
            DebugManager.instance.Log("StartPickUp aborted: object already held", activateDebugs, debugName);
            return;
        }

        DebugManager.instance.Log($"Starting pickup: {currentObject.name}", activateDebugs, debugName);

        CancelAction();
        actionRoutine = StartCoroutine(PickUpAfterDelay());
    }

    private void StartDrop()
    {
        if (currentObject == null)
        {
            DebugManager.instance.LogError("StartDrop failed: no current object", activateDebugs, debugName);
            return;
        }

        if (currentZone == null)
        {
            DebugManager.instance.LogError("StartDrop failed: no drop zone", activateDebugs, debugName);
            return;
        }

        if (!currentObject.IsHeld())
        {
            DebugManager.instance.Log("StartDrop aborted: object not held", activateDebugs, debugName);
            return;
        }

        DebugManager.instance.Log($"Starting drop into: {currentZone.name}", activateDebugs, debugName);

        CancelAction();
        actionRoutine = StartCoroutine(DropAfterDelay());
    }

    IEnumerator PickUpAfterDelay()
    {
        DebugManager.instance.Log($"Pickup coroutine started ({currentObject.pickUpTime}s)", activateDebugs, debugName);

        yield return new WaitForSeconds(currentObject.pickUpTime);

        DebugManager.instance.Log("Pickup completed", activateDebugs, debugName);
        isHoldingObject = true;
        currentObject.TryPickUp(holdPoint);
    }

    IEnumerator DropAfterDelay()
    {
        DebugManager.instance.Log($"Drop coroutine started ({currentObject.dropTime}s)", activateDebugs, debugName);

        yield return new WaitForSeconds(currentObject.dropTime);

        DebugManager.instance.Log("Drop completed", activateDebugs, debugName);
        isHoldingObject = false;
        currentObject.TryDrop(currentZone);
    }

    void CancelAction()
    {
        if (actionRoutine != null)
        {
            DebugManager.instance.Log("Canceling current action", activateDebugs, debugName);

            StopCoroutine(actionRoutine);
            actionRoutine = null;
        }
    }
}
