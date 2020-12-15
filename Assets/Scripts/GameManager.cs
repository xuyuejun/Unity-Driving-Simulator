using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Input")]
    public CarController car;
    public CameraController mainCamera;
    [Header("Location Status")]
    public float Xcoordinate;
    public float Ycoordinate;
    public float Altitude;
    [Header("Car Status")]
    public float CarSpeed;
    public float engineRPM;
    public bool isBrake;
    public float GearNum;
    [Header("Main Camera Status")]
    public int cameraStatus;
    [Header("Check Point Status")]
    public bool GameInProgress;
    void Start()
    {

    }
    void FixedUpdate()
    {
        getData();
        // debug();
    }
    void OnGameStart()
    {
        print("游戏开始");
    }
    void OnGameEnd()
    {
        print("游戏结束");
    }
    void getData()
    {
        CarSpeed = car.CarSpeed;
        GearNum = car.GearNum;
        engineRPM = car.engineRPM;

        Xcoordinate = car.transform.position.x + 600;
        Ycoordinate = car.transform.position.z + 400;
        Altitude = car.transform.position.y - 270;
    }
    void debug()
    {
        Debug.Log("Test");
    }

    void OnGUI()
    {
        GUILayout.Label("CarSpeed: " + CarSpeed);
        GUILayout.Label("engineRPM: " + engineRPM);
        GUILayout.Label("GearNum: " + GearNum);

        GUILayout.Label("Xcoordinate: " + Xcoordinate);
        GUILayout.Label("Ycoordinate: " + Ycoordinate);
        GUILayout.Label("Altitude" + Altitude);
    }

}
