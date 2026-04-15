using UnityEngine;

public class HearingSensor : MonoBehaviour
{
    public float baseRange = 10f;
    public Vector3 LastHeardPosition { get; private set; }

    private bool heard;

    private bool hasHeard;

    void OnEnable()
    {
        SoundEmitter.OnSound += OnSound;
    }

    void OnDisable()
    {
        SoundEmitter.OnSound -= OnSound;
    }

    void OnSound(Vector3 pos, float intensity)
    {
        float range = baseRange * intensity;

        if (Vector3.Distance(transform.position, pos) < range)
        {
            hasHeard = true;
            LastHeardPosition = pos;
        }
    }

    public bool CanHearSomething()
    {
        if (hasHeard)
        {
            hasHeard = false;
            return true;
        }
        return false;
    }
    /*void OnDrawGizmos()
    {
        // rango auditivo
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, baseRange);

        //  SOLO dibuja si escuchó
        if (hasHeard)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(LastHeardPosition, 0.3f);
            Gizmos.DrawLine(transform.position, LastHeardPosition);
        }
    }
    */
}