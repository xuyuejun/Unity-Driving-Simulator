using System;
using System.IO;
using UnityEngine;
/// <summary>
///  Copyright (c) 2016 Eric Zhu 
/// </summary>
namespace GreatArcStudios
{
    [System.Serializable]
    public class SaveSettings
    {
        public string fileName = "GameSettings.json";
        public int curQualityLevel;  // Quality preset
        public float fovINI;  // Field of View
        public int resHeight;  // Resolution heigh        
        public int resWidth;  // Resolution Width
        public bool fullscreenBool;  // Is the game in fullscreen
        public int msaaINI;     // MSAA quality

        public void LoadGameSettings(String readString)
        {
            SaveSettings read = JsonUtility.FromJson<SaveSettings>(readString);

            // Load Graphics
            QualitySettings.SetQualityLevel(read.curQualityLevel);
            PauseManager.mainCamShared.fieldOfView = read.fovINI;
            Screen.SetResolution(read.resWidth, read.resHeight, read.fullscreenBool);
            QualitySettings.antiAliasing = read.msaaINI;
        }

        // Get the quality/music settings before saving 
        public void SaveGameSettings()
        {
            Debug.Log("Save");
            string jsonString;
            if (File.Exists(Application.persistentDataPath + "/" + fileName))
            {
                File.Delete(Application.persistentDataPath + "/" + fileName);
            }

            // Save Graphics
            curQualityLevel = QualitySettings.GetQualityLevel();
            fovINI = PauseManager.mainCamShared.fieldOfView;
            resHeight = Screen.currentResolution.height;
            resWidth = Screen.currentResolution.width;
            fullscreenBool = Screen.fullScreen;
            msaaINI = QualitySettings.antiAliasing;
        
            jsonString = JsonUtility.ToJson(this);
            Debug.Log(jsonString);
            File.WriteAllText(Application.persistentDataPath + "/" + fileName, jsonString);
        }
    }
}