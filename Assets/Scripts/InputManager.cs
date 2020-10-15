using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal enum InputType
{
    keyboard,
    controller
}

public class InputManager : MonoBehaviour
{
    [SerializeField] private InputType inputType;

    public float Horizontal;
    public float Torque;
    public bool brake;
    void Update()
    {
        if (inputType == InputType.keyboard)
        {
            Horizontal = Input.GetAxis("Horizontal");
            Torque = Input.GetAxis("Vertical");
            brake = Input.GetKey(KeyCode.X);
        }
        else
        {
            // do nothing
        }
    }
}
