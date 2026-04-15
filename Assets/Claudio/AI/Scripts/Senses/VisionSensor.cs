using UnityEngine;

public class VisionSensor : MonoBehaviour
{
    [Header("Distancias")]
    public float closeRange = 6f;   // combate
    public float farRange = 12f;    // sospecha

    [Header("Ángulo")]
    public float viewAngle = 90f;

    public LayerMask obstacles;

    public enum VisionResult
    {
        None,
        Suspicious,
        Clear
    }

    public VisionResult CheckVision(Transform target)
    {
        Vector3 dir = (target.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, target.position);

        // fuera del rango máximo
        if (distance > farRange)
            return VisionResult.None;

        // ángulo de visión
        if (Vector3.Angle(transform.forward, dir) > viewAngle / 2)
            return VisionResult.None;

        // obstáculos
        if (Physics.Raycast(transform.position, dir, distance, obstacles))
            return VisionResult.None;

        // CERCA  combate
        if (distance <= closeRange)
            return VisionResult.Clear;

        // LEJOS sospecha
        return VisionResult.Suspicious;
    }

    // ESTE ES EL MÉTODO QUE FALTABA USAR
    /*void OnDrawGizmos()
    {
        // rango cercano
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, closeRange);

        // rango lejano
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, farRange);

        // cono cercano
        DrawWireArc(closeRange, Color.red);

        // cono lejano
        DrawWireArc(farRange, Color.yellow);

        // líneas centrales (dirección)
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * farRange);
    }

    void DrawWireArc(float radius, Color color)
    {
        Gizmos.color = color;

        int segments = 30;
        float angleStep = viewAngle / segments;

        Vector3 prevPoint = transform.position + Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward * radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = -viewAngle / 2 + angleStep * i;
            Vector3 nextPoint = transform.position + Quaternion.Euler(0, angle, 0) * transform.forward * radius;

            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }

        // líneas laterales del cono (forma V)
        Vector3 left = Quaternion.Euler(0, -viewAngle / 2, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, viewAngle / 2, 0) * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + left * radius);
        Gizmos.DrawLine(transform.position, transform.position + right * radius);
    }

    */
}