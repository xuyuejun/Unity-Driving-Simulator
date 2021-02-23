using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SaveSettings
{
    private GameManager gameManager;
    [Header("Video")]
    public int curQualityLevel;  // Quality preset
    public float fovINI;  // Field of View
    public int resHeight;  // Resolution heigh        
    public int resWidth;  // Resolution Width
    public bool fullscreenBool;  // Is the game in fullscreen
    public int msaaINI;     // MSAA quality
    [Header("Settings")]
    public string name;
    public int drivingExperience;

    public void LoadGameSettings(String readString)
    {
        Debug.Log("Load");
        SaveSettings read = JsonUtility.FromJson<SaveSettings>(readString);
        // Load Graphics
        QualitySettings.SetQualityLevel(read.curQualityLevel);
        PauseManager.mainCamShared.fieldOfView = read.fovINI;
        Screen.SetResolution(read.resWidth, read.resHeight, read.fullscreenBool);
        QualitySettings.antiAliasing = read.msaaINI;
        // Load Settings
        name = read.name;
        drivingExperience = read.drivingExperience;
    }

    // Get the quality/music settings before saving 
    public void SaveGameSettings()
    {
        Debug.Log("Save");
        string jsonString;
        if (File.Exists(Application.persistentDataPath + "/" + "GameSettings.json"))
        {
            File.Delete(Application.persistentDataPath + "/" + "GameSettings.json");
        }

        // Save Graphics
        curQualityLevel = QualitySettings.GetQualityLevel();
        fovINI = PauseManager.mainCamShared.fieldOfView;
        resHeight = Screen.currentResolution.height;
        resWidth = Screen.currentResolution.width;
        fullscreenBool = Screen.fullScreen;
        msaaINI = QualitySettings.antiAliasing;
        // Save Settings
        // name = gameManager.playerData.name;
        // drivingExperience = gameManager.playerData.drivingExperience;
        // Debug.Log(gameManager.playerData);


        jsonString = JsonUtility.ToJson(this);
        Debug.Log(jsonString);
        File.WriteAllText(Application.persistentDataPath + "/" + "GameSettings.json", jsonString);
    }
}