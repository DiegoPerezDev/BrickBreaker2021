using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class GameManager : MonoBehaviour
{
    /*
    * - - - NOTES - - -
    *   This class manages: 
    *       * the scene transitions with their respective settings and load screen management.
    *       * General game settings like developer options and running settings.
    */

    private static GameManager instance;
    public static bool loadingScene, settingScene;
    [SerializeField] private bool printTransitionStates;


    void Awake()
    {
        // Set only one instance.
        if (instance != null)
            Destroy(transform.parent.gameObject);
        else
        {
            // Set singleton
            instance = this;
            DontDestroyOnLoad(transform.parent.gameObject);

            // GENERAL SETTINGS
            Application.runInBackground = false;
            Debug.developerConsoleVisible = true;

            // Load saved data
            SavingData.LoadLevelData();
            SavingData.LoadSettingData();
            //SavingData.ResetSavingData();
        }
    }

    void Start()
    {
        SetSavedSetting();
        instance.StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex, true));
    }
    
    void OnDestroy()
    {
        SavingData.SaveSettingsData();
    }


    /// <summary>
    /// <para>So we can use the 'LoadScene()' corroutine like a method.</para>
    /// <para>'LoadScene()': Go to the loading scene while loading the desired scene while waiting for all of the managers to set up that scene.</para>
    /// </summary>
    /// <param name="scene">index of the scene to be loaded in the loading scene</param>
    public static void GoToScene(int scene)
    {
        if (!loadingScene)
            instance.StartCoroutine(LoadScene(scene, false));
        else
            print("Could not load scene because we are already loading a scene");
    }

    /// <summary>
    /// Go to the loading scene while loading the desired scene while waiting for all of the managers to set up that scene.</para>
    /// </summary>
    private static IEnumerator LoadScene(int scene, bool startingInLevel)
    {
        // Set some things first
        loadingScene = true;
        settingScene = true;
        UI_Manager.inMenu = true;
        AudioManager.StopAllAudio();

        // If start the game in a level in developent we dont have to go to the loading scene.
        if (startingInLevel)
            goto AfterSceneChange;

        // Go to the loading scene
        if (instance.printTransitionStates)
            print("Loading... Entering loading scene.");
        AsyncOperation loadingOperation = SceneManager.LoadSceneAsync(1);
        AudioManager.PlayLevelSong(1);
        while (!loadingOperation.isDone)
            yield return null;
        AudioManager.StopLevelSong();

        // Go to the desired scene
        if (instance.printTransitionStates)
            print("Loading... Entering desired scene.");
        loadingOperation = SceneManager.LoadSceneAsync(scene);
        while (!loadingOperation.isDone)
            yield return null;

        //Get the sceneType and then set the scene
        AfterSceneChange:
        if (instance.printTransitionStates)
            print("Loading... Setting scene.");
        while (settingScene)
            yield return null;

        // Start the scene.
        GameObject loadScreen = GameObject.Find("UI/Canvas_Loading");
        Destroy(loadScreen);
        loadingScene = false;
        if (instance.printTransitionStates)
            print("Scene loaded! Starting scene...");

        // To finish this process we need to enable the bool 'settingScene' in the respective scene type manager, E.G. in 'LevelManager' .
    }

    /// <summary>
    /// Final set of the scene loading. This method should be called from the specific scene manager, like 'LevelManager' for example.
    /// </summary>
    public static void StartScene(int scene)
    {
        AudioManager.PlayLevelSong(scene);
        InputsManager.DisableTriggers();
        if (instance.printTransitionStates)
            print("Scene started!");
    }

    /// <summary>
    /// Set settings things that needs saved data, like the audio volume.
    /// </summary>
    private static void SetSavedSetting()
    {
        // Set music volume
        float fixedVolume = Gameplay_UI.SliderValToMixerVal(Gameplay_UI.musicSliderVal);
        AudioManager.SetMusicVolume(fixedVolume);

        // Set sfx volume
        fixedVolume = Gameplay_UI.SliderValToMixerVal(Gameplay_UI.sfxSliderVal);
        AudioManager.SetSFX_Volume(fixedVolume);
    }

}