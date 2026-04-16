using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class PlayerPickDropInteraction : MonoBehaviour
{
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float timerDecay = 0.5f;

    //OBJECT LOGIC VARIABLES-----------------------------
    private PickUpObject currentObject;
    private DropZone currentZone;

    private bool isHoldingObject = false;

    //TIMER LOGIC VARIABLES--------------------------------
    //private UniTask actionRoutine;
    private float progress = 0f;
    private float targetTime = 0f;

    private bool isActive = false;
    private bool isReversing = false;

    private CancellationTokenSource cts;

    //DEBUGGING PURPOSES--------------------------------------
    [Header("Debugging")]
    [SerializeField] private bool activateDebugs;
    private string debugName = "[PickDrop]";

    #region Triggers
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

    #endregion Triggers

    #region StartFunctions

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

        targetTime = currentObject.pickUpTime;
        isActive = true;
        isReversing = false;

        StartLoopIfNeeded();
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

        targetTime = currentObject.dropTime;
        isActive = true;
        isReversing = false;

        StartLoopIfNeeded();
    }

    #endregion StartFunctions

    #region TimerControl

    async UniTaskVoid ActionLoop()
    {
        cts = new CancellationTokenSource();

        try
        {
            while (!cts.Token.IsCancellationRequested)
            {
                float dt = Time.deltaTime;

                if (isActive)
                {
                    progress += dt;
                } 
                else if (isReversing)
                {
                    progress -= dt * timerDecay;
                }

                progress = Mathf.Clamp(progress, 0f, targetTime);

                if (progress >= targetTime)
                {
                    CompleteAction();
                    progress = 0f;
                    isActive = false;
                }

                await UniTask.Yield(cts.Token);
            }
        }
        catch (OperationCanceledException) { }
    }

    /*async UniTask PickUpAfterDelay()
    {
        DebugManager.instance.Log($"Pickup coroutine started ({currentObject.pickUpTime}s)", activateDebugs, debugName);

        await UniTask.Delay(TimeSpan.FromSeconds(currentObject.pickUpTime));

        DebugManager.instance.Log("Pickup completed", activateDebugs, debugName);
        isHoldingObject = true;
        currentObject.TryPickUp(holdPoint);
    }

    async UniTask DropAfterDelay()
    {
        DebugManager.instance.Log($"Drop coroutine started ({currentObject.dropTime}s)", activateDebugs, debugName);

        await UniTask.Delay(TimeSpan.FromSeconds(currentObject.dropTime));

        DebugManager.instance.Log("Drop completed", activateDebugs, debugName);
        isHoldingObject = false;
        currentObject.TryDrop(currentZone);
    }*/

    void CancelAction()
    {
        isActive = false;
        isReversing = true;
    }

    void CompleteAction()
    {
        if (!isHoldingObject)
        {
            isHoldingObject = true;
            currentObject.TryPickUp(holdPoint);
        }
        else
        {
            isHoldingObject = false;
            currentObject.TryDrop(currentZone);
        }
    }

    void StartLoopIfNeeded()
    {
        if (cts == null || cts.IsCancellationRequested)
        {
            ActionLoop().Forget();
        }
    }

    void StopAll()
    {
        cts?.Cancel();
        cts = null;

        progress = 0f;
        isActive = false;
        isReversing = false;
    }

    #endregion TimerControl
}
