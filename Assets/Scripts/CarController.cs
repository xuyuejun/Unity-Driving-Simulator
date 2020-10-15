using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InputManager))]

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

    public float criticalSpeed = 5f;
    [Tooltip("Simulation sub-steps when the speed is above critical.")]
    public int stepsBelow = 5;
    [Tooltip("Simulation sub-steps when the speed is below critical.")]
    public int stepsAbove = 1;

    [Tooltip("The vehicle's drive type: rear-wheels drive, front-wheels drive or all-wheels drive.")]

    [HideInInspector] public float fwdInput, backInput, horizontalInput;


    private InputManager im;
    private Rigidbody car;
    public DriveType driveType;

    void Awake()
    {
        car = GetComponent<Rigidbody>();
        im = GetComponent<InputManager>();
    }

    void Update()
    {
        simpleMoveCar();
        animateWheels();
        animateSteeringWheels();
    }

    // 函数

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
        Vector3 SteeringWheelPosition = new Vector3(15, 0, -2 * im.Horizontal * maxAngle);
        SteeringWheelMesh.transform.localEulerAngles = SteeringWheelPosition;
    }

    void simpleMoveCar()
    {

        wheelColliders[0].ConfigureVehicleSubsteps(criticalSpeed, stepsBelow, stepsAbove);

        float angle = maxAngle * im.Horizontal;
        float torque = maxTorque * im.Torque;
        float handBrake = im.brake ? brakeTorque : 0;

        foreach (WheelCollider wheel in wheelColliders)
        {
            // A simple car where front wheels steer while rear ones drive.
            if (wheel.transform.localPosition.z > 0)
            {
                wheel.steerAngle = angle;
            }
            if (wheel.transform.localPosition.z < 0)
            {
                wheel.brakeTorque = handBrake;
            }
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
    // void OnGUI()
    // {
    //     foreach (WheelCollider wc in GetComponentsInChildren<WheelCollider>()) {
    //         GUILayout.Label (string.Format("{0} sprung: {1}, k: {2}, d: {3}", wc.name, wc.sprungMass, wc.suspensionSpring.spring, wc.suspensionSpring.damper));
    //     }
    //     GUILayout.Label ("horizontalInput: " + im.Horizontal);
    // }
}
