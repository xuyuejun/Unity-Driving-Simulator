using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public KeyCode switchViewKey = KeyCode.Space;
    public float smoothing = 6f;
    public Transform FirstPersonTarget;
    public Transform ThirdPersonTarget;
    public Transform lookAtTarget;
    bool m_SHowingFirstPersonView;
    void FixedUpdate()
    {
        UpdateCamera ();
    }

    void Update() {
        if (Input.GetKeyDown (switchViewKey))
        {
            m_SHowingFirstPersonView = !m_SHowingFirstPersonView;
        }    
    }
    private void UpdateCamera ()
    {
        if (m_SHowingFirstPersonView)
        {
            transform.position = FirstPersonTarget.position;
            transform.rotation = FirstPersonTarget.rotation;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, ThirdPersonTarget.position, Time.deltaTime * smoothing);
            transform.LookAt(lookAtTarget);
        }    
    }
}
