using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class PlayerPickDropInteraction : MonoBehaviour
{
    [Header("Player Object Interaction")]
    [SerializeField, Tooltip("Transform inside Player where the object is to be placed")] private Transform holdPoint;
    [SerializeField, Tooltip("Multiplier of how fast the timer will decay outside of the trigger zone of the object")] private float timerDecay = 0.5f;
    [Space]

    //OBJECT LOGIC VARIABLES-----------------------------------------------------------
    private PickUpObject currentObject;
    private DropZone currentZone;

    private bool isHoldingObject = false;

    //TIMER LOGIC VARIABLES------------------------------------------------------------
    private float progress = 0f;
    private float targetTime = 0f;

    private bool isActive = false;
    private bool isReversing = false;

    private CancellationTokenSource cts;

    //DEBUGGING PURPOSES---------------------------------------------------------------
    [Header("Debugging")]
    [SerializeField, Tooltip("Should Debug.Logs of this script show on console?")] private bool activateDebugs;
    private string debugName = "[PickDrop]";

    //OnEnterTrigger and OnExitTrigger Logic-------------------------------------------
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

    //Functions to Verify and Start the Pick and Drop of Objects-----------------------
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

    //Async functions that control the timing of the Picks and Drops-------------------
    #region TimerControl

    //Main Timer Function
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

                DebugManager.instance.Log(progress.ToString(), activateDebugs, debugName);

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

    //Stars reducing time on the timer
    void CancelAction()
    {
        isActive = false;
        isReversing = true;
    }

    //Puts 0 on the main timer after completion
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

    //Starts the loop of the new timer.
    void StartLoopIfNeeded()
    {
        if (cts == null || cts.IsCancellationRequested)
        {
            ActionLoop().Forget();
        }
    }

    //Stops all timers.
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
