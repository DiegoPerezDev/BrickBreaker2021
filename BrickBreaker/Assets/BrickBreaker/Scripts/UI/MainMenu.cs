using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using MyTools;


public class MainMenu : MonoBehaviour
{
    // Transition management
    public static bool ready;

    // Panel management -- WARNING: The 'panelsSceneName' must match the same order as the 'Panels' enum --
    [HideInInspector] public enum Panels { none, mainmenu, levelSelection, quitConfirmation }
    private static readonly string[] panelsSceneName = { " ", "Panel_Mainmenu", "Panel_LevelSelection", "Panel_QuitConfirmation" };
    public static readonly GameObject[] panelGO = new GameObject[Enum.GetNames(typeof(Panels)).Length];
    public static Panels[] openedMenus = new Panels[10];

    // Level selection
    private GameObject[] levelButtons = new GameObject[LevelManager.maxLevels];
    private GameObject[] levelScore = new GameObject[LevelManager.maxLevels];
    private Sprite[] starImages = new Sprite[4];


    void Awake()
    {
        // Get panels components and leave only the main canvas
        GameObject canvasMainmenu = SearchTools.TryFind("UI/UI_Mainmenu/Canvas_Mainmenu");
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

        // Enable the levels done
        for (int a = 1; a < LevelManager.maxLevels; a++)
        {
            if (LevelManager.levelsDone[a])
                levelButtons[a].SetActive(true);
        }

        // Set menus
        UI_Manager.currentMenuLayer = 1;
        for (int i = 0; i < openedMenus.Length; i++)
            openedMenus[i] = Panels.none;
        openedMenus[1] = Panels.mainmenu;

        // Tell the 'GameManager' that the UI is already set.
        ready = true;
    }

    private void OnDestroy()
    {
        ready = false;
        for (int i = 0; i < openedMenus.Length; i++)
            openedMenus[i] = Panels.none;
        openedMenus[1] = Panels.mainmenu;
    }


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
                UI_Manager.OpenMenuLayer<Panels>(Panels.levelSelection);
                break;

            case "quit":
                UI_Manager.OpenMenuLayer<Panels>(Panels.quitConfirmation);
                break;

            case "quitconfirm":
                print("quit application");
                Application.Quit();
                return;

            case "return":
                UI_Manager.ClosePanel();
                break;

            default:
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
    /// This assing the event that happens when clicking the buttons from the main menu.
    /// </summary>
    private static void AssignMainmenuButtonsListener()
    {
        // - - MAIN PANEL - -

        string panelPath = "UI/UI_Mainmenu/Canvas_Mainmenu/Panel_Mainmenu";
        Button tempButton;

        // Play button
        tempButton = GameObject.Find($"{panelPath}/ButtonPlay").GetComponent<Button>();tempButton.onClick.AddListener(delegate { MainMenuButtonsActions("play"); });

        // Quit button
        tempButton = GameObject.Find($"{panelPath}/ButtonQuit").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { MainMenuButtonsActions("quit"); });


        // - - - - QUIT CONFIRMATION PANEL - - - -

        panelPath = "UI/UI_Mainmenu/Canvas_Mainmenu/Panel_QuitConfirmation";

        // Confirm button
        tempButton = GameObject.Find($"{panelPath}/ButtonConfirm").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { MainMenuButtonsActions("quitconfirm"); });

        // Return button
        tempButton = GameObject.Find($"{panelPath}/ButtonReturn").GetComponent<Button>();
        tempButton.onClick.AddListener(delegate { MainMenuButtonsActions("return"); });


        // - - - - LEVEL SELECTION PANEL - - - -

        panelPath = "UI/UI_Mainmenu/Canvas_Mainmenu/Panel_LevelSelection";

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

    private void SetLevelSelectionPanel(GameObject panel)
    {
        // Get star images for score
        for (int i = 0; i < starImages.Length; i++)
            starImages[i] = Resources.Load<Sprite>($"Art2D/Stars/{i}stars");

        // Get level buttons and score images game object
        for (int i = 1; i < LevelManager.maxLevels + 1; i++)
        {
            levelButtons[i - 1] = SearchTools.TryFindInGameobject(panel, $"ButtonLevel{i}");
            levelScore[i - 1] = SearchTools.TryFindInGameobject(panel, $"ScoreLevel{i}");
        }

        // Set level buttons and score images game object
        for (int i = 0; i < LevelManager.maxLevels; i++)
        {
            levelButtons[i].SetActive(false);
            levelScore[i].SetActive(false);
        }
        for (int i = 0; i < LevelManager.levelsUnlocked; i++)
        {
            levelButtons[i].SetActive(true);
            levelScore[i].SetActive(true);
        }

        // Put the respective score image for each level score
        for (int i = 0; i < LevelManager.levelsUnlocked; i++)
            levelScore[i].GetComponent<Image>().sprite = starImages[LevelManager.levelsScore[i]];
    }

}