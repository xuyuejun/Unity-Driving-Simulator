﻿using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public enum DriveType
    {
        FrontWheelDrive,
        RearWheelDrive,
        AllWheelDrive
    }
    public float maxAngle = 30f;              // 最大角度
    public float maxTorque = 300f;            // 最大扭力
    public float brakeTorque = 30000f;        // 刹车动力
    public Transform[] wheelMesh;             // 车轮模型
    public WheelCollider[] wheelColliders;    // 车轮碰撞器
    public Transform SteeringWheelMesh;   // 方向盘模型

    private Rigidbody car;
    public DriveType driveType;

    // Input
    public float horizontalInput;
    public float acceleratorInput;
    public float brakeInput;
    public bool handBrakeInput;

    // 输入控制
    void OnDirection(InputValue value)
    {
        Vector2 directionInput = value.Get<Vector2>();
        horizontalInput = directionInput.x;
    }
    void OnAcceleratorPedal(InputValue value)
    {
        acceleratorInput = value.Get<float>();
    }
    void OnBrakePedal(InputValue value)
    {
        brakeInput = value.Get<float>();
    }
    void OnHandBrake(InputValue value)
    {
        handBrakeInput = value.isPressed;
    }

    void Update()
    {
        steer();
        accelerate();
        handBrake();
        animateWheels();
        animateSteeringWheels();
    }

    void steer()
    {
        foreach (WheelCollider wheel in wheelColliders)
        {
            if (wheel.transform.localPosition.z > 0)
            {
                wheel.steerAngle = maxAngle * horizontalInput;
            }
        }
    }
    void accelerate()
    {
        float torque = maxTorque * (acceleratorInput - brakeInput);

        foreach (WheelCollider wheel in wheelColliders)
        {
            if (wheel.transform.localPosition.z < 0 && driveType != DriveType.FrontWheelDrive)
            {
                wheel.motorTorque = torque;
            }
            if (wheel.transform.localPosition.z >= 0 && driveType != DriveType.RearWheelDrive)
            {
                wheel.motorTorque = torque;
            }
        }
    }

    void handBrake()
    {
        float handBrake = handBrakeInput ? brakeTorque : 0;

        foreach (WheelCollider wheel in wheelColliders)
        {
            wheel.brakeTorque = handBrake;
        }
    }
    void animateWheels()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;

        for (int i = 0; i < 4; i++)
        {
            wheelColliders[i].GetWorldPose(out wheelPosition, out wheelRotation);
            wheelMesh[i].position = wheelPosition;
            wheelMesh[i].rotation = wheelRotation;
        }
    }

    void animateSteeringWheels()
    {
        SteeringWheelMesh.transform.localEulerAngles = new Vector3(15, 0, -2 * horizontalInput * maxAngle);
    }

    // void OnGUI()
    // {
    //     foreach (WheelCollider wc in GetComponentsInChildren<WheelCollider>()) {
    //         GUILayout.Label (string.Format("{0} sprung: {1}, k: {2}, d: {3}", wc.name, wc.sprungMass, wc.suspensionSpring.spring, wc.suspensionSpring.damper));
    //     }
    //     GUILayout.Label ("horizontalInput: " + im.Horizontal);
    // }
}
