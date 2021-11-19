using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using MyTools;


public class MainMenu_UI : MonoBehaviour
{
    /*
     * INSTRUCTIONS:
     * This class manage the UI of the main menu, for the behaviour of the scene itself use the 'MainMenuManager' script instead.
     * It manage the UI by using the 'UI_Manager' script.
     */

    // Panel management
    // -- WARNING: The 'panelsSceneName' must match the same order as the 'Panels' enum --
    [HideInInspector] public enum Panels { none, mainmenu, levelSelection, quitConfirmation }
    private static readonly string[] panelsSceneName = { " ", "Panel_Mainmenu", "Panel_LevelSelection", "Panel_QuitConfirmation" };
    public static readonly GameObject[] panelGO = new GameObject[Enum.GetNames(typeof(Panels)).Length];
    public static Panels[] openedMenus = new Panels[10];
    public static bool ready;

    // Level selection 
    private GameObject[] levelButtons = new GameObject[LevelManager.maxLevels];
    private GameObject[] levelScore = new GameObject[LevelManager.maxLevels];
    private Sprite[] starImages = new Sprite[4];


    void Awake()
    {
        // Get stars images for score
        for (int i = 0; i < starImages.Length; i++)
            starImages[i] = Resources.Load<Sprite>($"Art2D/Stars/{i}stars");

        // Get panels components and leave only the main one
        GameObject canvasMainmenu = SearchTools.TryFind("UI/Canvas_Mainmenu");
        for (int i = 1; i < Enum.GetValues(typeof(Panels)).Length; i++)
        {
            panelGO[i] = SearchTools.TryFindInGameobject(canvasMainmenu, panelsSceneName[i]);
            if (i == (int)Panels.mainmenu)
                continue;
            if (i == (int)Panels.levelSelection)
                SetLevelSelectionPanel(panelGO[i]);
            panelGO[i].SetActive(false);
        }
    }

    void Start()
    {
        // Set buttons
        AssignMainmenuButtonsListener();

        // Set menus
        UI_Manager.currentMenuLayer = 1;
        for (int i = 0; i < openedMenus.Length; i++)
            openedMenus[i] = Panels.none;
        openedMenus[1] = Panels.mainmenu;

        // Tell the 'MainmenuManager' that the UI is already set.
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
        ready = false;
        for (int i = 0; i < openedMenus.Length; i++)
            openedMenus[i] = Panels.none;
        openedMenus[1] = Panels.mainmenu;
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
    /// Go to the previous menu if its in a below layer.
    /// </summary>
    private static void ReturnToPreviousMenu()
    {
        if (UI_Manager.inMenu)
        {
            AudioManager.PlayAudio(AudioManager.GameAudioSource, UI_Manager.uiClips[(int)UI_Manager.UiAudioNames.unPause], false, 1f);
            ClosePanel();
        }
    }

    #endregion

    #region Buttons

    /// <summary>
    /// This triggers the event when clicking a button from the main menu.
    /// </summary>
    private static void MainMenuButtonsActions(string button)
    {
        AudioManager.PlayAudio(UI_Manager.audioSource, UI_Manager.uiClips[(int)UI_Manager.UiAudioNames.button], false, 1f);

        button = button.ToLower();
        button = button.Trim();

        switch (button)
        {
            case "play":
                OpenMenuLayer(Panels.levelSelection);
                break;

            case "quit":
                OpenMenuLayer(Panels.quitConfirmation);
                break;

            case "quitconfirm":
                print("quit application");
                Application.Quit();
                return;

            case "return":
                ClosePanel();
                break;

            default: //level selection buttons
                int levelNum;
                if (int.TryParse(button.Substring(5, 2), out levelNum))
                {
                    if (levelNum >= 1)
                    {
                        string temp = button.Substring(0, 5).ToLower();
                        if (temp == "level")
                        {
                            GameManager.GoToScene(levelNum + 1);
                            break;
                        }
                    }
                }
                print("Wrong use of the MainMenu buttons function");
                Debug.Break();
                break;
        }
    }

    /// <summary>
    /// This assing an event that happens when clicking the main menu buttons.
    /// </summary>
    private static void AssignMainmenuButtonsListener()
    {
        // - - MAIN PANEL - -

        string panelPath = "UI/Canvas_Mainmenu/Panel_Mainmenu";
        Button tempButton;

        // Play button
        tempButton = GameObject.Find($"{panelPath}/ButtonPlay").GetComponent<Button>();tempButton.onClick.AddListener(delegate { MainMenuButtonsActions("play"); });

        // Quit button
        tempButton = GameObject.Find($"{panelPath}/ButtonQuit").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { MainMenuButtonsActions("quit"); });


        // - - - - QUIT CONFIRMATION PANEL - - - -

        panelPath = "UI/Canvas_Mainmenu/Panel_QuitConfirmation";

        // Confirm button
        tempButton = GameObject.Find($"{panelPath}/ButtonConfirm").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { MainMenuButtonsActions("quitconfirm"); });

        // Return button
        tempButton = GameObject.Find($"{panelPath}/ButtonReturn").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { MainMenuButtonsActions("return"); });


        // - - - - LEVEL SELECTION PANEL - - - -

        panelPath = "UI/Canvas_Mainmenu/Panel_LevelSelection";

        // Level buttons
        for(int i = 1; i < 11; i++)
        {
            string j;
            if (i < 10)
                j = $"0{i}";
            else
                j = i.ToString();
            tempButton = GameObject.Find($"{panelPath}/ButtonLevel{i}").GetComponent<Button>();
            tempButton.onClick.AddListener(delegate { MainMenuButtonsActions($"level{j}"); });
        }

        // Return button
        tempButton = GameObject.Find($"{panelPath}/ButtonReturn").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { MainMenuButtonsActions("return"); });
    }

    #endregion 

    private void SetLevelSelectionPanel(GameObject panel)
    {
        // Get level buttons and score images game object
        for (int i = 1; i < LevelManager.maxLevels + 1; i++)
        {
            levelButtons[i - 1] = SearchTools.TryFindInGameobject(panel, $"ButtonLevel{i}");
            levelScore[i - 1] = SearchTools.TryFindInGameobject(panel, $"ScoreLevel{i}");
        }

        // Set level buttons and score images game object
        for (int i = 0; i < LevelManager.maxLevels; i++)
        {
            if( (i + 1) > LevelManager.levelsUnlocked)
            {
                levelButtons[i].SetActive(false);
                levelScore[i].SetActive(false);
            }
        }

        // Put the respective score image for each level score
        for (int i = 0; i < LevelManager.levelsUnlocked; i++)
            levelScore[i].GetComponent<Image>().sprite = starImages[LevelManager.levelsScore[i]];
    }

}