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
    [Header("settings")]
    public float maxAngle = 30f;              // 最大角度
    public float motorTorque = 300f;            // 最大扭力
    public float brakeTorque = 30000f;        // 刹车动力
    public AnimationCurve enginePower;
    public float[] GearRatios = new float[] { 4.17f, 3.14f, 2.11f, 1.67f, 1.28f, 1f, 0.84f, 0.67f };
    [Header("Values")]
    public float totalPower;
    public float engineRPM;
    public float wheelsRPM;
    public float radius = 6;
    public float DownForceValue = 50;
    private float smoothTime = 0.09f;
    [Header("Objects")]
    public GameObject centerOfMass;
    public Transform[] wheelMesh;             // 车轮模型
    public WheelCollider[] wheelColliders;    // 车轮碰撞器
    public Transform SteeringWheelMesh;   // 方向盘模型
    public Rigidbody carRigidbody;
    public DriveType driveType;
    [Header("Input")]
    public float horizontalInput;
    public float acceleratorInput;
    public float brakeInput;
    public bool handBrakeInput;
    [Header("Car status")]
    public int gearNum = 0;
    public float Mps;
    public float Kph;
    public float rpm;
    [Header("Debugger")]
    public float[] slip = new float[4];

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

    void Awake()
    {
        carRigidbody.centerOfMass = centerOfMass.transform.localPosition;
    }

    void Update()
    {
        animateWheels();
        animateSteeringWheels();
    }

    void FixedUpdate()
    {
        steerVehicle();
        handBrake();

        calculateEnginePower();

        addDownForce();
        GetCarStatus();
        // getFriction();
    }

    void calculateEnginePower()
    {
        wheelRPM();
        totalPower = enginePower.Evaluate(engineRPM) * GearRatios[gearNum] * acceleratorInput;
        float velocity = 0.0f;
        engineRPM = Mathf.SmoothDamp(engineRPM, 1000 + (Mathf.Abs(wheelsRPM) * 3.6f * (GearRatios[gearNum])), ref velocity, smoothTime);
        if(engineRPM < 0.02f)
        {
            engineRPM = 0.0f;
        }
        
        accelerate();
    }

    void wheelRPM()
    {
        float sum = 0;
        int R = 0;
        for (int i = 0; i < 4; i++)
        {
            sum += wheelColliders[i].rpm;
            R++;
        }
        wheelsRPM = (R != 0) ? sum / R : 0;
    }

    void shifter()
    {
        
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
                wheel.motorTorque = (totalPower / 2) * (acceleratorInput - brakeInput);
            }
            if (driveType == DriveType.RearWheelDrive && wheel.transform.localPosition.z < 0)
            {
                wheel.motorTorque = (totalPower / 2) * (acceleratorInput - brakeInput);
            }
            if (driveType == DriveType.AllWheelDrive)
            {
                wheel.motorTorque = (totalPower / 4) * (acceleratorInput - brakeInput);
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
        Mps = carRigidbody.velocity.magnitude;
        Kph = Mps * 3.6f;
    }
    void addDownForce()
    {
        carRigidbody.AddForce(-transform.up * DownForceValue * carRigidbody.velocity.magnitude);
    }

    void getFriction()
    {
        for (int i = 0; i < wheelMesh.Length; i++)
        {
            WheelHit wheelHit;
            wheelColliders[i].GetGroundHit(out wheelHit);
            slip[i] = wheelHit.sidewaysSlip;
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