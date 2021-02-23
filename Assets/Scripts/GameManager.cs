using System;
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
        public double time;
        public double carSpeed;
        public double engineRPM;
        public int gearNum;
        public double locationX;
        public double locationY;
        public double altitude;
        public double centerlineDistance;
        public double horizontalInput;
        public double acceleratorInput;
        public double brakeInput;
        public RacingData(int Index, float Time, float CarSpeed, float EngineRPM, int GearNum, float LocationX, float LocationY, float Altitude, float CenterlineDistance, float HorizontalInput, float AcceleratorInput, float BrakeInput)
        {
            this.index = Index;
            this.time = double.Parse(Time.ToString("#0.0"));
            this.carSpeed = double.Parse(CarSpeed.ToString("#0.00"));
            this.engineRPM = double.Parse(EngineRPM.ToString("#0.00"));
            this.gearNum = GearNum;
            this.locationX = double.Parse(LocationX.ToString("#0.00"));
            this.locationY = double.Parse(LocationY.ToString("#0.00"));
            this.altitude = double.Parse(Altitude.ToString("#0.00"));
            this.centerlineDistance = double.Parse(CenterlineDistance.ToString("#0.00"));
            this.horizontalInput = double.Parse(HorizontalInput.ToString("#0.00"));
            this.acceleratorInput = double.Parse(AcceleratorInput.ToString("#0.00"));
            this.brakeInput = double.Parse(BrakeInput.ToString("#0.00"));
        }
    }
    [System.Serializable]
    public class Player
    {
        public string name;
        public int drivingExperience;
        public string weather;
        public string date;
        public string displayType;
        public float waitTime;
        public List<RacingData> RacingDatas;
    }
    [System.Serializable]
    public class GameSettings
    {
        // Player
        public string name;
        public int drivingExperience;
        public bool guidelineBool;
        // Video
        public int curQualityLevel;  // Quality preset
        public float fovINI;  // Field of View
        public int resHeight;  // Resolution heigh        
        public int resWidth;  // Resolution Width
        public bool fullscreenBool;  // Is the game in fullscreen
        public int msaaINI;     // MSAA quality
    }
    [Header("Input")]
    public CarController car;
    public CameraController mainCamera;
    public WaypointProgressTracker Tracker;
    [Header("Panel")]
    public GameObject mainPanel;
    public GameObject vidPanel;
    public GameObject settingPanel;
    [Header("InputField and Toggle")]
    public GameObject windshield;
    public InputField inputName;
    public InputField inputExperience;
    public Toggle guideline;
    [Header("Video Setting")]
    public Text presetLabel;
    public Slider fovSlider;
    public Text resolutionLabel;
    public Toggle fullscreenToggle;
    public Dropdown aaCombo;

    [Header("Mask")]
    public GameObject TitleTexts;
    public GameObject mask;
    [Header("Camera")]
    public Camera mainCam;
    internal static Camera mainCamShared;
    public GameObject mainCamObj;
    [Header("Settings")]
    public float timeScale = 1f;
    [Header("INI")]
    public int qualityLevel;
    private Resolution currentRes;
    public Boolean isFullscreen;
    public int msaaINI;
    [Header("Others")]
    //Resoutions
    private Resolution[] allRes;
    //Presets 
    private String[] presets;
    [Header("Location Status")]
    public float Xcoordinate;
    public float Ycoordinate;
    public float Altitude;
    public float CenterlineDistance;
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
    public GameSettings gameSettings;
    public Player playerData;
    // public string PlayerFile = "PlayerInformation.json";
    public float waitTime = 10;
    public float currentTime = 0;
    private int elapseTime = 0;
    public int index = 0;
    void OnPauseMenu()
    {
        if (mainPanel.activeSelf == false)
        {
            mainPanel.SetActive(true);
            vidPanel.SetActive(false);
            settingPanel.SetActive(false);
            TitleTexts.SetActive(true);
            mask.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = timeScale;
            mainPanel.SetActive(false);
            vidPanel.SetActive(false);
            settingPanel.SetActive(false);
            TitleTexts.SetActive(false);
            mask.SetActive(false);
        }
    }

    void LoadGameSettings(String readString)
    {
        Debug.Log("Load");
        GameSettings read = JsonUtility.FromJson<GameSettings>(readString);

        // Load Settings
        gameSettings.name = read.name;
        gameSettings.drivingExperience = read.drivingExperience;

        gameSettings.guidelineBool = read.guidelineBool;
        guideline.isOn = read.guidelineBool;
        showGuideline();

        // Load to PlayData
        playerData.name = read.name;
        playerData.drivingExperience = read.drivingExperience;

        // Load Graphics
        QualitySettings.SetQualityLevel(read.curQualityLevel);
        mainCamShared.fieldOfView = read.fovINI;
        Screen.SetResolution(read.resWidth, read.resHeight, read.fullscreenBool);
        fullscreenToggle.isOn = read.fullscreenBool;
        QualitySettings.antiAliasing = read.msaaINI;
    }
    public void SaveGameSettings()
    {
        Debug.Log("Save");
        string jsonString;
        if (File.Exists(Application.persistentDataPath + "/" + "GameSettings.json"))
        {
            File.Delete(Application.persistentDataPath + "/" + "GameSettings.json");
        }

        // Save Graphics
        gameSettings.curQualityLevel = QualitySettings.GetQualityLevel();
        gameSettings.fovINI = mainCamShared.fieldOfView;
        gameSettings.resHeight = Screen.currentResolution.height;
        gameSettings.resWidth = Screen.currentResolution.width;
        gameSettings.fullscreenBool = Screen.fullScreen;
        gameSettings.msaaINI = QualitySettings.antiAliasing;

        jsonString = JsonUtility.ToJson(gameSettings);
        Debug.Log(jsonString);
        File.WriteAllText(Application.persistentDataPath + "/" + "GameSettings.json", jsonString);
    }

    void Start()
    {
        mainCamShared = mainCam;
        // Video Panel
        presets = QualitySettings.names;        //Get the presets from the quality settings 
        presetLabel.text = presets[QualitySettings.GetQualityLevel()].ToString();
        qualityLevel = QualitySettings.GetQualityLevel();
        allRes = Screen.resolutions;
        currentRes = Screen.currentResolution;
        resolutionLabel.text = Screen.currentResolution.width.ToString() + " x " + Screen.currentResolution.height.ToString();
        isFullscreen = Screen.fullScreen;
        msaaINI = QualitySettings.antiAliasing;
        // Weather
        TitleTexts.SetActive(true);
        mainPanel.SetActive(false);
        vidPanel.SetActive(false);
        settingPanel.SetActive(false);
        mask.SetActive(false);

        inputName.onValueChanged.AddListener(inputNameChanged);
        inputExperience.onValueChanged.AddListener(inputExperienceChanged);

        // If settings files not found
        if (File.Exists(Application.persistentDataPath + "/" + "GameSettings.json"))
        {
            LoadGameSettings(File.ReadAllText(Application.persistentDataPath + "/" + "GameSettings.json"));
        }
        else
        {
            Debug.Log("Game settings not found in: " + Application.persistentDataPath + "/" + "GameSettings.json");
            SaveGameSettings();
        }
    }
    void FixedUpdate()
    {
        if (GameInProgress)
        {
            elapseTime++;
            if (elapseTime >= waitTime)
            {
                playerData.RacingDatas.Add(new RacingData(index, currentTime, CarSpeed, engineRPM, GearNum, Xcoordinate, Ycoordinate, Altitude, CenterlineDistance, horizontalInput, acceleratorInput, brakeInput));
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
        playerData.date = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        GameInProgress = true;
    }
    void OnGameEnd()
    {
        string json = JsonUtility.ToJson(playerData);
        WriteToFile(playerData.name + " " + playerData.weather + " " + System.DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss"), json);
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
    private string GetSettingFilePath(string fileName)
    {
        return Application.persistentDataPath + "/" + fileName;
    }
    private string ReadFromFile(string fileName)
    {
        string path = GetSettingFilePath(fileName);
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string json = reader.ReadToEnd();
                return json;
            }
        }
        else
        {
            Debug.LogWarning("File not found!");
        }
        return "";
    }
    void getData()
    {
        CarSpeed = car.CarSpeed;
        GearNum = car.GearNum;
        engineRPM = car.engineRPM;

        Xcoordinate = car.transform.position.x + 600;
        Ycoordinate = car.transform.position.z + 400;
        Altitude = car.transform.position.y - 270;

        CenterlineDistance = Tracker.Distance;

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
        GUILayout.Label("Altitude: " + Altitude);
        GUILayout.Label("CenterlineDistance: " + CenterlineDistance);
        GUILayout.Label("index: " + index);

        GUILayout.Label("Name: " + playerData.name);
        GUILayout.Label("Driving Experience: " + playerData.drivingExperience);
        GUILayout.Label("Name: " + playerData.weather);
    }

    // Pause Menu
    public void Restart()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = timeScale;
    }

    /// Method to resume the game, so disable the pause menu and re-enable all other ui elements
    public void Resume()
    {
        Time.timeScale = timeScale;

        mainPanel.SetActive(false);
        vidPanel.SetActive(false);
        settingPanel.SetActive(false);
        TitleTexts.SetActive(false);
        mask.SetActive(false);
    }

    public void Video()
    {
        mainPanel.SetActive(false);
        vidPanel.SetActive(true);
        settingPanel.SetActive(false);

        // Presets
        presetLabel.text = presets[QualitySettings.GetQualityLevel()].ToString();
        // Field of View
        fovSlider.value = mainCam.fieldOfView;
        // Fullscreen
        fullscreenToggle.isOn = Screen.fullScreen;
        // MSAA
        if (QualitySettings.antiAliasing == 0)
        {
            aaCombo.value = 0;
        }
        else if (QualitySettings.antiAliasing == 2)
        {
            aaCombo.value = 1;
        }
        else if (QualitySettings.antiAliasing == 4)
        {
            aaCombo.value = 2;
        }
        else if (QualitySettings.antiAliasing == 8)
        {
            aaCombo.value = 3;
        }
    }

    // Show the settings panel 
    public void Settings()
    {
        mainPanel.SetActive(false);
        vidPanel.SetActive(false);
        settingPanel.SetActive(true);
        // Somethings
    }

    // All the methods relating to qutting should be called here.
    public void quitOptions()
    {
        Application.Quit();
    }

    /// Cancel the video setting changes 
    public void resetVideo()
    {
        mainPanel.SetActive(true);
        vidPanel.SetActive(false);
        settingPanel.SetActive(false);

        QualitySettings.SetQualityLevel(3);
        mainCam.fieldOfView = 60;
        Screen.fullScreen = true;
        Screen.SetResolution(allRes[allRes.Length - 1].width, allRes[allRes.Length - 1].height, Screen.fullScreen);
        QualitySettings.antiAliasing = msaaINI;
    }
    public void applyVideo()
    {
        mainPanel.SetActive(true);
        vidPanel.SetActive(false);
        settingPanel.SetActive(false);

        SaveGameSettings();
    }
    public void resetSetting()
    {
        mainPanel.SetActive(true);
        vidPanel.SetActive(false);
        settingPanel.SetActive(false);

        Debug.Log("Doing nothing");
    }
    // Input Field
    void inputNameChanged(string value)
    {
        playerData.name = value;
        gameSettings.name = value;
        Debug.Log(value);
    }
    void inputExperienceChanged(string value)
    {
        playerData.drivingExperience = Int32.Parse(value);
        gameSettings.drivingExperience = Int32.Parse(value);
        Debug.Log(value);
    }

    public void showGuideline()
    {
        if (guideline.isOn == true)
        {
            gameSettings.guidelineBool = true;
            mainCam.cullingMask = -1;
        }
        else
        {
            gameSettings.guidelineBool = false;
            mainCam.cullingMask = 311;
        }
    }

    // Graphics Functions
    public void nextPreset()
    {
        qualityLevel = QualitySettings.GetQualityLevel();
        QualitySettings.IncreaseLevel();
        qualityLevel = QualitySettings.GetQualityLevel();
        presetLabel.text = presets[qualityLevel].ToString();
    }
    public void lastPreset()
    {
        qualityLevel = QualitySettings.GetQualityLevel();
        QualitySettings.DecreaseLevel();
        qualityLevel = QualitySettings.GetQualityLevel();
        presetLabel.text = presets[qualityLevel].ToString();
    }

    /// Change the fov using a float. The defualt should be 60.
    public void updateFOV(float fov)
    {
        mainCam.fieldOfView = fov;
    }

    // Set the Resolution
    public void nextRes()
    {
        //Iterate through all of the resoultions. 
        for (int i = 0; i < allRes.Length; i++)
        {
            //If the resoultion matches the current resoution height and width then go through the statement.
            if (allRes[i].height == currentRes.height && allRes[i].width == currentRes.width)
            {
                //Debug.Log("found " + i);
                //If the user is playing fullscreen. Then set the resoution to one element higher in the array, set the full screen boolean to true, reset the current resolution, and then update the resolution label.
                if (isFullscreen == true) { Screen.SetResolution(allRes[i + 1].width, allRes[i + 1].height, true); isFullscreen = true; currentRes = Screen.resolutions[i + 1]; resolutionLabel.text = currentRes.width.ToString() + " x " + currentRes.height.ToString(); }
                //If the user is playing in a window. Then set the resoution to one element higher in the array, set the full screen boolean to false, reset the current resolution, and then update the resolution label.
                if (isFullscreen == false) { Screen.SetResolution(allRes[i + 1].width, allRes[i + 1].height, false); isFullscreen = false; currentRes = Screen.resolutions[i + 1]; resolutionLabel.text = currentRes.width.ToString() + " x " + currentRes.height.ToString(); }
                //Debug.Log("Res after: " + currentRes);
            }
        }

    }

    //Method for moving to the last resoution in the allRes array. WARNING: This is not finished/buggy.  
    public void lastRes()
    {
        //Iterate through all of the resoultions. 
        for (int i = 0; i < allRes.Length; i++)
        {
            if (allRes[i].height == currentRes.height && allRes[i].width == currentRes.width)
            {
                //Debug.Log("found " + i);
                //If the user is playing fullscreen. Then set the resoution to one element lower in the array, set the full screen boolean to true, reset the current resolution, and then update the resolution label.
                if (isFullscreen == true) { Screen.SetResolution(allRes[i - 1].width, allRes[i - 1].height, true); isFullscreen = true; currentRes = Screen.resolutions[i - 1]; resolutionLabel.text = currentRes.width.ToString() + " x " + currentRes.height.ToString(); }
                //If the user is playing in a window. Then set the resoution to one element lower in the array, set the full screen boolean to false, reset the current resolution, and then update the resolution label.
                if (isFullscreen == false) { Screen.SetResolution(allRes[i - 1].width, allRes[i - 1].height, false); isFullscreen = false; currentRes = Screen.resolutions[i - 1]; resolutionLabel.text = currentRes.width.ToString() + " x " + currentRes.height.ToString(); }
                //Debug.Log("Res after: " + currentRes);
            }
        }
    }

    // Set the game to windowed or full screen. This is meant to be used with a checkbox
    public void setFullScreen()
    {
        if (fullscreenToggle.isOn == true)
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
        }
        else
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, false);
        }
    }

    // Set the MSAA
    public void updateMSAA(int msaaAmount)
    {
        Debug.Log("update msaa");
        if (msaaAmount == 0)
        {
            // disableMSAA
            QualitySettings.antiAliasing = 0;
        }
        else if (msaaAmount == 1)
        {
            // twoMSAA
            QualitySettings.antiAliasing = 2;
        }
        else if (msaaAmount == 2)
        {
            // fourMSAA
            QualitySettings.antiAliasing = 4;
        }
        else if (msaaAmount == 3)
        {
            // eightMSAA()
            QualitySettings.antiAliasing = 8;
        }
    }

    public void Windshield(int id)
    {
        Debug.Log("Weather ID " + id);
        if (id == 0)
        {
            playerData.weather = "Rain";
            windshield.SetActive(true);
        }
        else if (id == 1)
        {
            playerData.weather = "Clear Sky";
            windshield.SetActive(false);
        }
        else
        {
            playerData.weather = "Foggy";
            windshield.SetActive(false);
        }
    }
}
