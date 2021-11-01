using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyTools;

public class GameManager : MonoBehaviour
{
    /*
    * - - - NOTES - - -
        This class manages:
    - The general game save and load (using other saving codes).
    - The scene transitions.
    - The scene set for the transitions.
    */

    // General data
    private static GameManager instance;

    // Scene management
    public enum SceneType { Mainmenu, load, gameplay }
    public static SceneType currentSceneType;
    public static bool loadingScene, settingScene;
    [SerializeField] private bool printTransitionStates;


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
        // Load data
        StartCoroutine(LoadGameData());

        // For debugging purposes
        Debug.developerConsoleVisible = true;

        // Open scene
        GoToScene(SceneManager.GetActiveScene().buildIndex);
    }


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
        UI_Manager.inMenu = true;
        AudioManager.StopAllAudio();

        // Go to the loading scene
        if(instance.printTransitionStates)
            print("Loading... Entering loading scene");
        currentSceneType = SceneType.load;
        AsyncOperation loadingOperation1 = SceneManager.LoadSceneAsync(1);
        AudioManager.PlayLevelSong(1);
        while (!loadingOperation1.isDone)
        {
            yield return null;
        }
        AudioManager.StopLevelSong();

        // Go to the desired scene
        if (instance.printTransitionStates)
            print("Loading... Entering desired scene");
        AsyncOperation loadingOperation2 = SceneManager.LoadSceneAsync(scene);
        while (!loadingOperation2.isDone)
        {
            yield return null;
        }

        //Get the sceneType and then set the scene
        GetSceneType(scene);
        settingScene = true;
        if (instance.printTransitionStates)
            print("Loading... Setting scene");

        switch (currentSceneType)
        {
            case SceneType.Mainmenu:
                if (instance.printTransitionStates)
                    print("entering main menu");
                instance.StartCoroutine(MainmenuSetDelay());
                break;

            case SceneType.load:
                print("Going directly to the loading scene should only happen testing the loading scene in development.");
                yield break;

            case SceneType.gameplay:
                if (instance.printTransitionStates)
                    print("entering gameplay");
                instance.StartCoroutine(GameplaySetDelay());
                break;

            default:
                print("Didn't recognice the scene to load. Check the build index you'r trying to access");
                yield break;
        }
        while (settingScene)
        {
            yield return null;
        }

        // Close the loading frame after setting all the scene.
        try
        {
            Destroy(GameObject.Find("UI/UI_Loading"));
        }
        catch
        {
            print("could not find the loading screen to destroy in scene number: " + scene);
            Debug.Break();
        }

        // Start the scene.
        //      Dont start the gameplay scene after scene set because we still want to wait for the level introduction frame delay.
        //      Call the StartScene method in the LevelManager for the gameplay scene type.
        if (currentSceneType == SceneType.Mainmenu)
            StartScene(scene);
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

    public static void StartScene(int scene)
    {
        AudioManager.PlayLevelSong(scene);
        
        InputsManager.DisableTriggers();
        loadingScene = false;
        if (instance.printTransitionStates)
            print("Scene loaded!");
    }

    #endregion

    #region Scene start settings

    /// <summary>
    /// Set the main menu scene. Only for using it in the 'LoadScene' corroutine.
    /// </summary>
    private static IEnumerator MainmenuSetDelay()
    {
        UI_Manager.inMenu = true;

        // Set InputManager
        InputsManager.gameplayInputsDisabled = true;

        // Set UI
        if (instance.printTransitionStates)
            print("Loading... Setting UI");
        while (!MainMenu.ready)
        {
            yield return null;
        }
        MainMenu.ready = false;

        // Set AudioManager
        if (instance.printTransitionStates)
            print("Loading... Setting audio");
        // (Nothing for now)

        // Finish
        settingScene = false;
    }

    /// <summary>
    /// Set a gameplay scene. Only for using it in the 'LoadScene' corroutine.
    /// </summary>
    /// <param name="scene">Which level to go</param>
    private static IEnumerator GameplaySetDelay()
    {
        // Set InputsManager
        if (instance.printTransitionStates)
            print("Loading... Setting Inputs");
        InputsManager.SetGameplay();
        while (!InputsManager.InputsReady)
        {
            yield return null;
        }
        InputsManager.InputsReady = false;

        // Set UI_Manager
        if (instance.printTransitionStates)
            print("Loading... Setting UI");
        while (!GameplayMenu.ready)
        {
            yield return null;
        }
        GameplayMenu.ready = false;

        // Set LevelManager
        if (instance.printTransitionStates)
            print("Loading... Level");
        LevelManager.SetGameplay();
        while (!LevelManager.levelReady)
        {
            yield return null;
        }
        LevelManager.levelReady = false;

        // Finish
        UI_Manager.inMenu = false;
        settingScene = false;
        //LevelManager.Pause(false);
    }

    #endregion

    #region saving & loading general game data

    public static void SaveGameData(bool newRecord, int currentLevel)
    {
        // Another level done if i havent done this specific level yet
        if (LevelManager.levelsDone[currentLevel - 1] == false)
        {
            LevelManager.levelsDone[currentLevel - 1] = true;
            LevelManager.levelsUnlocked++;
        }

        // if new record, then save new record and show it in the UI
        if (newRecord)
            LevelManager.levelsScore[currentLevel - 1] = LevelManager.lives;

        SaveSystem.SaveLevelData();
    }

    public static IEnumerator LoadGameData()
    {
        float delay = 0;
        while (delay < 0.2f)
        {
            yield return null;
            delay += Time.deltaTime;
        }

        SaveData saveData = SaveSystem.LoadLevelData();
        SaveData.LoadData(saveData);

        if (LevelManager.levelsUnlocked > 1)
        {
            for (int i = 0; i < LevelManager.levelsUnlocked; i++)
                LevelManager.levelsDone[i] = true;
        }
    }

    #endregion

}