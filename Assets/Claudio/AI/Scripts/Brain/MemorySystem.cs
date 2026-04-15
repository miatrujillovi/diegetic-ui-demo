using UnityEngine;

public class MemorySystem : MonoBehaviour
{
    public Vector3 LastKnownPosition { get; private set; }
    public bool HasMemory { get; private set; }

    public void SetLastKnownPosition(Vector3 pos)
    {
        LastKnownPosition = pos;
        HasMemory = true;
    }

    public void ClearMemory()
    {
        HasMemory = false;
    }
}