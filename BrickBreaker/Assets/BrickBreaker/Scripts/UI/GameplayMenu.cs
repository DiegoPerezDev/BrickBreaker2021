using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using MyTools;
using UnityEngine.Audio;

public class GameplayMenu : MonoBehaviour
{
    /*
     * INSTRUCTIONS:
     * This codes manage the menu setup, the buttons listeners and actions nad the level pause.
     * The specific behaviour of some data (e.g.  life) are done in other UI codes.
     */


    // - - MAIN MANAGEMENT - -

    // - - - Transition management - - -
    public static bool ready;

    // - - - Panel management - - -
    // WARNING: The 'panelsSceneName' must have the same order as the 'Panels' enum
    [HideInInspector] public enum Panels { none, pause, lose, win, quitConfirmation }
    private static readonly string[] panelsSceneName = { " ", "Panel_Pause", "Panel_Lose", "Panel_Win", "Panel_QuitConfirmation" };
    public static GameObject[] panelGO = new GameObject[Enum.GetNames(typeof(Panels)).Length];
    public static Panels[] openedMenus = new Panels[10];

    // - - - HUD - - -
    public static GameObject unstuckButton;

    // - - - Pause menu - - -
    public AudioMixer musicMixer, SFX_Mixer;
    public Slider musicSlider, sfxSlider;
    private static float musicVol = 0f, sfxVol = 0f;

    // - - - - Win menu - - - -
    [SerializeField] private bool lastLevel;


    void Awake()
    {
        // We are now at the gameplay play time
        UI_Manager.currentMenuLayer = 0;
        for (int i = 0; i < openedMenus.Length; i++)
            openedMenus[i] = Panels.none;
    }

    void Start()
    {
        // Set UI for both of the gameplay Canvas
        SetHUD();
        SetPauseMenu(ref panelGO, panelsSceneName);
        CheckIfLastLevel();

        // Tell the 'Game manager' that the UI is ready so it can close the loading panel.
        ready = true;
    }

    private void OnDestroy()
    {
        ready = false;
        for (int i = 0; i < openedMenus.Length; i++)
            openedMenus[i] = Panels.none;
    }


    #region HUD

    public static void SetHUD()
    {
        // Get gameplay HUD components
        unstuckButton = SearchTools.TryFind("UI/Canvas_HUD/ButtonUnstuck");
        unstuckButton.SetActive(false);

        // Set buttons
        AssignHUDButtonsListener();
    }

    private static void AssignHUDButtonsListener()
    {
        // - - - - HUD CANVAS - - - -

        Button tempButton;
        string panelPath = "UI/Canvas_HUD";

        // Unstuck button
        tempButton = GameObject.Find(panelPath + "/ButtonUnstuck").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { HUDButtonsActions("unstuck"); });

        // - - - - RIGHTBLOCK PANEL - - - -

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
                UI_Manager.ClosePanel();
                break;

            case "quit":
                UI_Manager.OpenMenuLayer<Panels>(Panels.quitConfirmation);
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

    public void SetMusicVolume(float volume) => musicMixer.SetFloat("volume", musicVol = volume);

    public void SetSFX_Volume(float volume) => SFX_Mixer.SetFloat("volume", sfxVol = volume);

    #endregion

    #region general

    private void CheckIfLastLevel()
    {
        if (lastLevel)
        {
            GameObject nextLevelButton = SearchTools.TryFind("UI/Canvas_Menu/Panel_Win/ButtonNextLevel");
            Destroy(nextLevelButton);
        }
    }

    #endregion
}