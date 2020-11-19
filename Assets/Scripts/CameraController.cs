using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float smoothing = 6f;
    public Transform FirstPersonTarget;
    public Transform ThirdPersonTarget;
    public Transform lookAtTarget;
    public bool ShowingFirstPersonView;

    void OnChangeCamera()
    {
        ShowingFirstPersonView = !ShowingFirstPersonView;
    }

    void Update()
    {
        if (ShowingFirstPersonView)
        {
            transform.position = FirstPersonTarget.position;
            transform.rotation = FirstPersonTarget.rotation;
        }
    }
    void FixedUpdate()
    {
        if (!ShowingFirstPersonView)
        {
            transform.position = Vector3.Lerp(transform.position, ThirdPersonTarget.position, Time.deltaTime * smoothing);
            transform.LookAt(lookAtTarget);
        }
    }
}
