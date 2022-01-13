using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class Gameplay_UI : PanelManagement
{
    /*
     * INSTRUCTIONS:
     * This codes manage the menu setup, the buttons listeners and actions nad the level pause.
     * The specific behaviour of some data (e.g.  life) are done in other UI codes.
     */


    // - - - GENERAL DATA - - -
    public static bool ready;
    private static Gameplay_UI instance;
    [SerializeField] private bool standaloneUI, smartphoneUI;

    // WARNING: The 'panelsSceneName' must have the same order as the 'Panels' enum
    [HideInInspector] public enum Panels { none, pause, lose, win, quitConfirmation }
    private static readonly string[] panelsSceneName = { " ", "Panel_Pause", "Panel_Lose", "Panel_Win", "Panel_QuitConfirmation" };
    public static GameObject[] panelGO = new GameObject[Enum.GetNames(typeof(Panels)).Length];
    public static Panels[] openedMenus = new Panels[10];

    // - - - HUD Buttons - - -
    public static Button unstuckButton;
    public static Button launchButton;

    // - - - Pause menu - - -
    private static Slider musicSlider, sfxSlider;
    public static float musicSliderVal = 1f, sfxSliderVal = 0f;
    

    void Awake()
    {
        SetOS_UI();
    }

    void Start()
    {
        // Set UI manager
        UI_Manager.currentMenuLayer = 0;
        UI_Manager.currentPanel = null;
        UI_Manager.inMenu = true;

        // This code settings
        SetLevelIndicators();
        SetHUD();
        SetPauseMenu(ref panelGO, panelsSceneName);

        // Other codes settings
#if UNITY_ANDROID
        if(SceneManager.GetActiveScene().buildIndex == 2)
            gameObject.AddComponent<FirstLevelGuide>().Set();
#endif
        StartCoroutine(WaitForOtherCodes());
    }

    private IEnumerator WaitForOtherCodes()
    {
        while(!HUD.ready)
            yield return null;
        HUD.ready = false;
        ready = true;
    }

    void Update()
    {
        if (GameManager.loadingScene)
            return;

        // Return to previous menu
        if (UI_Manager.returnTrigger)
        {
            UI_Manager.returnTrigger = false;
            ReturnToPreviousMenu();
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < openedMenus.Length; i++)
            openedMenus[i] = Panels.none;
        StopAllCoroutines();
    }


#region Panel management

    /// <summary>
    /// Open a new panel menu of the UI.
    /// </summary>
    /// <param name="panelToGo"> Enum of the panel of this class for easy use.</param>
    public static void OpenMenu(Panels panelToGo)
    {
        if (panelGO[(int)panelToGo] != null)
        {
            // Open new menu and close previous one
            GameObject currentPanel = panelGO[(int)(openedMenus[UI_Manager.currentMenuLayer])];
            GameObject newPanel = panelGO[(int)(panelToGo)];
            OpenNewMenu(newPanel, currentPanel);

            // Save menu
            openedMenus[UI_Manager.currentMenuLayer] = panelToGo;
        }
    }

    public static void ClosePanel()
    {
        GameObject panelToOpenGO = panelGO[(int)(openedMenus[UI_Manager.currentMenuLayer - 1])];
        GameObject panelToCloseGO = panelGO[(int)openedMenus[UI_Manager.currentMenuLayer]];
        //print($"Opening panel: {openedMenus[UI_Manager.currentMenuLayer - 1]}");
        //print($"Closing panel panel: {openedMenus[UI_Manager.currentMenuLayer]}");
        ReturnToPreviousPanel(panelToOpenGO, panelToCloseGO);
    }

    /// <summary>
    /// Go to the previous menu if its in a below layer. In case of the pause main menu, close it and unpause the game.
    /// </summary>
    private static void ReturnToPreviousMenu()
    {
        if (UI_Manager.inMenu)
        {
            if (openedMenus[UI_Manager.currentMenuLayer] == Panels.pause)
                LevelManager.PauseMenu(false, true);
            else
                ClosePanel();
        }
    }

#endregion

#region HUD

    private static void SetHUD()
    {
        // Get gameplay HUD components
        unstuckButton = instance.transform.Find("Canvas_HUD/Panel_LeftBlock/ButtonUnstuck").gameObject.GetComponent<Button>();
        unstuckButton.interactable = false;
        launchButton = instance.transform.Find("Canvas_HUD/Panel_LeftBlock/LaunchBallButton").gameObject.GetComponent<Button>();

        // Set buttons
        AssignHUDButtonsListener();
    }

    private static void AssignHUDButtonsListener()
    {
        Button tempButton;
        string panelPath = "Canvas_HUD/Panel_LeftBlock";

        // Unstuck button
        tempButton = instance.transform.Find(panelPath + "/ButtonUnstuck").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { HUDButtonsActions("unstuck"); });

        // Launch ball button
        tempButton = instance.transform.Find(panelPath + "/LaunchBallButton").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { HUDButtonsActions("launchball"); });

        panelPath = "Canvas_HUD/Panel_RightBlock";

        // Pause button
        tempButton = instance.transform.Find(panelPath + "/PauseButton").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { HUDButtonsActions("pause"); });
    }

    private static void HUDButtonsActions(string button)
    {
        button = button.ToLower();
        button = button.Trim();

        switch (button)
        {
            case "pause":
                LevelManager.pauseTrigger = true;
                break;

            case "unstuck":
                Ball.unstuckBallTrigger = true;
                Ball.hitObject = true;
                break;

            case "launchball":
                InputsManager.releaseBall = true;
                launchButton.interactable = false;
                break;

            default:
                print("attempt didn't work");
                return;
        }

        //AudioManager.PlayAudio(UI_Manager.audioSource, UI_Manager.uiClips[(int)UI_Manager.UiAudioNames.button], false, 1f);
    }

#endregion

#region Pause menu

    public void SetPauseMenu(ref GameObject[] panelGO, string[] panelsSceneName)
    {
        // Get panels for gameplay
        GameObject canvasGameplayMenu = transform.Find("Canvas_Menu").gameObject;
        for (int i = 1; i < Enum.GetValues(typeof(Panels)).Length; i++)
        {
            panelGO[i] = canvasGameplayMenu.transform.Find(panelsSceneName[i]).gameObject;
            panelGO[i].SetActive(false);
        }

        // Set buttons
        AssignMenuButtonsListener();

        // Get audio sliders
        musicSlider = instance.transform.Find("Canvas_Menu/Panel_Pause/Music/VolumeSlider").GetComponent<Slider>();
        sfxSlider = instance.transform.Find("Canvas_Menu/Panel_Pause/SFX/VolumeSlider").GetComponent<Slider>();
        musicSlider.value = musicSliderVal;
        sfxSlider.value = sfxSliderVal;

        // Change win panel if its the last level
        if (SceneManager.GetActiveScene().buildIndex >= (LevelManager.maxLevels + 1))
            Destroy(instance.transform.Find("Canvas_Menu/Panel_Win/ButtonNextLevel").gameObject);
        else
            Destroy(instance.transform.Find("Canvas_Menu/Panel_Win/Thanks").gameObject);
    }

    /// <summary>
    /// This triggers the action when clicking a button from the pause menu in gameplay.
    /// </summary>
    private static void PauseMenuButtonsActions(string button)
    {
        button = button.ToLower();
        button = button.Trim();

        switch (button)
        {
            case "continue":
                UI_Manager.returnTrigger = true;
                AudioManager.PlayAudio(UI_Manager.audioSource, UI_Manager.uiClips[(int)UI_Manager.UiAudioNames.unPause], false, 1f);
                return;

            case "return":
                ClosePanel();
                break;

            case "quit":
                OpenMenu(Panels.quitConfirmation);
                break;

            case "quitconfirm":
                print("quit application");
                Application.Quit();
                return;

            case "restart":
                GameManager.GoToScene(SceneManager.GetActiveScene().buildIndex);
                break;

            case "nextlevel":
                GameManager.GoToScene(SceneManager.GetActiveScene().buildIndex + 1);
                break;

            case "mainmenu":
                GameManager.GoToScene(0);
                break;

            default:
                print("attempt didn't work");
                return;
        }

        AudioManager.PlayAudio(UI_Manager.audioSource, UI_Manager.uiClips[(int)UI_Manager.UiAudioNames.button], false, 1f);
    }

    /// <summary>
    /// This assing the event when clicking the buttons for the gameplay menus, so we dont have to do it manually in the inspector every time we start a new game.
    /// </summary>
    private static void AssignMenuButtonsListener()
    {
        // - - - - PAUSE PANEL - - - -

        string panelPath = "Canvas_Menu/Panel_Pause";
        Button tempButton;

        // Continue button
        tempButton = instance.transform.Find(panelPath + "/ButtonContinue").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("continue"); });

        // Restart button
        tempButton = instance.transform.Find(panelPath + "/ButtonRestart").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("restart"); });

        // Main menu button
        tempButton = instance.transform.Find(panelPath + "/ButtonMainmenu").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("mainmenu"); });

        // Quit button
        tempButton = instance.transform.Find(panelPath + "/ButtonQuit").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("quit"); });


        // - - - - QUIT CONFIRMATION PANEL - - - -

        panelPath = "Canvas_Menu/Panel_QuitConfirmation";

        // Confirm button
        tempButton = instance.transform.Find(panelPath + "/ButtonConfirm").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("quitconfirm"); });

        // Return button
        tempButton = instance.transform.Find(panelPath + "/ButtonReturn").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("return"); });


        // - - - - WIN PANEL - - - -

        panelPath = "Canvas_Menu/Panel_Win";

        // Next level button
        tempButton = instance.transform.Find(panelPath + "/ButtonNextLevel").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("nextlevel"); });

        // Main menu button
        tempButton = instance.transform.Find(panelPath + "/ButtonMainmenu").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("mainmenu"); });


        // - - - - LOSE PANEL - - - -

        panelPath = "Canvas_Menu/Panel_Lose";

        // Restart button
        tempButton = instance.transform.Find(panelPath + "/ButtonRestart").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("restart"); });

        // Main menu button
        tempButton = instance.transform.Find(panelPath + "/ButtonMainmenu").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("mainmenu"); });
    }

    public void SetMusicVolume(float volume)
    {
        musicSliderVal = volume;
        float fixedVolume = SliderValToMixerVal(volume);
        AudioManager.SetMusicVolume(fixedVolume);
    }

    public void SetSFX_Volume(float volume)
    {
        sfxSliderVal = volume;
        float fixedVolume = SliderValToMixerVal(volume);
        AudioManager.SetSFX_Volume(fixedVolume);
    }

    public static float SliderValToMixerVal(float volume)
    {
        // NOTA: valores optimos entre 20 y -20. Totales entre 20 y -80
        if (volume >= 0)
            return MapValue(volume, 0, 2, 0, 10);
        else if (volume >= -7)
            return MapValue(volume, -7, 0, -20, 0);
        else
            return MapValue(volume, -8, -7, -80, -20);
    }

    static float MapValue(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

#endregion

    private void SetOS_UI()
    {
#if UNITY_STANDALONE
        if(smartphoneUI)
            Destroy(this.gameObject);
        else
        {
            instance = this;
            instance.gameObject.name = "UI";
        }
#endif

#if UNITY_ANDROID
        // Destroy pc UI and change smartphone ui name so the codes can use it.
        if (standaloneUI)
            Destroy(this.gameObject);
        else if (smartphoneUI)
        {
            instance = this;
            instance.gameObject.name = "UI";
        }
#endif
    }

    private void SetLevelIndicators()
    {
        int actualScene = SceneManager.GetActiveScene().buildIndex - 1;
        TextMeshProUGUI startLevelIndicator = instance.transform.Find("Canvas_Menu/Panel_LevelEntering/TitleTMP").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI InGame_LevelIndicator = instance.transform.Find("Canvas_HUD/Panel_RightBlock/Level").GetComponent<TextMeshProUGUI>();
        if (actualScene < 10)
            startLevelIndicator.text = InGame_LevelIndicator.text = $"Level 0{actualScene}";
        else
            startLevelIndicator.text = InGame_LevelIndicator.text = $"Level {actualScene}";
    }

}