using UnityEngine;
using System.Collections;

[RequireComponent (typeof(InputManager))]

public class CarController : MonoBehaviour
{
    internal enum driveType
	{
		FrontWheelDrive,
		RearWheelDrive,
		AllWheelDrive
	}
    public float topSpeed;                    // 最高速度
    public float accleration;                 // 加速度
    public float boostForce = 1000;           // boost
    public Transform[] wheelMesh;             // 车轮模型
    public WheelCollider[] wheelColliders;    // 车轮碰撞器

	[HideInInspector]public float fwdInput, backInput, horizontalInput;


    private InputManager im;                  // 输入管理 import manager
    private Rigidbody car;                    // 汽车刚体

    void Awake ()
	{
		car = GetComponent<Rigidbody> ();
		im = GetComponent<InputManager> ();
	}

    void FixedUpdate()
    {
        // fwdInput = im.forward;
		// backInput = im.backward;
		// horizontalInput = im.Horizontal;
        animateWheels ();
    }

    void animateWheels ()
    {
        Vector3 wheelPosition = Vector3.zero;
		Quaternion wheelRotation = Quaternion.identity;

		for (int i = 0; i < 4; i++) {
			wheelColliders [i].GetWorldPose (out wheelPosition, out wheelRotation);
			wheelMesh [i].position = wheelPosition;
			wheelMesh [i].rotation = wheelRotation;
		}
    }
}
