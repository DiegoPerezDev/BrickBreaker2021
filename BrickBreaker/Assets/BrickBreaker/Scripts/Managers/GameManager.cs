using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyTools;

public class GameManager : MonoBehaviour
{
    /*
    * - - - NOTES - - -
    - This class manage the scene transitions, the pausing and other things in game that happen in more than one scene type (for exaple gameplay but also main menu).
    */


    // General stuff
    private static GameManager instance;
    public static bool inGameplay, gamePaused;

    // Inputs
    public static bool returnTrigger, pauseTrigger;

    // Menu management
    public static bool inMenu;

    //Audio
    private static AudioClip loseAudio, winAudio;

    // Scene management
    public enum SceneType { Mainmenu, load, gameplay }
    public static SceneType currentSceneType;
    public static bool loadingScene, settingScene;
    private static readonly bool printTransitionStates = false;


    void Awake()
    {
        if (instance != null)
            Destroy(transform.parent.gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(transform.parent.gameObject);
        }
    }

    void Start()
    {
        // For debugging purposes
        GoToScene(SceneManager.GetActiveScene().buildIndex);
        Debug.developerConsoleVisible = true;

        // Get audio clips
        loseAudio = SearchTools.TryLoadResource("Audio/Level general/(lg1) lose") as AudioClip;
        winAudio = SearchTools.TryLoadResource("Audio/Level general/(lg3) win") as AudioClip;
    }

    void Update()
    {
        if(!loadingScene)
            MenuManagement();

        // For debugging purposes
        //print(gamePaused);
    }


    // - - - - MAIN MANAGEMENT - - - -

    #region Menu management

    /// <summary>
    /// This is a state machine that manage the menu and the pause behaviour for it.
    /// </summary>
    private void MenuManagement()
    {
        // Return to previous menu
        if (returnTrigger)
        {
            returnTrigger = false;

            // Unpause and close menu on gameplay if we are at the first pause menu layer, else just return to the previous menu.
            if (inMenu)
            {
                if ( (currentSceneType == SceneType.gameplay) && UI_Manager.inPauseMenuFirstLayer)
                {
                    pauseTrigger = false;
                    PauseMenu(false);
                }
                // Return to the previous menu if we are not in these panels:
                // win, lose, pause, main menu or none
                else if(UI_Manager.currentMenuLayer > 1)
                {
                    UI_Manager.ReturnToPreviousPanel();
                }
            }
        }

        // Pause management
        if (currentSceneType == SceneType.gameplay)
        {
            if (pauseTrigger)
            {
                pauseTrigger = false;

                // If i am at the first pause menu layer, unpause the game, else pause it.
                if (inMenu)
                {
                    if (UI_Manager.inPauseMenuFirstLayer)
                        PauseMenu(false);
                }
                else
                {
                    PauseMenu(true);
                    InputsManager.DisableTriggers();
                }
            }
        }
    }

    #endregion

    #region Transition functions

    /// <summary>
    /// <para>So we can use the 'LoadScene()' corroutine like a method.</para>
    /// <para>'LoadScene()': Go to the loading scene while loading the desired scene while waiting for all of the managers to set up that scene.</para>
    /// </summary>
    /// <param name="scene">index of the scene to be loaded in the loading scene</param>
    public static void GoToScene(int scene)
    {
        if (!loadingScene)
            instance.StartCoroutine(LoadScene(scene));
        else
            print("Could not load scene because we are already loading a scene");
    }

    /// <summary>
    /// Go to the loading scene while loading the desired scene while waiting for all of the managers to set up that scene.</para>
    /// </summary>
    private static IEnumerator LoadScene(int scene)
    {
        // Set some things first
        loadingScene = true;
        ResetGameManager();

        // Go to the loading scene
        if(printTransitionStates)
            print("Loading... Entering loading scene");
        currentSceneType = SceneType.load;
        AsyncOperation loadingOperation1 = SceneManager.LoadSceneAsync(1);
        while (!loadingOperation1.isDone)
        {
            yield return null;
        }

        // Go to the desired scene
        if (printTransitionStates)
            print("Loading... Entering desired scene");
        AsyncOperation loadingOperation2 = SceneManager.LoadSceneAsync(scene);
        while (!loadingOperation2.isDone)
        {
            yield return null;
        }

        //Get the sceneType and then set the scene depending on the type of scene
        GetSceneType(scene);
        settingScene = true;

        switch(currentSceneType)
        {
            case SceneType.Mainmenu:
                if (printTransitionStates)
                    print("entering main menu");
                instance.StartCoroutine(MainmenuSetDelay());
                break;

            case SceneType.load:
                if (printTransitionStates)
                    print("entering loading screen");
                goto LoadingEnd;

            case SceneType.gameplay:
                if (printTransitionStates)
                    print("entering gameplay");
                instance.StartCoroutine(GameplaySetDelay());
                break;

            default:
                print("Didn't recognice the scene to load. Check the build index you'r trying to access");
                yield break;
        }
        if (printTransitionStates)
            print("Loading... Setting scene");
        while (settingScene)
        {
            yield return null;
        }

        // Last settings for main menu and gameplay scenes
        try
        {
            Destroy(GameObject.Find("UI/UI_Loading"));
        }
        catch
        {
            print("could not find the loading screen to destroy in scene number: " + scene);
            Debug.Break();
        }

        LoadingEnd:
        AudioManager.PlayLevelSong(scene);
        if (printTransitionStates)
            print("Scene loaded!");
        loadingScene = false;
        Pause(false);
    }
    
    /// <summary>
    /// <para>Set our scene type depending on the scene index. </para>
    /// <para>This function is just so we can use the 'LoadScene()' easier.</para>
    /// </summary>
    /// <param name="sceneIndex">Unity's scene index of the scene, its place in the 'File/BuildSettings'.</param>
    private static void GetSceneType(int sceneIndex)
    {
        currentSceneType = sceneIndex switch
        {
            0 => SceneType.Mainmenu,
            1 => SceneType.load,
            _ => SceneType.gameplay,
        };
    }

    /// <summary>
    /// <para>Restart some of this code values when loading a new scene.</para>
    /// <para>This function is just so we can use the 'LoadScene()' easier.</para>
    /// </summary>
    private static void ResetGameManager()
    {
        Pause(true);
        returnTrigger = pauseTrigger = false;
        inMenu = true;
        AudioManager.StopAllAudio();
    }

    #endregion


    // - - - - MAIN MENU - - - -

    #region Start functions

    /// <summary>
    /// Set the main menu scene. Only for using it in the 'LoadScene' corroutine.
    /// </summary>
    private static IEnumerator MainmenuSetDelay()
    {
        // Set GameManager
        inMenu = true;

        // Set InputManager
        InputsManager.gameplayInputsDisabled = true;

        // Set UIManager
        if (printTransitionStates)
            print("Loading... Setting UI");
        UI_Manager.SetMainmenu();
        while (!UI_Manager.UI_Ready)
        {
            yield return null;
        }
        UI_Manager.UI_Ready = false;

        // Set AudioManager
        if (printTransitionStates)
            print("Loading... Setting audio");
        // (Nothing for now)

        // Finish
        settingScene = false;
    }

    #endregion


    // - - - - - GAMEPLAY - - - - -

    #region Start functions

    /// <summary>
    /// Set a gameplay scene. Only for using it in the 'LoadScene' corroutine.
    /// </summary>
    /// <param name="scene">Which level to go</param>
    private static IEnumerator GameplaySetDelay()
    {
        // Set InputsManager
        if (printTransitionStates)
            print("Loading... Setting Inputs");
        InputsManager.SetGameplay();
        while (!InputsManager.InputsReady)
        {
            yield return null;
        }
        InputsManager.InputsReady = false;

        // Set UI_Manager
        if (printTransitionStates)
            print("Loading... Setting UI");
        UI_Manager.SetGameplay();
        while (!UI_Manager.UI_Ready)
        {
            yield return null;
        }
        UI_Manager.UI_Ready = false;

        // Set LevelManager
        if (printTransitionStates)
            print("Loading... Level");
        LevelManager.SetGameplay();
        while (!LevelManager.levelReady)
        {
            yield return null;
        }
        LevelManager.levelReady = false;

        // Finish
        inMenu = false;
        inGameplay = true;
        settingScene = false;
    }

    #endregion

    #region Pause management

    /// <summary>
    /// Open or close the pause menu on a gameplay scene. Also manage the real pausing in each case. For just pausing without UI changing use the 'Pause()' method.
    /// </summary>
    private static void PauseMenu(bool pausing)
    {
        if (pausing)
        {
            UI_Manager.SwitchPanel(UI_Manager.Panels.pause, true);
            AudioManager.PlayAudio(AudioManager.GameAudioSource, UI_Manager.uiClips[(int)UI_Manager.UiAudioNames.pause], false, 1f);
        }
        else
        {
            UI_Manager.ReturnToPreviousPanel();
            AudioManager.PlayAudio(AudioManager.GameAudioSource, UI_Manager.uiClips[(int)UI_Manager.UiAudioNames.unPause], false, 1f);
            InputsManager.DisableTriggers();
        }
        InputsManager.gameplayInputsDisabled = pausing;
        inMenu = pausing;
        Pause(pausing);
    }

    /// <summary>
    /// Pause or unpause the game without changing the menus.
    /// </summary>
    /// <param name="enable"></param>
    private static void Pause(bool enable)
    {
        gamePaused = enable;
        if (enable)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    #endregion

    #region Win and Lose 

    /// <summary>
    /// What happens when losing the game. Call it in another code that triggers the losing.
    /// </summary>
    public static void LoseGame()
    {
        print("game lost");
        Pause(true);
        inMenu = true;
        inGameplay = false;
        AudioManager.StopLevelSong();
        AudioManager.PlayAudio(AudioManager.GameAudioSource, loseAudio, false, 1f);
        UI_Manager.SwitchPanel(UI_Manager.Panels.lose, true);
    }

    /// <summary>
    /// What happens when winning the game. Call it in another code that triggers the winning.
    /// </summary>
    public static void WinGame()
    {
        print("game won");
        Pause(true);
        inMenu = true;
        inGameplay = false;
        AudioManager.StopLevelSong();
        AudioManager.PlayAudio(AudioManager.GameAudioSource, winAudio, false, 1f);
        UI_Manager.SwitchPanel(UI_Manager.Panels.win, true);
    }

    #endregion

}