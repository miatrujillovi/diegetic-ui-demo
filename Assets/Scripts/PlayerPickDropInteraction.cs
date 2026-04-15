using System.Collections;
using UnityEngine;

public class PlayerPickDropInteraction : MonoBehaviour
{
    [SerializeField] private Transform holdPoint;

    private PickUpObject currentObject;
    private DropZone currentZone;

    private Coroutine actionRoutine;

    private void OnTriggerEnter(Collider other)
    {
        PickUpObject obj = other.GetComponent<PickUpObject>();
        if (obj != null)
        {
            currentObject = obj;
            StartPickUp();
        }

        DropZone zone = other.GetComponent<DropZone>();
        if (zone != null)
        {
            currentZone = zone;
            StartDrop();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PickUpObject>() != null)
        {
            CancelAction();
            currentObject = null;
        }

        if (other.GetComponent<DropZone>() != null)
        {
            CancelAction();
            currentZone = null;
        }
    }

    private void StartPickUp()
    {
        if (currentObject == null || currentObject.IsHeld()) return;

        CancelAction();
        actionRoutine = StartCoroutine(PickUpAfterDelay());
    }

    private void StartDrop()
    {
        if (currentObject == null || !currentObject.IsHeld() || currentZone == null) return;

        CancelAction();
        actionRoutine = StartCoroutine(DropAfterDelay());
    }

    IEnumerator PickUpAfterDelay()
    {
        yield return new WaitForSeconds(currentObject.pickUpTime);
        currentObject.TryPickUp(holdPoint);
    }

    IEnumerator DropAfterDelay()
    {
        yield return new WaitForSeconds(currentObject.dropTime);
        currentObject.TryDrop(currentZone);
    }

    void CancelAction()
    {
        if (actionRoutine != null)
        {
            StartCoroutine("actionRoutine");
            actionRoutine = null;
        }
    }
}
