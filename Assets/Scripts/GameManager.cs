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
    public bool GameInProgress = false;
    [Header("Camera and Canvas status")]
    public bool FirstPersonView;
    public GameObject recObject;
    public GameObject DashBoard;
    public GameObject MiniMap;
    void Start()
    {

    }
    void FixedUpdate()
    {
        UpdateMainCanvas();
        getData();
        // debug();
    }

    void OnGameStart()
    {
        print("游戏开始");
        GameInProgress = true;
    }
    void OnGameEnd()
    {
        print("游戏结束");
        GameInProgress = false;
    }
    void UpdateMainCanvas()
    {
        recObject.SetActive(GameInProgress);
        DashBoard.SetActive(!FirstPersonView);
        MiniMap.SetActive(!FirstPersonView);
    }
    void getData()
    {
        CarSpeed = car.CarSpeed;
        GearNum = car.GearNum;
        engineRPM = car.engineRPM;

        Xcoordinate = car.transform.position.x + 600;
        Ycoordinate = car.transform.position.z + 400;
        Altitude = car.transform.position.y - 270;

        FirstPersonView = mainCamera.ShowingFirstPersonView;
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
