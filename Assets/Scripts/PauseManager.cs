using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.IO;

public class PauseManager : MonoBehaviour
{
    [Header("Panel")]
    public GameObject mainPanel;
    public GameObject vidPanel;
    public GameObject settingPanel;
    [Header("Video Setting")]
    public Text presetLabel;
    public Slider fovSlider;
    public Text resolutionLabel;
    public Toggle fullscreenToggle;
    public Dropdown aaCombo;

    [Header("Mask")]
    public GameObject TitleTexts;
    public GameObject mask;
    public Text pauseMenu;
    [Header("Camera")]
    public Camera mainCam;
    internal static Camera mainCamShared;
    public GameObject mainCamObj;
    [Header("Settings")]
    public float timeScale = 1f;
    [Header("INI")]
    public int qualityLevel;
    public float fovINI;
    private Resolution currentRes;
    public Boolean isFullscreen;
    public int msaaINI;
    [Header("Others")]
    //Resoutions
    private Resolution[] allRes;
    //Presets 
    private String[] presets;
    //last shadow cascade value
    public SaveSettings saveSettings = new SaveSettings();
    // The start method; you will need to place all of your inital value getting/setting here. 
    public void Start()
    {
        mainCamShared = mainCam;
        // Video Panel
        presets = QualitySettings.names;        //Get the presets from the quality settings 
        presetLabel.text = presets[QualitySettings.GetQualityLevel()].ToString();
        qualityLevel = QualitySettings.GetQualityLevel();
        fovINI = mainCam.fieldOfView;
        allRes = Screen.resolutions;
        currentRes = Screen.currentResolution;
        resolutionLabel.text = Screen.currentResolution.width.ToString() + " x " + Screen.currentResolution.height.ToString();
        isFullscreen = Screen.fullScreen;
        msaaINI = QualitySettings.antiAliasing;

        TitleTexts.SetActive(true);
        mainPanel.SetActive(false);
        vidPanel.SetActive(false);
        settingPanel.SetActive(false);
        mask.SetActive(false);

        // If settings files not found
        if (File.Exists(Application.persistentDataPath + "/" + "GameSettings.json"))
        {
            saveSettings.LoadGameSettings(File.ReadAllText(Application.persistentDataPath + "/" + "GameSettings.json"));
        }
        else
        {
            Debug.Log("Game settings not found in: " + Application.persistentDataPath + "/" + "GameSettings.json");
            saveSettings.SaveGameSettings();
        }
    }

    /// Restart the level by loading the loaded level.
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
        pauseMenu.text = "Video Menu";

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
        pauseMenu.text = "Audio Menu";
        // Somethings
    }

    // All the methods relating to qutting should be called here.
    public void quitOptions()
    {
        Application.Quit();
    }

    public void Update()
    {
        if (vidPanel.activeSelf == true)
        {
            pauseMenu.text = "Video Menu";
        }
        else if (settingPanel.activeSelf == true)
        {
            pauseMenu.text = "Setting Menu";
        }
        else if (mainPanel.activeSelf == true)
        {
            pauseMenu.text = "Pause Menu";
        }

        if (Input.GetKeyDown(KeyCode.Escape) && mainPanel.activeSelf == false)
        {
            mainPanel.SetActive(true);
            vidPanel.SetActive(false);
            settingPanel.SetActive(false);
            TitleTexts.SetActive(true);
            mask.SetActive(true);
            Time.timeScale = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && mainPanel.activeSelf == true)
        {
            Time.timeScale = timeScale;
            mainPanel.SetActive(false);
            vidPanel.SetActive(false);
            settingPanel.SetActive(false);
            TitleTexts.SetActive(false);
            mask.SetActive(false);
        }
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

        fovINI = mainCam.fieldOfView;
        isFullscreen = Screen.fullScreen;
        saveSettings.SaveGameSettings();
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
    public void setFullScreen(Boolean b)
    {
        if (b == true)
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
}