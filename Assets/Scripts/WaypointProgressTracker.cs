using System;
using UnityEngine;

#pragma warning disable 649

public class WaypointProgressTracker : MonoBehaviour
{
    [SerializeField] private WaypointCircuit circuit;
    public WaypointCircuit.RoutePoint progressPoint { get; private set; }
    public Rigidbody body;
    public float CarSpeed;
    public Transform target;
    public Vector3 progressDelta;
    public float Distance; 

    public float progressDistance;
    private void FixedUpdate()
    {
        CarSpeed = body.velocity.magnitude;
        target.position = circuit.GetRoutePoint(progressDistance).position;
        target.rotation = Quaternion.LookRotation(circuit.GetRoutePoint(progressDistance).direction);

        progressPoint = circuit.GetRoutePoint(progressDistance);
        progressDelta = progressPoint.position - transform.position;

        if (Vector3.Dot(progressDelta, progressPoint.direction) < 0)
        {
            progressDistance += progressDelta.magnitude * CarSpeed * 0.01f;
            Distance = progressDelta.magnitude + Vector3.Dot(progressDelta, progressPoint.direction);
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