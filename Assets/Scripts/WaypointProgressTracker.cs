using System;
using UnityEngine;

#pragma warning disable 649

public class WaypointProgressTracker : MonoBehaviour
{
    [SerializeField] private WaypointCircuit circuit; // A reference to the waypoint-based route we should follow
    public WaypointCircuit.RoutePoint progressPoint { get; private set; }

    public Transform target;
    public Vector3 progressDelta;

    public float progressDistance; // The progress round the route, used in smooth mode.
    public int progressNum; // the current waypoint number, used in point-to-point mode.
    private void FixedUpdate()
    {
        target.position = circuit.GetRoutePoint(progressDistance).position;
        target.rotation = Quaternion.LookRotation(circuit.GetRoutePoint(progressDistance).direction);

        progressPoint = circuit.GetRoutePoint(progressDistance);
        progressDelta = progressPoint.position - transform.position;

        // Debug.Log(progressPoint.position);
        // Debug.Log(Vector3.Dot(progressDelta, progressPoint.direction));

        if (Vector3.Dot(progressDelta, progressPoint.direction) < 0)
        {
            Debug.Log(progressDelta.magnitude);
            progressDistance += progressDelta.magnitude * 0.2f;
        }

    }


    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);
            Gizmos.DrawWireSphere(circuit.GetRoutePosition(progressDistance), 1);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(target.position, target.position + target.forward);
        }
    }
}