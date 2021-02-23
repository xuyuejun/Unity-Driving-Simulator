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

    /// Restart the level by loading the loaded level.
    public void showGuideline(bool b)
    {
        Debug.Log(b);

        if (b == true)
        {
            Debug.Log("Show guideline");
            //
        }
        else
        {
            Debug.Log("Dont show it");
            // 
        }
    }
}