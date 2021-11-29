using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class MainMenu_UI : PanelManagement
{
    /*
     * INSTRUCTIONS:
     * This class manage the UI of the main menu, for the behaviour of the scene itself use the 'MainMenuManager' script instead.
     * It manage the UI by using the 'UI_Manager' script.
     */

    // General management
    private static MainMenu_UI instance;
    [SerializeField] private bool standaloneUI, smartphoneUI;

    // Panel management
    // -- WARNING: The 'panelsSceneName' must match the same order as the 'Panels' enum --
    [HideInInspector] public enum Panels { none, mainmenu, levelSelection, quitConfirmation }
    private static readonly string[] panelsSceneName = { " ", "Panel_Mainmenu", "Panel_LevelSelection", "Panel_QuitConfirmation" };
    public static readonly GameObject[] panelGO = new GameObject[Enum.GetNames(typeof(Panels)).Length];
    public static Panels[] openedMenus = new Panels[10];
    public static bool ready;

    // Level selection 
    private Button[] levelButtons = new Button[LevelManager.maxLevels];
    private GameObject[] levelScore = new GameObject[LevelManager.maxLevels];
    private Sprite[] starImages = new Sprite[4];


    void Awake()
    {
        //Set UI prefab depending on the OS
        SetOS_UI();
    }

    void Start()
    {
        // Get stars images for score
        for (int i = 0; i < starImages.Length; i++)
            starImages[i] = Resources.Load<Sprite>($"Art2D/Stars/{i}stars");

        // Get panels components and leave only the main one
        GameObject canvasMainmenu = transform.Find("Canvas_Mainmenu").gameObject;
        for (int i = 1; i < Enum.GetValues(typeof(Panels)).Length; i++)
        {
            panelGO[i] = canvasMainmenu.transform.Find(panelsSceneName[i]).gameObject;
            if (i == (int)Panels.mainmenu) // Dont close tha main menu
                continue;
            panelGO[i].SetActive(false); // Close every other menu panel
        }

        // UI manager
        UI_Manager.currentPanel = panelGO[1];
        UI_Manager.inMenu = true;

        // Set buttons
        AssignMainmenuButtonsListener();

        // Set menus
        UI_Manager.currentMenuLayer = 1;
        for (int i = 0; i < openedMenus.Length; i++)
            openedMenus[i] = Panels.none;
        openedMenus[1] = Panels.mainmenu;

        // Set level selection menu buttons
        SetLevelSelectionPanel(panelGO[(int)Panels.levelSelection]);

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
            if (UI_Manager.currentMenuLayer > 1)
                ClosePanel();
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
    private static void OpenMenu(Panels panelToGo)
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

    private static void ClosePanel()
    {
        GameObject panelToOpenGO = panelGO[(int)(openedMenus[UI_Manager.currentMenuLayer - 1])];
        GameObject panelToCloseGO = panelGO[(int)openedMenus[UI_Manager.currentMenuLayer]];
        //print($"Opening panel: {openedMenus[UI_Manager.currentMenuLayer - 1]}");
        //print($"Closing panel panel: {openedMenus[UI_Manager.currentMenuLayer]}");
        ReturnToPreviousPanel(panelToOpenGO, panelToCloseGO);
    }

    #endregion

    #region Buttons

    /// <summary>
    /// This triggers the event when clicking a button from the main menu.
    /// </summary>
    private static void MainMenuButtonsActions(string button)
    {
        button = button.ToLower();
        button = button.Trim();

        switch (button)
        {
            case "play":
                OpenMenu(Panels.levelSelection);
                break;

            case "quit":
                OpenMenu(Panels.quitConfirmation);
                break;

            case "quitconfirm":
                print("quit application");
                Application.Quit();
                return;

            case "return":
                ClosePanel();
                return;

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

        AudioManager.PlayAudio(UI_Manager.audioSource, UI_Manager.uiClips[(int)UI_Manager.UiAudioNames.button], false, 1f);
    }

    /// <summary>
    /// This assing an event that happens when clicking the main menu buttons.
    /// </summary>
    private static void AssignMainmenuButtonsListener()
    {
        // - - MAIN PANEL - -

        string panelPath = "Canvas_Mainmenu/Panel_Mainmenu";
        Button tempButton;

        // Play button
        tempButton = instance.transform.Find($"{panelPath}/ButtonPlay").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { MainMenuButtonsActions("play"); });

        // Quit button
        tempButton = instance.transform.Find($"{panelPath}/ButtonQuit").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { MainMenuButtonsActions("quit"); });


        // - - - - QUIT CONFIRMATION PANEL - - - -

        panelPath = "Canvas_Mainmenu/Panel_QuitConfirmation";

        // Confirm button
        tempButton = instance.transform.Find($"{panelPath}/ButtonConfirm").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { MainMenuButtonsActions("quitconfirm"); });

        // Return button
        tempButton = instance.transform.Find($"{panelPath}/ButtonReturn").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { MainMenuButtonsActions("return"); });


        // - - - - LEVEL SELECTION PANEL - - - -

        panelPath = "Canvas_Mainmenu/Panel_LevelSelection";

        // Level buttons
        for(int i = 1; i < 11; i++)
        {
            string j;
            if (i < 10)
                j = $"0{i}";
            else
                j = i.ToString();
            tempButton = instance.transform.Find($"{panelPath}/ButtonLevel{i}").GetComponent<Button>();
            tempButton.onClick.AddListener(delegate { MainMenuButtonsActions($"level{j}"); });
        }

        // Return button
        tempButton = instance.transform.Find($"{panelPath}/ButtonReturn").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { MainMenuButtonsActions("return"); });
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

    private void SetLevelSelectionPanel(GameObject panel)
    {
        // Get level buttons and score images game object
        for (int i = 1; i < LevelManager.maxLevels + 1; i++)
        {
            levelButtons[i - 1] = panel.transform.Find($"ButtonLevel{i}").gameObject.GetComponent<Button>();
            levelScore[i - 1] = panel.transform.Find($"ScoreLevel{i}").gameObject;
        }

        // Set level buttons and score images game object
        for (int i = 2; i <= LevelManager.maxLevels; i++)
        {
            if (i > LevelManager.levelsUnlocked)
            {
                levelButtons[i - 1].interactable = false;
                levelScore[i - 1].SetActive(false);
            }
        }

        // Put the respective score image for each level score
        for (int i = 0; i < LevelManager.levelsUnlocked; i++)
            levelScore[i].GetComponent<Image>().sprite = starImages[LevelManager.levelsScore[i]];
    }

}