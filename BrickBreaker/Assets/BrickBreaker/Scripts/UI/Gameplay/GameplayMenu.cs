using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MyTools;
using UnityEngine.Audio;

public class GameplayMenu : MonoBehaviour
{
    /*
     * INSTRUCTIONS:
     * This codes manage the menu setup, the buttons listeners and actions nad the level pause.
     * The specific behaviour of some data (e.g.  life) are done in other UI codes.
     */


    // - - - GENERAL DATA - - -
    public static bool ready;

    // WARNING: The 'panelsSceneName' must have the same order as the 'Panels' enum
    [HideInInspector] public enum Panels { none, pause, lose, win, quitConfirmation }
    private static readonly string[] panelsSceneName = { " ", "Panel_Pause", "Panel_Lose", "Panel_Win", "Panel_QuitConfirmation" };
    public static GameObject[] panelGO = new GameObject[Enum.GetNames(typeof(Panels)).Length];
    public static Panels[] openedMenus = new Panels[10];

    // - - - HUD Buttons - - -
    public static GameObject unstuckButton;
    public static GameObject launchButton;

    // - - - Pause menu - - -
    public AudioMixer musicMixer, SFX_Mixer;
    public Slider musicSlider, sfxSlider;
    private static float musicVol = -5f, sfxVol = -10f;


    void Awake()
    { 
        // We are now at the gameplay play time
        UI_Manager.currentMenuLayer = 0;
        for (int i = 0; i < openedMenus.Length; i++)
            openedMenus[i] = Panels.none;

        launchButton = SearchTools.TryFind("UI/Canvas_HUD/Panel_LeftBlock/LaunchBallButton");
    }

    void Start()
    {
        SetHUD();
        SetPauseMenu(ref panelGO, panelsSceneName);
        if (SceneManager.GetActiveScene().buildIndex >= (LevelManager.maxLevels + 1) )
        {
            GameObject nextLevelButton = SearchTools.TryFind("UI/Canvas_Menu/Panel_Win/ButtonNextLevel");
            Destroy(nextLevelButton);
        }
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
    public static void OpenMenuLayer(Panels panelToGo)
    {
        if (panelGO[(int)panelToGo] != null)
        {
            // Set manager info
            UI_Manager.inMenu = true;
            UI_Manager.currentMenuLayer++;

            // Remember menu
            openedMenus[UI_Manager.currentMenuLayer] = panelToGo;

            // Open menu
            panelGO[(int)((object)panelToGo)].SetActive(true);
        }
        else
            print($"Could not open the panel: '{panelToGo}', because of a null exception");
    }

    public static void ClosePanel()
    {
        // Set manager info
        if (UI_Manager.currentMenuLayer > 0)
            UI_Manager.currentMenuLayer--;
        else
        {
            print("Could not close panel because there is no opened panel");
            return;
        }

        Panels panelToClose = openedMenus[UI_Manager.currentMenuLayer + 1];
        if (panelGO[(int)panelToClose] != null)
        {
            //Set info of the opened panels
            openedMenus[UI_Manager.currentMenuLayer + 1] = Panels.none;
            if (UI_Manager.currentMenuLayer == 0)
                UI_Manager.inMenu = false;

            // Close panel
            panelGO[(int)panelToClose].SetActive(false);
        }
        else
            print($"Could not close the panel because of a null exception.");
    }

    /// <summary>
    /// Go to the previous menu if its in a below layer. In case of the pause main menu, close it and unpause the game.
    /// </summary>
    private static void ReturnToPreviousMenu()
    {
        if (UI_Manager.inMenu)
        {
            AudioManager.PlayAudio(AudioManager.GameAudioSource, UI_Manager.uiClips[(int)UI_Manager.UiAudioNames.unPause], false, 1f);

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
        unstuckButton = SearchTools.TryFind("UI/Canvas_HUD/Panel_LeftBlock/ButtonUnstuck");
        unstuckButton.SetActive(false);

        // Set buttons
        AssignHUDButtonsListener();
    }

    private static void AssignHUDButtonsListener()
    {
        Button tempButton;
        string panelPath = "UI/Canvas_HUD/Panel_LeftBlock";

        // Unstuck button
        tempButton = GameObject.Find(panelPath + "/ButtonUnstuck").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { HUDButtonsActions("unstuck"); });

        // Launch ball button
        tempButton = GameObject.Find(panelPath + "/LaunchBallButton").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { HUDButtonsActions("launchball"); });

        panelPath = "UI/Canvas_HUD/Panel_RightBlock";

        // Pause button
        tempButton = GameObject.Find(panelPath + "/PauseButton").GetComponent<Button>();
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
                return;

            case "unstuck":
                Ball.unstuckBallTrigger = true;
                Ball.hitObject = true;
                break;

            case "launchball":
                InputsManager.releaseBall = true;
                launchButton.SetActive(false);
                break;

            default:
                print("attempt didn't work");
                return;
        }

        AudioManager.PlayAudio(UI_Manager.audioSource, UI_Manager.uiClips[(int)UI_Manager.UiAudioNames.button], false, 1f);
    }

    #endregion

    #region Pause menu

    public void SetPauseMenu(ref GameObject[] panelGO, string[] panelsSceneName)
    {
        // Get panels for gameplay
        GameObject canvasGameplayMenu = SearchTools.TryFind("UI/Canvas_Menu");
        for (int i = 1; i < Enum.GetValues(typeof(Panels)).Length; i++)
        {
            panelGO[i] = SearchTools.TryFindInGameobject(canvasGameplayMenu, panelsSceneName[i]);
            panelGO[i].SetActive(false);
        }

        // Set buttons
        AssignMenuButtonsListener();

        // Set audio
        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;
        SetMusicVolume(musicVol);
        SetSFX_Volume(sfxVol);
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
                OpenMenuLayer(Panels.quitConfirmation);
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

        string panelPath = "UI/Canvas_Menu/Panel_Pause";
        Button tempButton;

        // Continue button
        tempButton = GameObject.Find(panelPath + "/ButtonContinue").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("continue"); });

        // Restart button
        tempButton = GameObject.Find(panelPath + "/ButtonRestart").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("restart"); });

        // Main menu button
        tempButton = GameObject.Find(panelPath + "/ButtonMainmenu").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("mainmenu"); });

        // Quit button
        tempButton = GameObject.Find(panelPath + "/ButtonQuit").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("quit"); });


        // - - - - QUIT CONFIRMATION PANEL - - - -

        panelPath = "UI/Canvas_Menu/Panel_QuitConfirmation";

        // Confirm button
        tempButton = GameObject.Find(panelPath + "/ButtonConfirm").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("quitconfirm"); });

        // Return button
        tempButton = GameObject.Find(panelPath + "/ButtonReturn").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("return"); });


        // - - - - WIN PANEL - - - -

        panelPath = "UI/Canvas_Menu/Panel_Win";

        // Next level button
        tempButton = GameObject.Find(panelPath + "/ButtonNextLevel").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("nextlevel"); });

        // Main menu button
        tempButton = GameObject.Find(panelPath + "/ButtonMainmenu").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("mainmenu"); });


        // - - - - LOSE PANEL - - - -

        panelPath = "UI/Canvas_Menu/Panel_Lose";

        // Restart button
        tempButton = GameObject.Find(panelPath + "/ButtonRestart").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("restart"); });

        // Main menu button
        tempButton = GameObject.Find(panelPath + "/ButtonMainmenu").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("mainmenu"); });
    }

    public void SetMusicVolume(float volume)
    {
        // NOTA: valores optimos entre 20 y -20. Totales entre 20 y -80
        musicVol = volume;
        float fixedVolume;
        if (volume >= -70)
            fixedVolume = MapValue(volume, -70, 20, -20, 10);
        else
            fixedVolume = MapValue(volume, -80, -70, -80, -20);
        musicMixer.SetFloat("volume", fixedVolume);
    }

    public void SetSFX_Volume(float volume)
    {
        // NOTA: valores optimos entre 20 y -20. Totales entre 20 y -80
        sfxVol = volume;
        float fixedVolume;
        if (volume >= -70)
            fixedVolume = MapValue(volume, -70, 20, -20, 10);
        else
            fixedVolume = MapValue(volume, -80, -70, -80, -20);
        SFX_Mixer.SetFloat("volume", fixedVolume);
    }

    float MapValue(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    #endregion

    public void RestartUI()
    {
        Awake();
        Start();
    }

}