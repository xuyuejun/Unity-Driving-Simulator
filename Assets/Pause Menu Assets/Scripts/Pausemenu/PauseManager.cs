using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.IO;

namespace GreatArcStudios
{
    public class PauseManager : MonoBehaviour
    {
        [Header("Panel")]
        public GameObject mainPanel;
        public GameObject vidPanel;
        public GameObject settingPanel;
        [Header("Video Setting")]
        public Text presetLabel;
        public Toggle vSyncToggle;
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
        public float detailDensity;
        public float timeScale = 1f;
        [Header("Others")]
        /// <summary>
        /// Audio Panel animator
        /// </summary>
        public Animator settingPanelAnimator;
        /// <summary>
        /// Video Panel animator  
        /// </summary>
        public Animator vidPanelAnimator;
        /// <summary>
        /// Quit Panel animator  
        /// </summary>
        public Animator quitPanelAnimator;

        internal static float renderDistINI;
        /// <summary>
        /// Inital AA quality 2, 4, or 8
        /// </summary>
        internal static float aaQualINI;
        /// <summary>
        /// Inital terrain detail density
        /// </summary>
        internal static float densityINI;
        /// <summary>
        /// Amount of trees that are acutal meshes
        /// </summary>
        internal static float treeMeshAmtINI;
        /// <summary>
        /// Inital fov 
        /// </summary>
        internal static float fovINI;
        /// <summary>
        /// Inital msaa amount 
        /// </summary>
        internal static int msaaINI;
        internal static int vsyncINI;
        public AudioSource[] music;
        /// <summary>
        /// An array of sound effect audio sources
        /// </summary>
        public AudioSource[] effects;
        /// <summary>
        /// An array of the other UI elements, which is used for disabling the other elements when the game is paused.
        /// </summary>
        public GameObject[] otherUIElements;
        /// <summary>
        /// Editor boolean for hardcoding certain video settings. It will allow you to use the values defined in LOD Bias and Shadow Distance
        /// </summary>
        public Boolean hardCodeSomeVideoSettings;
        /// <summary>
        /// Boolean for turning on simple terrain
        /// </summary>
        public Boolean useSimpleTerrain;
        public static Boolean readUseSimpleTerrain;
        /// <summary>
        /// Event system
        /// </summary>
        public EventSystem uiEventSystem;
        /// <summary>
        /// Defualt selected on the video panel
        /// </summary>
        public GameObject defualtSelectedVideo;
        /// <summary>
        /// Defualt selected on the video panel
        /// </summary>
        public GameObject defualtSelectedAudio;
        /// <summary>
        /// Defualt selected on the video panel
        /// </summary>
        public GameObject defualtSelectedMain;
        //last music multiplier; this should be a value between 0-1
        internal static float lastMusicMult;
        //last audio multiplier; this should be a value between 0-1
        internal static float lastAudioMult;
        //Initial master volume
        internal static float beforeMaster;
        //last texture limit 
        internal static int lastTexLimit;
        //int for amount of effects
        private int _audioEffectAmt = 0;
        //Inital audio effect volumes
        private float[] _beforeEffectVol;

        //Initial music volume
        private float _beforeMusic;
        //Preset level
        private int _currentLevel;
        //Resoutions
        private Resolution[] allRes;
        //Camera dof script
        private MonoBehaviour tempScript;
        //Presets 
        private String[] presets;
        //Fullscreen Boolean
        private Boolean isFullscreen;
        //current resoultion
        internal static Resolution currentRes;
        //Last resoultion 
        private Resolution beforeRes;

        //last shadow cascade value
        internal static int lastShadowCascade;

        public static Boolean aoBool;
        public static Boolean dofBool;
        private Boolean lastAOBool;
        private Boolean lastDOFBool;
        public static Terrain readTerrain;
        public static Terrain readSimpleTerrain;

        private SaveSettings saveSettings = new SaveSettings();

        /// <summary>
        /// The start method; you will need to place all of your inital value getting/setting here. 
        /// </summary>
        public void Start()
        {
            mainCamShared = mainCam;
            //Set the first selected item
            uiEventSystem.firstSelectedGameObject = defualtSelectedMain;
            //Get the presets from the quality settings 
            presets = QualitySettings.names;
            presetLabel.text = presets[QualitySettings.GetQualityLevel()].ToString();
            _currentLevel = QualitySettings.GetQualityLevel();
            //Get the current resoultion, if the game is in fullscreen, and set the label to the original resolution
            allRes = Screen.resolutions;
            currentRes = Screen.currentResolution;
            //Debug.Log("ini res" + currentRes);
            resolutionLabel.text = Screen.currentResolution.width.ToString() + " x " + Screen.currentResolution.height.ToString();
            isFullscreen = Screen.fullScreen;
            //get all ini values
            aaQualINI = QualitySettings.antiAliasing;
            renderDistINI = mainCam.farClipPlane;
            fovINI = mainCam.fieldOfView;
            msaaINI = QualitySettings.antiAliasing;
            vsyncINI = QualitySettings.vSyncCount;
            //enable titles
            TitleTexts.SetActive(true);
            //Disable other panels
            mainPanel.SetActive(false);
            vidPanel.SetActive(false);
            settingPanel.SetActive(false);
            //Enable mask
            mask.SetActive(false);
            //set last texture limit
            lastTexLimit = QualitySettings.masterTextureLimit;
            //set last shadow cascade 
            lastShadowCascade = QualitySettings.shadowCascades;
            try
            {
                saveSettings.LoadGameSettings(File.ReadAllText(Application.persistentDataPath + "/" + saveSettings.fileName));
            }
            catch
            {
                Debug.Log("Game settings not found in: " + Application.persistentDataPath + "/" + saveSettings.fileName);
                saveSettings.SaveGameSettings();
            }

        }

        /// Restart the level by loading the loaded level.
        public void Restart()
        {
            SceneManager.LoadScene(0);
            Time.timeScale = timeScale;
            // uiEventSystem.firstSelectedGameObject = defualtSelectedMain;
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
            for (int i = 0; i < otherUIElements.Length; i++)
            {
                otherUIElements[i].gameObject.SetActive(true);
            }
        }

        /// All the methods relating to qutting should be called here.
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
                pauseMenu.text = "Audio Menu";
            }
            else if (mainPanel.activeSelf == true)
            {
                pauseMenu.text = "Pause Menu";
            }

            if (Input.GetKeyDown(KeyCode.Escape) && mainPanel.activeSelf == false)
            {

                uiEventSystem.SetSelectedGameObject(defualtSelectedMain);
                mainPanel.SetActive(true);
                vidPanel.SetActive(false);
                settingPanel.SetActive(false);
                TitleTexts.SetActive(true);
                mask.SetActive(true);
                Time.timeScale = 0;
                for (int i = 0; i < otherUIElements.Length; i++)
                {
                    otherUIElements[i].gameObject.SetActive(false);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && mainPanel.activeSelf == true)
            {
                Time.timeScale = timeScale;
                mainPanel.SetActive(false);
                vidPanel.SetActive(false);
                settingPanel.SetActive(false);
                TitleTexts.SetActive(false);
                mask.SetActive(false);
                for (int i = 0; i < otherUIElements.Length; i++)
                {
                    otherUIElements[i].gameObject.SetActive(true);
                }
            }
        }

        /// Show the audio panel 
        public void Audio()
        {
            mainPanel.SetActive(false);
            vidPanel.SetActive(false);
            settingPanel.SetActive(true);
            audioIn();
            pauseMenu.text = "Audio Menu";
        }

        /// Play the "audio panel in" animation.
        public void audioIn()
        {
            uiEventSystem.SetSelectedGameObject(defualtSelectedAudio);
            float a; float b; float f;
            try
            {
                a = music[0].volume;
                b = music[1].volume;
                f = a % b;
            }
            catch
            {
                Debug.Log("You do not have multiple audio sources");
            }
            //Do this with the effects
            try
            {
                a = effects[0].volume;
                b = effects[1].volume;
                f = a % b;
            }
            catch
            {
                Debug.Log("You do not have multiple audio sources");
            }

        }
        /// <summary>
        /// Audio Option Methods
        /// </summary>
        /// <param name="f"></param>
        public void updateMasterVol(float f)
        {

            //Controls volume of all audio listeners 
            AudioListener.volume = f;
        }
        /// <summary>
        /// Update music effects volume
        /// </summary>
        /// <param name="f"></param>
        public void updateMusicVol(float f)
        {
            try
            {
                for (int _musicAmt = 0; _musicAmt < music.Length; _musicAmt++)
                {
                    music[_musicAmt].volume *= f;
                }
            }
            catch
            {
                Debug.Log("Please assign music sources in the manager");
            }
            //_beforeMusic = music.volume;
        }
        /// <summary>
        /// Update the audio effects volume
        /// </summary>
        /// <param name="f"></param>
        public void updateEffectsVol(float f)
        {
            try
            {
                for (_audioEffectAmt = 0; _audioEffectAmt < effects.Length; _audioEffectAmt++)
                {
                    //get the values for all effects before the change
                    _beforeEffectVol[_audioEffectAmt] = effects[_audioEffectAmt].volume;

                    //lower it by a factor of f because we don't want every effect to be set to a uniform volume
                    effects[_audioEffectAmt].volume *= f;
                }
            }
            catch
            {
                Debug.Log("Please assign audio effects sources in the manager.");
            }

        }

        public void Video()
        {
            mainPanel.SetActive(false);
            vidPanel.SetActive(true);
            settingPanel.SetActive(false);
            videoIn();
            pauseMenu.text = "Video Menu";

        }
        public void videoIn()
        {

            // uiEventSystem.SetSelectedGameObject(defualtSelectedVideo);
            // Presets
            presetLabel.text = presets[QualitySettings.GetQualityLevel()].ToString();
            // VSync
            if (QualitySettings.vSyncCount == 0)
            {
                vSyncToggle.isOn = false;
            }
            else if (QualitySettings.vSyncCount == 1)
            {
                vSyncToggle.isOn = true;
            }
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

        /// Cancel the video setting changes 
        public void cancelVideo()
        {
            uiEventSystem.SetSelectedGameObject(defualtSelectedMain);
            cancelVideoMain();
        }

        /// Use an IEnumerator to first play the animation and then changethe video settings
        public void cancelVideoMain()
        {
            try
            {
                mainCam.farClipPlane = renderDistINI;
                mainCam.fieldOfView = fovINI;
                mainPanel.SetActive(true);
                vidPanel.SetActive(false);
                settingPanel.SetActive(false);
                aoBool = lastAOBool;
                dofBool = lastDOFBool;
                Screen.SetResolution(beforeRes.width, beforeRes.height, Screen.fullScreen);
                QualitySettings.antiAliasing = (int)aaQualINI;
                QualitySettings.antiAliasing = msaaINI;
                QualitySettings.vSyncCount = vsyncINI;
                QualitySettings.masterTextureLimit = lastTexLimit;
                QualitySettings.shadowCascades = lastShadowCascade;
                Screen.fullScreen = isFullscreen;
            }
            catch
            {
                Debug.Log("A problem occured (chances are the terrain was not assigned )");
                mainCam.farClipPlane = renderDistINI;
                mainCam.fieldOfView = fovINI;
                mainPanel.SetActive(true);
                vidPanel.SetActive(false);
                settingPanel.SetActive(false);
                aoBool = lastAOBool;
                dofBool = lastDOFBool;
                Screen.SetResolution(beforeRes.width, beforeRes.height, Screen.fullScreen);
                QualitySettings.antiAliasing = (int)aaQualINI;
                QualitySettings.antiAliasing = msaaINI;
                QualitySettings.vSyncCount = vsyncINI;
                QualitySettings.masterTextureLimit = lastTexLimit;
                QualitySettings.shadowCascades = lastShadowCascade;
                //Screen.fullScreen = isFullscreen;
            }

        }
        public void apply()
        {
            applyVideo();
            uiEventSystem.SetSelectedGameObject(defualtSelectedMain);
        }
        /// <summary>
        /// Use an IEnumerator to first play the animation and then change the video settings.
        /// </summary>
        /// <returns></returns>
        public void applyVideo()
        {
            mainPanel.SetActive(true);
            vidPanel.SetActive(false);
            settingPanel.SetActive(false);
            renderDistINI = mainCam.farClipPlane;
            fovINI = mainCam.fieldOfView;
            lastAOBool = aoBool;
            lastDOFBool = dofBool;
            beforeRes = currentRes;
            lastTexLimit = QualitySettings.masterTextureLimit;
            lastShadowCascade = QualitySettings.shadowCascades;
            vsyncINI = QualitySettings.vSyncCount;
            isFullscreen = Screen.fullScreen;
            saveSettings.SaveGameSettings();
        }

        public void toggleVSync(Boolean B)
        {
            vsyncINI = QualitySettings.vSyncCount;
            if (B == true)
            {
                QualitySettings.vSyncCount = 1;
            }
            else
            {
                QualitySettings.vSyncCount = 0;
            }
        }

        /// Change the fov using a float. The defualt should be 60.
        public void updateFOV(float fov)
        {
            mainCam.fieldOfView = fov;
        }

        /// Set the game to windowed or full screen. This is meant to be used with a checkbox
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

        public void nextRes()
        {
            beforeRes = currentRes;
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
            beforeRes = currentRes;
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

        public void nextPreset()
        {
            _currentLevel = QualitySettings.GetQualityLevel();
            QualitySettings.IncreaseLevel();
            _currentLevel = QualitySettings.GetQualityLevel();
            presetLabel.text = presets[_currentLevel].ToString();
        }
        public void lastPreset()
        {
            _currentLevel = QualitySettings.GetQualityLevel();
            QualitySettings.DecreaseLevel();
            _currentLevel = QualitySettings.GetQualityLevel();
            presetLabel.text = presets[_currentLevel].ToString();
        }

        // set quality level
        public void setMinimal()
        {
            QualitySettings.SetQualityLevel(0);
        }
        public void setVeryLow()
        {
            QualitySettings.SetQualityLevel(1);
        }
        public void setLow()
        {
            QualitySettings.SetQualityLevel(2);
        }
        public void setNormal()
        {
            QualitySettings.SetQualityLevel(3);
        }
        public void setVeryHigh()
        {
            QualitySettings.SetQualityLevel(4);
        }
        public void setUltra()
        {
            QualitySettings.SetQualityLevel(5);
        }
        public void setExtreme()
        {
            QualitySettings.SetQualityLevel(6);
        }
    }
}
