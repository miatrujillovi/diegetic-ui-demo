using System;
using UnityEngine;

public class PickUpObject : MonoBehaviour
{
    [Header("Pick Up Characteristics")]
    [SerializeField, Tooltip("How much does it take to pick up the object?")] public float pickUpTime = 2f;
    [SerializeField, Tooltip("How much does it take to drop the object?")] public float dropTime = 2f;
    [SerializeField, Tooltip("Type of object")] public ObjectZoneManager.RitualObjects ritualObjectType;

    private Transform selectedDropPoint;

    private bool isPlaced = false;

    private bool isBeingHeld = false;

    private Transform holdPoint; //Transform inside the player on where he holds the object

    public static Action<ObjectZoneManager.RitualObjects> OnObjectDropped; //Suscriptions: ObjectZoneManager.cs

    #region PickUps
    public void TryPickUp(Transform _playerHoldPoint)
    {
        if (isBeingHeld || isPlaced) return;

        holdPoint = _playerHoldPoint;
        PickUpRoutine();
    }

    private void PickUpRoutine()
    {
        isBeingHeld = true;

        //Parent Object to Player
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;

    }

    #endregion PickUps

    #region Drops
    public void TryDrop(DropZone _zone)
    {
        if (!isBeingHeld) return;

        DropRoutine(_zone);
    }

    private void DropRoutine(DropZone _zone)
    {
        isBeingHeld = false;

        SelectDropPoint(_zone);

        transform.SetParent(null);
        transform.position = selectedDropPoint.position;

        isPlaced = true;
        gameObject.GetComponent<BoxCollider>().enabled = false;

        OnObjectDropped?.Invoke(ritualObjectType);
    }
    #endregion Drops

    //From the currentObject, selects corresponding area to locate
    private void SelectDropPoint(DropZone _zone)
    {
        switch (ritualObjectType)
        {
            case ObjectZoneManager.RitualObjects.Light1:
                selectedDropPoint = _zone.light1DropPoint;
                break;

            case ObjectZoneManager.RitualObjects.Light2:
                selectedDropPoint = _zone.light2DropPoint;
                break;

            case ObjectZoneManager.RitualObjects.Light3:
                selectedDropPoint = _zone.light3DropPoint;
                break;

            case ObjectZoneManager.RitualObjects.Light4:
                selectedDropPoint = _zone.light4DropPoint;
                break;

            case ObjectZoneManager.RitualObjects.Bible:
                selectedDropPoint = _zone.bibleDropPoint;
                break;

            case ObjectZoneManager.RitualObjects.Sword:
                selectedDropPoint = _zone.swordDropPoint;
                break;

            default:
                selectedDropPoint = null;
                break;
        }
    }

    //Is an Object being held? Verification
    public bool IsHeld()
    {
        return isBeingHeld;
    }
}
