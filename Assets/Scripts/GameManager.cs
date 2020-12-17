using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class RacingData
    {
        public int index;
        public float time;
        public float carSpeed;
        public float engineRPM;
        public int gearNum;
        public float locationX;
        public float locationY;
        public float altitude;
        public float horizontalInput;
        public float acceleratorInput;
        public float brakeInput;
        public RacingData(int Index, float Time, float CarSpeed, float EngineRPM, int GearNum, float LocationX, float LocationY, float Altitude, float HorizontalInput, float AcceleratorInput, float BrakeInput)
        {
            this.index = Index;
            this.time = Time;
            this.carSpeed = CarSpeed;
            this.engineRPM = EngineRPM;
            this.gearNum = GearNum;
            this.locationX = LocationX;
            this.locationY = LocationY;
            this.altitude = Altitude;
            this.horizontalInput = HorizontalInput;
            this.acceleratorInput = AcceleratorInput;
            this.brakeInput = BrakeInput;
        }
    }
    [System.Serializable]
    public class Player
    {
        public string name;
        public string EnglishName;
        public int drivingExperience;
        public string date;
        public string displayType;
        public float waitTime;
        public List<RacingData> RacingDatas;
    }
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
    public int GearNum;
    [Header("Car Status")]
    public float horizontalInput;
    public float acceleratorInput;
    public float brakeInput;
    [Header("Main Camera Status")]
    public int cameraStatus;
    [Header("Check Point Status")]
    public bool GameInProgress = false;
    [Header("Camera and Canvas status")]
    public bool FirstPersonView;
    public GameObject recObject;
    public GameObject DashBoard;
    public GameObject MiniMap;
    [Header("Json Data")]
    public Player playerData;
    public float waitTime = 10;
    public float currentTime = 0;
    private int elapseTime = 0;
    public int index = 0;
    void Start()
    {

    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        if (GameInProgress)
        {
            elapseTime++;
            if (elapseTime >= waitTime)
            {
                playerData.RacingDatas.Add(new RacingData(index, currentTime, CarSpeed, engineRPM, GearNum, Xcoordinate, Ycoordinate, Altitude, horizontalInput, acceleratorInput, brakeInput));
                index++;
                currentTime = currentTime + 0.1f;
                elapseTime = 0;
            }
        }
        UpdateMainCanvas();
        getData();
    }

    void OnGameStart()
    {
        print("游戏开始");
        playerData.date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        GameInProgress = true;
    }
    void OnGameEnd()
    {
        print("游戏结束");
        string json = JsonUtility.ToJson(playerData);
        WriteToFile(System.DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), json);
        GameInProgress = false;
    }
    void UpdateMainCanvas()
    {
        recObject.SetActive(GameInProgress);
        DashBoard.SetActive(!FirstPersonView);
        MiniMap.SetActive(!FirstPersonView);
    }
    private void WriteToFile(string fileName, string json)
    {
        string path = GetFilePath(fileName);
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(json);
        }
    }
    private string GetFilePath(string fileName)
    {
        return Application.persistentDataPath + "/PlayerData/" + fileName + ".json";
    }
    void getData()
    {
        CarSpeed = car.CarSpeed;
        GearNum = car.GearNum;
        engineRPM = car.engineRPM;

        Xcoordinate = car.transform.position.x + 600;
        Ycoordinate = car.transform.position.z + 400;
        Altitude = car.transform.position.y - 270;

        horizontalInput = car.horizontalInput;
        acceleratorInput = car.acceleratorInput;
        brakeInput = car.brakeInput;

        FirstPersonView = mainCamera.ShowingFirstPersonView;
    }
    void OnGUI()
    {
        GUILayout.Label("CarSpeed: " + CarSpeed);
        GUILayout.Label("engineRPM: " + engineRPM);
        GUILayout.Label("GearNum: " + GearNum);

        GUILayout.Label("Xcoordinate: " + Xcoordinate);
        GUILayout.Label("Ycoordinate: " + Ycoordinate);
        GUILayout.Label("Altitude" + Altitude);
        GUILayout.Label("index" + index);
    }

}
