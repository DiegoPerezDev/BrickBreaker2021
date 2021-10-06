using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MyTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    /*
    * - - - NOTES - - -
    - This class is for everything that happens only in the gameplay scenes (levels).
    - Gameplay codes should only use this manager and the audio manager.
    */

    // - - - GENERAL LEVEL MANAGEMENT - - -

    // General management: Pause
    public static bool pauseTrigger;

    // General management: Audio
    private static AudioClip loseAudio, winAudio;

    // General management: Setting class
    private static LevelManager instance;
    private static IEnumerator setCorroutine;
    public static bool levelReady;


    // - - - SPECIFIC LEVEL DATA - - -

    // Level data
    public static int numberOfActiveBricks;
    public static int lives;
    public static readonly int liveCap = 3;
    public static int levelsUnlocked = 1;
    public static readonly int maxLevels = 10;
    public static bool[] levelsDone = new bool[maxLevels];

    // Score
    public static int[] levelsScore = new int[maxLevels];
    private static GameObject newRecordText;
    private static Image scoreImg;
    private static readonly Sprite[] scoreImgs = new Sprite[4];

    // Level objects
    private static GameObject screenBoundsPref;
    private static Ball ballCode;

    // Audio
    public static AudioSource[] bricksAudioSources = new AudioSource[5];
    public static AudioSource powersAudioSource;
    public static AudioClip loseLifeAudio, getPowerAudio;
    public static AudioClip hitAudio, metalHitAudio, destructionAudio;

    

    void Awake()
    {
        instance = this;

        // Level entering text
        GameObject temp1 = SearchTools.TryFind("UI/UI_Gameplay/Canvas_Menu/Panel_LevelEntering/TitleTMP");
        TextMeshProUGUI temp2;
        int actualScene = SceneManager.GetActiveScene().buildIndex - 1;
        if (temp1)
        {
            temp2 = SearchTools.TryGetComponent<TextMeshProUGUI>(temp1);
            if(actualScene < 10)
                temp2.text = $"Level 0{actualScene}";
            else
                temp2.text = $"Level {actualScene}";
        }

        // Get audio
        powersAudioSource = GameObject.Find("LevelDev/Bricks_").AddComponent<AudioSource>();
        for (int i = 0; i < bricksAudioSources.Length; i++)
            bricksAudioSources[i] = GameObject.Find("LevelDev/Bricks_").AddComponent<AudioSource>();
        loseAudio = SearchTools.TryLoadResource("Audio/Level general/(lg1) lose") as AudioClip;
        winAudio = SearchTools.TryLoadResource("Audio/Level general/(lg3) win") as AudioClip;
        getPowerAudio = SearchTools.TryLoadResource("Audio/Level objects/(lo1) get power") as AudioClip;
        hitAudio = SearchTools.TryLoadResource("Audio/Level objects/(gs1) brick getting hit") as AudioClip;
        metalHitAudio = SearchTools.TryLoadResource("Audio/Level objects/(lo1) metalBrick") as AudioClip;
        destructionAudio = SearchTools.TryLoadResource("Audio/Level objects/(gs1) brick getting crushed") as AudioClip;
        loseLifeAudio = Resources.Load<AudioClip>("Audio/Level general/(gs1) losing life");

        // Prefabs for instancing
        screenBoundsPref = SearchTools.TryLoadResource("Prefabs/LevelDev/ScreenBounds") as GameObject;

        // Score
        newRecordText = SearchTools.TryFind("UI/UI_Gameplay/Canvas_Menu/Panel_Win/NewRecord");
        var temp = SearchTools.TryFind("UI/UI_Gameplay/Canvas_Menu/Panel_Win/Score");
        scoreImg = SearchTools.TryGetComponent<Image>(temp);
        for (int i = 0; i < 4; i++)
            scoreImgs[i] = Resources.Load<Sprite>($"Art2D/Stars/{i}stars");
    }

    void Start()
    {
        InstantiateLevelObjects();
        Pause(false);
    }

    void Update()
    {
        // Pause management
        if (pauseTrigger)
        {
            pauseTrigger = false;

            // If i am at the first pause menu layer, unpause the game, else pause it.
            if (UI_Manager.inMenu)
            {
                if (GameplayMenu.openedMenus[UI_Manager.currentMenuLayer] == GameplayMenu.Panels.pause)
                {
                    AudioManager.PlayAudio(AudioManager.GameAudioSource, UI_Manager.uiClips[(int)UI_Manager.UiAudioNames.unPause], false, 1f);
                    PauseMenu(false);
                }
            }
            else
            {
                PauseMenu(true);
                InputsManager.DisableTriggers();
            }
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }


    #region setting functions

    /// <summary>
    /// <para> For setting everything for the gameplay level when starting those levels.</para> 
    /// <para> This method should be called when making a transition to a gameplay scene from the GameManager class.</para> 
    /// </summary>
    public static void SetGameplay()
    {
        var ball = SearchTools.TryFindInGameobject(instance.gameObject, Ball.ballName);
        ballCode = ball.GetComponent<Ball>();

        // Set lifes
        lives = liveCap;
        GameplayMenu.RewriteLife();

        // Wait while the other codes of the gameplay finish setting things
        if (setCorroutine == null)
        {
            // This will prevent mutiple calls bugs
            setCorroutine = SetGameplayCorr();
            instance.StartCoroutine(setCorroutine);
        }
    }

    /// <summary>
    /// Corroutine for waiting till all the codes that has things to set in the level finish setting those things.
    /// </summary>
    private static IEnumerator SetGameplayCorr()
    {
        // Set Screen bounds
        while (!ScreenBounds.StartSet)
        {
            yield return null;
        }

        // Set bricks
        numberOfActiveBricks = 0;
        numberOfActiveBricks = GameObject.Find("LevelDev/Bricks_").transform.childCount;

        // Set paddle
        while (!Paddle.StartSet)
        {
            yield return null;
        }

        // Set ball
        while (!Ball.StartSet)
        {
            yield return null;
        }

        // Tell the GameManager this code is ready
        levelReady = true;

        // Level entering panel for a short time
        yield return new WaitForSecondsRealtime(2f);
        GameObject temp = SearchTools.TryFind("UI/UI_Gameplay/Canvas_Menu/Panel_LevelEntering");
        if (temp)
            temp.SetActive(false);

        // Start the game
        GameManager.StartScene(SceneManager.GetActiveScene().buildIndex);
        setCorroutine = null;
    }

    private static void InstantiateLevelObjects()
    {
        GameObject screenBounds = Instantiate(screenBoundsPref, screenBoundsPref.transform.position, Quaternion.identity);
        screenBounds.transform.parent = instance.gameObject.transform;
        screenBounds.GetComponent<ScreenBounds>().Begin();
    }

    public static void CheckNumberOfBricks()
    {

        if (numberOfActiveBricks == 0)
        {
            Destroy(SearchTools.TryFind(Ball.ballPath));
            WinGame();
        }
    }

    #endregion

    #region Pause management

    /// <summary>
    /// Open or close the pause menu on a gameplay scene. Also manage the real pausing in each case. For just pausing without UI changing use the 'Pause()' method.
    /// </summary>
    public static void PauseMenu(bool pausing)
    {
        pauseTrigger = false;

        if (pausing)
        {
            UI_Manager.OpenMenuLayer<GameplayMenu.Panels>(GameplayMenu.Panels.pause);
            AudioManager.PlayAudio(AudioManager.GameAudioSource, UI_Manager.uiClips[(int)UI_Manager.UiAudioNames.pause], false, 1f);
        }
        else
        {
            UI_Manager.ClosePanel();

            InputsManager.DisableTriggers();
        }
        InputsManager.gameplayInputsDisabled = pausing;
        Pause(pausing);
    }

    /// <summary>
    /// Pause or unpause the game without changing the menus.
    /// </summary>
    /// <param name="enable"></param>
    private static void Pause(bool enable)
    {
        if (enable)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    #endregion

    #region Win and Lose 

    public static void LoseLive()
    {
        lives--;
        GameplayMenu.RewriteLife();
        if (lives < 1)
        {
            LoseGame();
            Destroy(ballCode.gameObject);
            return;
        }
        else
        {
            AudioManager.PlayAudio(AudioManager.GameAudioSource, loseLifeAudio, false, 0.4f);
            var camCode = Camera.main.GetComponent<CameraShake>();
            camCode.StartCoroutine(camCode.Shake(0.25f, 0.07f));
        }
        ballCode.RestartBall();
    }

    /// <summary>
    /// What happens when losing the game. Call it in another code that triggers the losing.
    /// </summary>
    public static void LoseGame()
    {
        // Gemeral management
        Pause(true);

        // Audio and UI settings
        AudioManager.StopLevelSong();
        AudioManager.PlayAudio(AudioManager.GameAudioSource, loseAudio, false, 1f);
        UI_Manager.OpenMenuLayer(GameplayMenu.Panels.lose);
    }

    /// <summary>
    /// What happens when winning the game. Call it in another code that triggers the winning.
    /// </summary>
    public static void WinGame()
    {
        // New record
        int currentLevel = SceneManager.GetActiveScene().buildIndex - 1; // - 1 because the levels starts on scene #2
        bool newRecord = false;
        if (levelsScore[currentLevel - 1] < lives)
            newRecord = true;

        // Audio and UI settings
        AudioManager.StopLevelSong();
        AudioManager.PlayAudio(AudioManager.GameAudioSource, winAudio, false, 1f);
        UI_Manager.OpenMenuLayer<GameplayMenu.Panels>(GameplayMenu.Panels.win);
        if (newRecord)
        {
            scoreImg.sprite = scoreImgs[lives];
        }
        else
            newRecordText.SetActive(false);

        // Gemeral management
        Pause(true);
        GameManager.SaveGameData(newRecord, currentLevel);
    }

    #endregion

    

}