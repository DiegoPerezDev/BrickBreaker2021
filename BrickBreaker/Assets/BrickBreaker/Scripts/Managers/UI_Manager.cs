using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using MyTools;

public class UI_Manager : MonoBehaviour
{
    /*
    * - - - NOTES - - -
    - All GUI management for all scenes, Main menu, menu, HUD and else.
    - It has one WARNING comment in the variable declaration that need atention.
    */


    // - - MAIN MANAGEMENT - -

    // For transition management
    public static bool UI_Ready;
    private static UI_Manager instance;

    // For panel management
    [HideInInspector] public enum Panels { none, mainmenu, settings, inputs, HUD, messages, pause, lose, win }
    private static readonly GameObject[] panelGO = new GameObject[Enum.GetNames(typeof(Panels)).Length];
    private static readonly Panels[] activeMenus = new Panels[10];
    // -- -- -- WARNING: The 'panelsSceneName' must have the same order as the 'Panels' enum -- -- --
    private static readonly string[] panelsSceneName = { "", "Panel_Mainmenu", "Settings_Section/Panel_MainSettings", "Settings_Section/Panel_Inputs", "Panel_HUD", "Panel_Messages", "Panel_Pause", "Panel_Lose", "Panel_Win" };
    [HideInInspector] public enum Canvas { mainmenu, gameplayInplay, gameplayMenu };
    public static int currentMenuLayer;
    public static bool inPauseMenuFirstLayer;
    private static GameObject canvasMainmenu;

    // For button selection management
    private readonly static Button[] firstMenuButton = new Button[Enum.GetNames(typeof(Panels)).Length];
    private static EventSystem eventSystem;

    // Audio
    private static AudioSource audioSource;
    [HideInInspector] public enum UiAudioNames { button, pause, unPause }
    public static AudioClip[] uiClips = new AudioClip[Enum.GetNames(typeof(UiAudioNames)).Length];
    

    // - - - - GAMEPLAY - - - -

    // For HUD management
    private static TextMeshProUGUI lifeTmp;


    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;

        // UI audio
        audioSource = SearchTools.TryGetComponent<AudioSource>(this.gameObject);
        string[] uiClipsPaths = { "(UI1) button", "(UI2) Pause", "(UI3) UnPause" };
        foreach (UiAudioNames audioClip in Enum.GetValues(typeof(UiAudioNames)))
        {
            uiClips[(int)audioClip] = SearchTools.TryLoadResource($"Audio/UI/{uiClipsPaths[(int)audioClip]}") as AudioClip;
        }
    }


    // - - - - MAIN MANAGEMENT - - - -

    #region Start functions

    /// <summary>
    /// <para>Check if a panel is from a specific canvas, like if its part of the pause menu or the in play UI.</para>
    /// <para>This helps to not having to create many enum data in this script for the panels.</para>
    /// </summary>
    private static bool CheckIfPanelIsFromCanvas(int panelToCheck, int canvasType)
    {
        switch (canvasType)
        {
            case (int)Canvas.mainmenu:
                switch (panelToCheck)
                {
                    case (int)Panels.inputs:
                    case (int)Panels.mainmenu:
                    case (int)Panels.settings:
                        return true;
                    default:
                        return false;
                }

            case (int)Canvas.gameplayMenu:
                switch (panelToCheck)
                {
                    case (int)Panels.inputs:
                    case (int)Panels.lose:
                    case (int)Panels.pause:
                    case (int)Panels.settings:
                    case (int)Panels.win:
                        return true;
                    default:
                        return false;
                }

            case (int)Canvas.gameplayInplay:
                switch (panelToCheck)
                {
                    case (int)Panels.HUD:
                    case (int)Panels.messages:
                        return true;
                    default:
                        return false;
                }

            default:
                print("Scenetype not known, review again.");
                return false;
        }
    }

    #endregion

    #region Panel change management

    /// <summary>
    /// Go to a new panel. Deactivating or not deactivating the previous one.
    /// </summary>
    /// <param name="panelToGo">Panel gameObject to enable.</param>
    /// <param name="disablePrevPanel">Should we desable the previous panel?</param>
    public static void SwitchPanel(Panels panelToGo, bool disablePrevPanel)
    {
        // Remember previous panel
        Panels previousPanel = activeMenus[currentMenuLayer];

        // Set the panel to go and also which layer it represents
        switch (panelToGo)
        {
            case Panels.none:
                activeMenus[currentMenuLayer = 0] = Panels.none;
                break;

            case Panels.mainmenu:
                activeMenus[currentMenuLayer = 1] = Panels.mainmenu;
                break;
                
            case Panels.pause:
                activeMenus[currentMenuLayer = 1] = Panels.pause;
                inPauseMenuFirstLayer = true;
                break;

            case Panels.win:
                activeMenus[currentMenuLayer = 1] = Panels.win;
                break;

            case Panels.lose:
                activeMenus[currentMenuLayer = 1] = Panels.lose;
                break;

            case Panels.settings:
                activeMenus[currentMenuLayer = 2] = Panels.settings;
                break;

            case Panels.inputs:
                activeMenus[currentMenuLayer = 3] = Panels.inputs;
                break;
        }

        // Check if we are getting outside of the first menu panel
        if(inPauseMenuFirstLayer)
        {
            if (activeMenus[currentMenuLayer] != Panels.pause)
                inPauseMenuFirstLayer = false;
        }

        // Set previous panel. Evade disabling some panels that are not meant to be disabled.
        if ( (previousPanel == Panels.none) || (previousPanel == Panels.mainmenu))
            disablePrevPanel = false;

        //Actual switching
        TrySwitchPanel<Panels>(panelToGo, disablePrevPanel, previousPanel, panelGO);

        //Set event system for the new panel opened
        SetSelectedButton();
    }
    
    /// <summary>
    /// Close the current panel and go back to the previous one if there is one.
    /// </summary>
    public static void ReturnToPreviousPanel()
    {
        // Go to previous panel
        if (currentMenuLayer > 1)
            // close prev panel and open a new one
            TryReturnToPreviousPanel<Panels>(activeMenus[currentMenuLayer], panelGO, true, activeMenus[currentMenuLayer - 1]);
        else if (currentMenuLayer == 1)
            // close prev panel and DON'T open a new one
            TryReturnToPreviousPanel<Panels>(activeMenus[currentMenuLayer], panelGO, false, Panels.none);
        else
            // Avoid trying to return if we are at the first panel of all (the 'none' panel count as the first panel in the gameplay)
            return;

        // Set new fields values
        activeMenus[currentMenuLayer] = Panels.none;
        currentMenuLayer--;

        // accesing to Pause menu in gameplay
        if (activeMenus[currentMenuLayer] == Panels.pause)
            inPauseMenuFirstLayer = true;

        //Set event system for the new panel opened
        SetSelectedButton();
    }

    /// <summary>
    /// Change between panels, verifing the existance of those to go and to close.
    /// </summary>
    /// <typeparam name="T">Enum type for panels. Is a type so we can make many enums if the case is needed.</typeparam>
    private static void TrySwitchPanel<T>(T panelToGo, bool disablePrevPanel, T previousPanel, GameObject[] panelGameObject)
    {
        // Enable new panel
        try
        {
            panelGameObject[(int)((object)panelToGo)].SetActive(true);
        }
        catch (ArgumentNullException)
        {
            Debug.Log("Could not make the panel switching because of a null exception");
        }
        catch (Exception)
        {
            Debug.Log("Could not make the panel switching for the panel named: " + panelToGo.ToString());
        }

        // Disable previus panel if requested
        if (disablePrevPanel)
        {
            try
            {
                panelGameObject[(int)((object)previousPanel)].SetActive(false);
            }
            catch (ArgumentNullException)
            {
                Debug.Log("Could not disable previous panel because of a null exception");
            }
            catch (Exception)
            {
                Debug.Log("Could not disable previous panel for the panel named: " + previousPanel.ToString());
            }
        }
    }

    /// <summary>
    /// Close the current panel and go back to the previous one if needed, verifing the existance of it.
    /// </summary>
    /// <typeparam name="T">Enum type for panels. Is a type so we can make many enums if the case is needed.</typeparam>
    private static void TryReturnToPreviousPanel<T>(T activePanel, GameObject[] panelGameObject, bool closePrevPanel, T previousPanel)
    {
        // Audio
        AudioManager.PlayAudio(audioSource, uiClips[(int)UiAudioNames.button], false, 1f);

        // Open previus panel
        if(closePrevPanel)
        {
            GameObject panelToActivate = panelGameObject[((int)(object)previousPanel)];
            if (panelToActivate != null)
                panelToActivate.SetActive(true);
        }

        // Close active panel
        GameObject panelToDesactivate = panelGameObject[((int)(object)activePanel)];
        if (panelToDesactivate != null)
            panelToDesactivate.SetActive(false);
    }

    #endregion

    #region Button selection management

    /// <summary>
    /// <para>Select the first button of the current panel. This is needed for UI navigation when we are not navigating with mouse or taps (in case of smartphone).</para>
    /// <para>We previously stored the first buttons in variables in the 'SetMainmenuFirstButtons()' and the 'SetGameplayFirstButtons()' functions.</para>
    /// </summary>
    private static void SetSelectedButton()
    {
        // Dont try to select button for the buttonless menus
        switch (activeMenus[currentMenuLayer])
        {
            case Panels.HUD:
            case Panels.messages:
            case Panels.none:
                eventSystem.SetSelectedGameObject(null);
                return;
        }

        // Select the first button of the active panel
        if(firstMenuButton[(int)activeMenus[currentMenuLayer]])
            firstMenuButton[(int)activeMenus[currentMenuLayer]].Select();
    }

    #endregion


    // - - - - MAIN MENU - - - -

    #region Start functions

    /// <summary>
    /// Set all the needed things for the UI in the main menu before closing the loading screen when entering the main menu scene.
    /// </summary>
    public static void SetMainmenu()
    {
        // We are now at the main menu
        currentMenuLayer = 1;
        activeMenus[currentMenuLayer] = Panels.mainmenu;

        // Set event system for UI selection
        GameObject eventSystemTempGO = SearchTools.TryFind("UI/EventSystem");
        eventSystem = SearchTools.TryGetComponent<EventSystem>(eventSystemTempGO);

        // Get panels components
        canvasMainmenu = SearchTools.TryFind("UI/UI_Mainmenu/Canvas_Mainmenu");
        for (int i = 0; i < Enum.GetValues(typeof(Panels)).Length; i++)
        {
            if (CheckIfPanelIsFromCanvas(i, (int)Canvas.mainmenu))
            {
                panelGO[i] = SearchTools.TryFindInGameobject(canvasMainmenu, panelsSceneName[i]);
                panelGO[i].SetActive(false);
            }
        }
        panelGO[(int)Panels.mainmenu].SetActive(true);

        // Set buttons
        AssignMainmenuButtonsListener();
        SetMainmenuFirstButtons();
        SetSelectedButton();

        // Tell the 'GameManager' that the UI is already set.
        UI_Ready = true;
    }

    #endregion

    #region Button functions

    /// <summary>
    /// This triggers the action when clicking a button from the main menu.
    /// </summary>
    private static void MainMenuButtonsActions(string button)
    {
        AudioManager.PlayAudio(audioSource, uiClips[(int)UiAudioNames.button], false, 1f);
        button = button.ToLower();
        button = button.Trim();

        switch (button)
        {
            case "play":
                GameManager.GoToScene(2);
                break;

            case "settings":
                SwitchPanel(Panels.settings, true);
                break;

            case "inputs":
                SwitchPanel(Panels.inputs, true);
                break;

            case "return":
                ReturnToPreviousPanel();
                break;

            case "quit":
                print("quitting application");
                Application.Quit();
                break;

            default:
                print("Wrong use of the MainMenu buttons function");
                Debug.Break();
                break;
        }
    }

    /// <summary>
    /// This assing the event when clicking the buttons for the main menu, so we dont have to do it manually in the inspector every time we start a new game.
    /// </summary>
    private static void AssignMainmenuButtonsListener()
    {
        // - - MAIN MENU PANEL - -
        string panelPath = "UI/UI_Mainmenu/Canvas_Mainmenu/Panel_Mainmenu";
        Button tempButton;

        // Play button
        tempButton = GameObject.Find(panelPath + "/ButtonPlay").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { MainMenuButtonsActions("play"); });

        // Settings button
        tempButton = GameObject.Find(panelPath + "/ButtonSettings").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { MainMenuButtonsActions("settings"); });

        // Quit button
        tempButton = GameObject.Find(panelPath + "/ButtonQuit").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { MainMenuButtonsActions("quit"); });


        // - - SETTINGS PANEL - -
        panelPath = "UI/UI_Mainmenu/Canvas_Mainmenu/Settings_Section/Panel_MainSettings";

        // Inputs button
        tempButton = GameObject.Find(panelPath + "/ButtonInputs").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { MainMenuButtonsActions("inputs"); });

        // Return button
        tempButton = GameObject.Find(panelPath + "/ButtonReturn").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { MainMenuButtonsActions("return"); });


        // - - INPUTS PANEL - -
        panelPath = "UI/UI_Mainmenu/Canvas_Mainmenu/Settings_Section/Panel_Inputs";

        // Return button
        tempButton = GameObject.Find(panelPath + "/ButtonReturn").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { MainMenuButtonsActions("return"); });
    }

    /// <summary>
    /// Store the button component of the first buttons of each main menu panel. For using the 'SetSelectedButton()' function.
    /// </summary>
    private static void SetMainmenuFirstButtons()
    {
        string buttonPath;
        for (int i = 1; i < Enum.GetValues(typeof(Panels)).Length; i++)
        {
            switch (i)
            {
                case (int)Panels.mainmenu:
                    buttonPath = "Panel_Mainmenu/ButtonPlay";
                    break;

                case (int)Panels.settings:
                    buttonPath = "Settings_Section/Panel_MainSettings/ButtonInputs";
                    break;

                case (int)Panels.inputs:
                    buttonPath = "Settings_Section/Panel_Inputs/ButtonExample";
                    break;

                default:
                    continue;
            }

            // Find the first buttons of the desired menus with error handle
            GameObject buttonGO = SearchTools.TryFindInGameobject(canvasMainmenu, buttonPath);
            firstMenuButton[i] = SearchTools.TryGetComponent<Button>(buttonGO);
        }
    }

    #endregion


    // - - - - GAMEPLAY - - - -

    #region Start functions

    /// <summary>
    /// Set all the needed things for the UI in the gameplay scenes before closing the loading screen when entering those scenes.
    /// </summary>
    public static void SetGameplay()
    {
        // We are now at the gameplay play time
        currentMenuLayer = 0;
        activeMenus[currentMenuLayer] = Panels.none;

        // Set event system for UI selection
        GameObject eventSystemTempGO = SearchTools.TryFind("UI/EventSystem");
        eventSystem = SearchTools.TryGetComponent<EventSystem>(eventSystemTempGO);

        // Set UI for both of the gameplay Canvas
        SetGameplayInPlay();
        SetGameplayMenu();

        // Set buttons
        AssignGameplayButtonsListener();
        SetGameplayFirstButtons();
        SetSelectedButton();

        // Set Everithing for the HUD and else
        StartInplayUI();

        // Tell the 'Game manager' that the UI is ready so it can close the loading panel.
        UI_Ready = true;
    }

    private static void SetGameplayInPlay()
    {
        // Get panels for gameplay 'inplay' (the UI that happens in the play time, not the pause menu)
        GameObject canvasGameplay = SearchTools.TryFind("UI/UI_Gameplay/Canvas_InPlay");
        for (int i = 0; i < Enum.GetValues(typeof(Panels)).Length; i++)
        {
            if (CheckIfPanelIsFromCanvas(i, (int)Canvas.gameplayInplay))
                panelGO[i] = SearchTools.TryFindInGameobject(canvasGameplay, panelsSceneName[i]);
        }

        // Get gameplay 'inplay' components
        GameObject lifeTmpGO = SearchTools.TryFindInGameobject(panelGO[(int)Panels.HUD], "Life");
        lifeTmp = SearchTools.TryGetComponent<TextMeshProUGUI>(lifeTmpGO);
    }

    private static void SetGameplayMenu()
    {
        // Get panels for gameplay
        GameObject canvasGameplayMenu = SearchTools.TryFind("UI/UI_Gameplay/Canvas_Menu");
        for (int i = 0; i < Enum.GetValues(typeof(Panels)).Length; i++)
        {
            if (CheckIfPanelIsFromCanvas(i, (int)Canvas.gameplayMenu))
            {
                panelGO[i] = SearchTools.TryFindInGameobject(canvasGameplayMenu, panelsSceneName[i]);
                panelGO[i].SetActive(false);
            }
        }

        // Find the first buttons of the desired menus
        GameObject eventSystemGO = SearchTools.TryFind("UI/EventSystem");
        eventSystem = SearchTools.TryGetComponent<EventSystem>(eventSystemGO);
        SetGameplayFirstButtons();
    }

    #endregion

    #region Button functions

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
                GameManager.returnTrigger = true;
                AudioManager.PlayAudio(audioSource, uiClips[(int)UiAudioNames.unPause], false, 1f);
                return;

            case "settings":
                SwitchPanel(Panels.settings, true);
                break;

            case "inputs":
                SwitchPanel(Panels.inputs, true);
                break;

            case "return":
                ReturnToPreviousPanel();
                break;

            case "quit":
                print("quit application");
                AudioManager.PlayAudio(audioSource, uiClips[(int)UiAudioNames.button], false, 1f);
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

        AudioManager.PlayAudio(audioSource, uiClips[(int)UiAudioNames.button], false, 1f);
    }

    /// <summary>
    /// This assing the event when clicking the buttons for the gameplay menus, so we dont have to do it manually in the inspector every time we start a new game.
    /// </summary>
    private static void AssignGameplayButtonsListener()
    {
        // - - PAUSE PANEL - -
        string panelPath = "UI/UI_Gameplay/Canvas_Menu/Panel_Pause";
        Button tempButton;

        // Continue button
        tempButton = GameObject.Find(panelPath + "/ButtonContinue").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("continue"); });

        // Restart button
        tempButton = GameObject.Find(panelPath + "/ButtonRestart").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("restart"); });

        // Settings button
        tempButton = GameObject.Find(panelPath + "/ButtonSettings").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("settings"); });

        // Main menu button
        tempButton = GameObject.Find(panelPath + "/ButtonMainmenu").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("mainmenu"); });

        // Quit button
        tempButton = GameObject.Find(panelPath + "/ButtonQuit").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("quit"); });


        // - - WIN PANEL - -
        panelPath = "UI/UI_Gameplay/Canvas_Menu/Panel_Win";

        // Next level button
        GameObject tempButtonGo = GameObject.Find(panelPath + "/ButtonNextLevel");
        if (tempButtonGo)
        {
            tempButton = tempButtonGo.GetComponent<Button>();
            tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("nextlevel"); });
        }

        // Main menu button
        tempButton = GameObject.Find(panelPath + "/ButtonMainmenu").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("mainmenu"); });


        // - - LOSE PANEL - -
        panelPath = "UI/UI_Gameplay/Canvas_Menu/Panel_Lose";

        // Restart button
        tempButton = GameObject.Find(panelPath + "/ButtonRestart").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("restart"); });

        // Main menu button
        tempButton = GameObject.Find(panelPath + "/ButtonMainmenu").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("mainmenu"); });


        // - - SETTINGS PANEL - -
        panelPath = "UI/UI_Gameplay/Canvas_Menu/Settings_Section/Panel_MainSettings";

        // Inputs button
        tempButton = GameObject.Find(panelPath + "/ButtonInputs").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("inputs"); });

        // Return button
        tempButton = GameObject.Find(panelPath + "/ButtonReturn").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("return"); });


        // - - INPUTS PANEL - -
        panelPath = "UI/UI_Gameplay/Canvas_Menu/Settings_Section/Panel_Inputs";

        // Return button
        tempButton = GameObject.Find(panelPath + "/ButtonReturn").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { PauseMenuButtonsActions("return"); });
    }

    /// <summary>
    /// Store the button component of the first buttons of each gameplay panel. For using the 'SetSelectedButton()' function.
    /// </summary>
    private static void SetGameplayFirstButtons()
    {
        // Find the first buttons of the desired menus
        string buttonPath;
        for (int i = 1; i < Enum.GetValues(typeof(Panels)).Length; i++)
        {
            switch (i)
            {
                case (int)Panels.pause:
                    buttonPath = "Panel_Pause/ButtonContinue";
                    break;

                case (int)Panels.win:
                    buttonPath = "Panel_Win/ButtonNextLevel";
                    GameObject tempButton = GameObject.Find($"UI/UI_Gameplay/Canvas_Menu/{buttonPath}");
                    if(tempButton == null)
                        buttonPath = "Panel_Win/ButtonMainmenu";
                    break;

                case (int)Panels.lose:
                    buttonPath = "Panel_Lose/ButtonRestart";
                    break;

                case (int)Panels.settings:
                    buttonPath = "Settings_Section/Panel_MainSettings/ButtonInputs";
                    break;

                case (int)Panels.inputs:
                    buttonPath = "Settings_Section/Panel_Inputs/ButtonExample";
                    break;

                default:
                    continue;
            }

            GameObject canvasPausemenu = SearchTools.TryFind("UI/UI_Gameplay/Canvas_Menu");
            GameObject buttonGO = SearchTools.TryFindInGameobject(canvasPausemenu, buttonPath);
            firstMenuButton[i] = SearchTools.TryGetComponent<Button>(buttonGO);
        }
    }

    #endregion

    #region HUD

    private static void StartInplayUI()
    {
        // Life
        if (lifeTmp != null)
            lifeTmp.text = "Life: " + LevelManager.lives;
    }

    public static void RewriteLife()
    {
        lifeTmp.text = "Life: " + LevelManager.lives;
    }

    #endregion

}