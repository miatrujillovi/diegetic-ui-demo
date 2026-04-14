using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    public PatrolPoint[] points;

    public PatrolPoint GetPoint(int index)
    {
        return points[index];
    }

    public int Length => points.Length;
}