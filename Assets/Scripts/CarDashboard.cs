using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarDashboard : MonoBehaviour
{
    public CarController car;
    [Header("Tachometer")]
    public GameObject needle;
    public float RPM;
    private float startPosiziton = 27f, endPosition = -206f;        // 转速仪表盘
    private float desiredPosition;
    [Header("Speedometer and Gear")]
    public GameObject SpeedometerAndGear;
    public float Speed;
    public float GearNum;
    private Text digit0, digit1, digit2, gear;
	private float fd0, fd1, fd2;
	private int d0, d1, d2;
    void Start()
    {
        digit0 = SpeedometerAndGear.transform.GetChild (0).GetComponent<Text> ();
		digit1 = SpeedometerAndGear.transform.GetChild (1).GetComponent<Text> ();
		digit2 = SpeedometerAndGear.transform.GetChild (2).GetComponent<Text> ();
		gear = SpeedometerAndGear.transform.GetChild (3).GetComponent<Text> ();
    }

    // Update is called once per frame
    void Update()
    {
        Speed = car.CarSpeed;
        GearNum = car.GearNum;
        RPM = car.engineRPM;
        updateNeedle();
        updateSpeedometerAndGear();
    }

    public void updateNeedle()
    {
        desiredPosition = startPosiziton - endPosition;
        float temp = RPM / 10000;
        needle.transform.eulerAngles = new Vector3(0, 0, (startPosiziton - temp * desiredPosition));
    }
    public void updateSpeedometerAndGear()
    {
		fd2 = Speed % 10;
		fd1 = (Speed / 10) % 10;
		fd0 = (Speed / 100) % 10;

		d2 = (int)fd2;
		d1 = (int)fd1;
		d0 = (int)fd0;

		if (d0 < 0)
			d0 *= -1;
		if (d1 < 0)
			d1 *= -1;
		if (d2 < 0)
			d2 *= -1;

		digit2.text = d2.ToString ();
		digit1.text = d1.ToString ();
		digit0.text = d0.ToString ();

		if (GearNum == 0)
			gear.text = "R";
		else
			gear.text = GearNum.ToString ();
    }
}
