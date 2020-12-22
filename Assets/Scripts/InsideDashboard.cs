using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InsideDashboard : MonoBehaviour
{
    public CarController car;
    public Text speedText;
    public float Speed;
    // Update is called once per frame
    void Update()
    {
        Speed = car.CarSpeed;
        speedText.text = Speed.ToString("0");
    }
}
