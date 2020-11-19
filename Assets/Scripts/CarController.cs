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
    public enum Transmission
    {
        automatic,
        manual
    }
    [Header("Car status")]
    public int GearNum = 0;
    public int GearNumShow;
    public float CarSpeed;
    public float engineRPM;
    [Header("settings")]
    public float maxAngle = 30f;              // 最大角度
    public float brakeTorque = 30000f;        // 刹车动力
    [Header("Engine")]
    public float MinRPM;
    public float MaxRPM;
    public float MaxTorque;
    public float FinalDriveRatio = 2.56f;
    public float smoothTime = 0.3f;
    public float totalPower;
    public float wheelsRPM;
    public float radius = 6;
    public float DownForceValue = 50;
    public AnimationCurve RPMCurve;
    public float[] GearRatios = new float[] { 4.17f, 3.14f, 2.11f, 1.67f, 1.28f, 1f, 0.84f, 0.67f };
    [Header("Objects")]
    public GameObject centerOfMass;
    public Transform[] wheelMesh;             // 车轮模型
    public WheelCollider[] wheelColliders;    // 车轮碰撞器
    public Transform SteeringWheelMesh;   // 方向盘模型
    public Rigidbody carRigidbody;
    public DriveType driveType;
    public Transmission TransmissionType;
    [Header("Input")]
    public float horizontalInput;
    public float acceleratorInput;
    public float brakeInput;
    public bool handBrakeInput;
    public bool HeadLight;
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
    void OnHeadLight()
    {
        HeadLight = !HeadLight;
    }
    void OnUpShift()
    {
        if (TransmissionType == Transmission.manual)
        {
            GearNum++;
        }
    }
    void OnDownShift()
    {
        if (TransmissionType == Transmission.manual)
        {
            GearNum--;
        }
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
        shifter();
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
        float velocity = 0.0f;
        engineRPM = Mathf.SmoothDamp(engineRPM, MinRPM + (Mathf.Abs(wheelsRPM) * FinalDriveRatio * GearRatios[GearNum]), ref velocity, smoothTime);
        totalPower = RPMCurve.Evaluate(engineRPM / MaxRPM) * GearRatios[GearNum] * FinalDriveRatio * MaxTorque * acceleratorInput;
        if (engineRPM < 0.02f)
        {
            engineRPM = 0.0f;
        }

        accelerate();
    }

    void wheelRPM()
    {
        if (driveType == DriveType.FrontWheelDrive)
        {
            wheelsRPM = (wheelColliders[0].rpm + wheelColliders[1].rpm) / 2f;
        }
        if (driveType == DriveType.RearWheelDrive)
        {
            wheelsRPM = (wheelColliders[2].rpm + wheelColliders[3].rpm) / 2f;
        }
        if (driveType == DriveType.AllWheelDrive)
        {
            wheelsRPM = (wheelColliders[0].rpm + wheelColliders[1].rpm + wheelColliders[2].rpm + wheelColliders[3].rpm) / 4f;
        }
        if (wheelsRPM < 0)
        {
            wheelsRPM = 0;
        }
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
        CarSpeed = carRigidbody.velocity.magnitude * 3.6f;
        GearNumShow = GearNum;
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
    void shifter()
    {
        if (TransmissionType == Transmission.automatic)
        {
            if (engineRPM > MaxRPM - 1000 && GearNum < GearRatios.Length - 1)
            {
                GearNum++;
                engineRPM = engineRPM - 2000;
            }
            if (engineRPM < MinRPM + 1000 && GearNum > 1)
            {
                GearNum--;
                engineRPM = engineRPM + 1500;
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