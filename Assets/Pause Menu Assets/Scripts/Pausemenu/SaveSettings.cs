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
        public int vsyncINI;  // VSync settings
        public float fovINI;  // Field of View
        public int resHeight;  // Resolution heigh        
        public int resWidth;  // Resolution Width
        public bool fullscreenBool;  // Is the game in fullscreen
        public int msaaINI;     // MSAA quality

        public void LoadGameSettings(String readString)
        {
            Debug.Log("Load");
            SaveSettings read = JsonUtility.FromJson<SaveSettings>(readString);
            QualitySettings.antiAliasing = read.msaaINI;
            PauseManager.mainCamShared.fieldOfView = read.fovINI;
            QualitySettings.vSyncCount = read.vsyncINI;
            QualitySettings.SetQualityLevel(read.curQualityLevel);
            Screen.SetResolution(read.resWidth, read.resHeight, read.fullscreenBool);
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

            msaaINI = QualitySettings.antiAliasing;
            fovINI = PauseManager.mainCamShared.fieldOfView;
            curQualityLevel = QualitySettings.GetQualityLevel();

            resHeight = Screen.currentResolution.height;
            resWidth = Screen.currentResolution.width;
            fullscreenBool = Screen.fullScreen;
            
            jsonString = JsonUtility.ToJson(this);
            Debug.Log(jsonString);
            File.WriteAllText(Application.persistentDataPath + "/" + fileName, jsonString);
        }
    }
}