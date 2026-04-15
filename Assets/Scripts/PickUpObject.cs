using System.Collections;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    [SerializeField] public float pickUpTime = 2f;
    [SerializeField] public float dropTime = 2f;

    private bool isBeingHeld = false;
    private bool isPicking = false;

    private Transform holdPoint; //Transform inside the player on where he holds the object

    public void TryPickUp(Transform _playerHoldPoint)
    {
        if (isBeingHeld || isPicking) return;

        holdPoint = _playerHoldPoint;
        StartCoroutine(PickUpRoutine());
    }

    IEnumerator PickUpRoutine()
    {
        isPicking = true;

        yield return new WaitForSeconds(pickUpTime);

        isBeingHeld = true;

        //Parent Object to Player
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;

        isPicking = false;
    }

    public void TryDrop(DropZone _zone)
    {
        if (isBeingHeld || isPicking) return;

        StartCoroutine(DropRoutine(_zone));
    }

    IEnumerator DropRoutine(DropZone _zone)
    {
        isPicking = true;

        yield return new WaitForSeconds(dropTime);

        isBeingHeld = false;

        transform.SetParent(null);
        transform.position = _zone.dropPoint.position;

        isPicking = false;
    }

    public bool IsHeld()
    {
        return isBeingHeld;
    }
}
