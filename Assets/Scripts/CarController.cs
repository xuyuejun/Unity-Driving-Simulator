using UnityEngine;
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
    public float motorTorque = 300f;            // 最大扭力
    public float brakeTorque = 30000f;        // 刹车动力
    public float radius = 6;
    public Transform[] wheelMesh;             // 车轮模型
    public WheelCollider[] wheelColliders;    // 车轮碰撞器
    public Transform SteeringWheelMesh;   // 方向盘模型
    public Rigidbody car;
    public DriveType driveType;
    [Header("Input")]
    public float horizontalInput;
    public float acceleratorInput;
    public float brakeInput;
    public bool handBrakeInput;
    [Header("Car status")]
    public float Mps;
    public float Kph;
    public float rpm;

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

    private void FixedUpdate()
    {
        steerVehicle();
        accelerate();
        handBrake();

        animateWheels();
        animateSteeringWheels();
        GetCarStatus();
    }

    void steerVehicle()
    {
        //acerman steering formula
        if (horizontalInput > 0)
        {
            //rear tracks size is set to 1.5f       wheel base has been set to 2.55f
            wheelColliders[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;
            wheelColliders[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * horizontalInput;
        }
        else if (horizontalInput < 0)
        {
            wheelColliders[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * horizontalInput;
            wheelColliders[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;
        }
        else
        {
            wheelColliders[0].steerAngle = 0;
            wheelColliders[1].steerAngle = 0;
        }
    }
    void accelerate()
    {
        foreach (WheelCollider wheel in wheelColliders)
        {
            if (driveType == DriveType.FrontWheelDrive && wheel.transform.localPosition.z >= 0)
            {
                wheel.motorTorque = (motorTorque / 2) * (acceleratorInput - brakeInput);
            }
            if (driveType == DriveType.RearWheelDrive && wheel.transform.localPosition.z < 0)
            {
                wheel.motorTorque = (motorTorque / 2) * (acceleratorInput - brakeInput);
            }
            if (driveType == DriveType.AllWheelDrive)
            {
                wheel.motorTorque = (motorTorque / 4) * (acceleratorInput - brakeInput);
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

    void GetCarStatus()
    {
        Mps = car.velocity.magnitude;
        Kph = Mps * 3.6f;
    }
    // void OnGUI()
    // {
    //     foreach (WheelCollider wc in GetComponentsInChildren<WheelCollider>()) {
    //         GUILayout.Label (string.Format("{0} sprung: {1}, k: {2}, d: {3}", wc.name, wc.sprungMass, wc.suspensionSpring.spring, wc.suspensionSpring.damper));
    //     }
    //     GUILayout.Label ("horizontalInput: " + im.Horizontal);
    // }
}

public enum GearState
{
    ParkingGear = 1,      //停车挡
    ReversGear = 2,       //倒挡
    NeutralGear = 3,       //空挡
    ForwardGear = 4,         //前进挡
}

public enum SpeedGear
{
    none,
    Speed01,
    Speed02,
    Speed03,
    Speed04,
    Speed05,
    Speed06
}