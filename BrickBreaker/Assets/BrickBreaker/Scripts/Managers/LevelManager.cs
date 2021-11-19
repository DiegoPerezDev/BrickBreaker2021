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
    - This class manage the gameplay scenes (levels) general behaviour:
        * Pausing.
        * Level data: levels unlocked, levels score, etc.
        * Level entering and leaving behaviour
        * Win and lose behaviour
    */

    // - - - GENERAL LEVEL MANAGEMENT - - -

    // General
    [SerializeField] private bool printTransitionStates;

    // Pause
    public static bool pauseTrigger, playing, paused;

    // Win and lose
    private static AudioClip loseAudio, winAudio;
    private static CameraMovement camCode;
    private static GameObject newRecordText;
    private static Image scoreImg;
    private static readonly Sprite[] scoreImgs = new Sprite[4];

    // Start setting
    private static LevelManager instance;
    private static IEnumerator setCorroutine;

    // Level data
    public static int levelsUnlocked;
    public static readonly int maxLevels = 10;
    public static int[] levelsScore = new int[maxLevels];


    void Awake()
    {
        // Components for start settings
        instance = this;
        GameObject LevelEnteringTitleGO = SearchTools.TryFind("UI/Canvas_Menu/Panel_LevelEntering/TitleTMP");
        GameObject InGame_LevelIndicatorGO = SearchTools.TryFind("UI/Canvas_HUD/Panel_RightBlock/Level");
        int actualScene = SceneManager.GetActiveScene().buildIndex - 1;
        if (LevelEnteringTitleGO)
        {
            TextMeshProUGUI startLevelIndicator = SearchTools.TryGetComponent<TextMeshProUGUI>(LevelEnteringTitleGO);
            TextMeshProUGUI InGame_LevelIndicator = SearchTools.TryGetComponent<TextMeshProUGUI>(InGame_LevelIndicatorGO);
            if (actualScene < maxLevels)
                startLevelIndicator.text = InGame_LevelIndicator.text = $"Level 0{actualScene}";
            else
                startLevelIndicator.text = InGame_LevelIndicator.text = $"Level {actualScene}";
        }

        // Components for win and lose management
        loseAudio = SearchTools.TryLoadResource("Audio/Level general/(lg1) lose") as AudioClip;
        winAudio = SearchTools.TryLoadResource("Audio/Level general/(lg3) win") as AudioClip;
        camCode = Camera.main.GetComponent<CameraMovement>();
        newRecordText = SearchTools.TryFind("UI/Canvas_Menu/Panel_Win/NewRecord");
        var temp = SearchTools.TryFind("UI/Canvas_Menu/Panel_Win/Score");
        scoreImg = SearchTools.TryGetComponent<Image>(temp);
        for (int i = 0; i < 4; i++)
            scoreImgs[i] = Resources.Load<Sprite>($"Art2D/Stars/{i}stars");
    }

    void Start()
    {
        SetLevelObjects();

        // Set all the things that needs time for being set
        setCorroutine = SetGameplayCorr();
        instance.StartCoroutine(setCorroutine);
    }

    void Update()
    {
        // Pause management
        if (pauseTrigger)
        {
            pauseTrigger = false;

            // If i am at the first pause menu layer, unpause the game, else pause it if not in the pause menu.
            if (UI_Manager.inMenu)
            {
                if (GameplayMenu.openedMenus[UI_Manager.currentMenuLayer] == GameplayMenu.Panels.pause)
                {
                    AudioManager.PlayAudio(AudioManager.GameAudioSource, UI_Manager.uiClips[(int)UI_Manager.UiAudioNames.unPause], false, 1f);
                    PauseMenu(false, true);
                }
            }
            else
            {
                PauseMenu(true, true);
                InputsManager.DisableTriggers();
            }
        }
    }

    void OnDestroy()
    {
        // reset some values
        playing = false;

        // stop coroutines
        StopAllCoroutines();
        if(instance != null)
            instance.StopAllCoroutines();
        if (setCorroutine != null)
            StopCoroutine(setCorroutine);
    }

    /// <summary>
    /// Behaviour of the game when minimizing. In this case we open the pause menu.
    /// </summary>
    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus || GameManager.loadingScene)
            return;
        if (!paused)
            PauseMenu(true, false);
    }


    #region start settings functions

    /// <summary>
    /// Corroutine for waiting till all the codes that has things to set in the level finish setting those things.
    /// </summary>
    private static IEnumerator SetGameplayCorr()
    {
        // Set InputsManager
        if (instance.printTransitionStates)
            print("Loading... Setting Inputs");
        InputsManager.SetGameplay();
        while (!InputsManager.InputsReady)
            yield return null;
        InputsManager.InputsReady = false;

        // Set UI_Manager
        if (instance.printTransitionStates)
            print("Loading... Setting UI");
        while (!GameplayMenu.ready)
            yield return null;
        GameplayMenu.ready = false;

        // Set saving data
        int currentLevel = SceneManager.GetActiveScene().buildIndex - 1; // - 1 because the levels starts on scene #2
        if (currentLevel > levelsUnlocked)
        {
            levelsUnlocked++;
            if (levelsUnlocked > maxLevels)
                levelsUnlocked = maxLevels;
        }
        SavingData.SaveLevelData();

        // Set level objects:
        if (instance.printTransitionStates)
            print("Loading... Setting screen bounds");
        while (!ScreenBounds.StartSet)
            yield return null;
        if (instance.printTransitionStates)
            print("Loading... Setting paddle");
        while (!Paddle.StartSet)
            yield return null;
        if (instance.printTransitionStates)
            print("Loading... Setting ball");
        while (!Ball.StartSet)
            yield return null;

        // Tell the GameManager this code is ready so it disables the loading screen.
        GameManager.settingScene = false;

        // Enable level entering panel for a short time
        if ( (SceneManager.GetActiveScene().buildIndex - 1) <= maxLevels)
            yield return new WaitForSecondsRealtime(1.6f);
        GameObject temp = SearchTools.TryFind("UI/Canvas_Menu/Panel_LevelEntering");
        temp.SetActive(false);

        // Starts the level
        GameManager.StartScene(SceneManager.GetActiveScene().buildIndex);
        UI_Manager.inMenu = false;
        Pause(false);
        playing = true;
    }

    /// <summary>
    /// Instantiate some objects, set some others and manage some components needed.
    /// </summary>
    private static void SetLevelObjects()
    {
        // Add some codes to this gameObject
        if (instance.gameObject.GetComponent<PowersSystem>() == null)
            instance.gameObject.AddComponent<PowersSystem>();
        if (instance.gameObject.GetComponent<BricksSystem>() == null)
            instance.gameObject.AddComponent<BricksSystem>();

        // Enable postProcessing
        PostprocessingManager.EnablePostProcessing(true);
    }

    #endregion

    #region Pause management

    /// <summary>
    /// <para> Open or close the pause menu on a gameplay scene.</para>
    /// <para> For pausing without UI changing use the 'Pause()' method. </para>
    /// </summary>
    public static void PauseMenu(bool pausing, bool withSound)
    {
        pauseTrigger = false;

        if (pausing)
        {
            paused = true;
            playing = false;
            if(withSound)
                AudioManager.PlayAudio(AudioManager.GameAudioSource, UI_Manager.uiClips[(int)UI_Manager.UiAudioNames.pause], false, 1f);
            GameplayMenu.OpenMenuLayer(GameplayMenu.Panels.pause);
        }
        else
        {
            paused = false;
            playing = true;
            GameplayMenu.ClosePanel();
            InputsManager.DisableTriggers();
        }
        InputsManager.gameplayInputsDisabled = pausing;
        Pause(pausing);
    }

    /// <summary>
    /// <para> Pause or unpause the game without changing the menus. </para>
    /// <para> For pausing without UI changing use the 'Pause()' method. </para>
    /// </summary>
    private static void Pause(bool enable)
    {
        if (enable)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    #endregion

    #region Win and Lose management

    /// <summary>
    /// What happens when losing the game. Call it in another code that triggers the losing.
    /// </summary>
    public static void LoseGame()
    {
        // Gemeral management
        Pause(true);
        playing = false;

        // Audio and UI settings
        AudioManager.StopLevelSong();
        AudioManager.PlayAudio(AudioManager.GameAudioSource, loseAudio, false, 1f);
        GameplayMenu.OpenMenuLayer(GameplayMenu.Panels.lose);
    }

    /// <summary>
    /// What happens when winning the game. Call it in another code that triggers the winning.
    /// </summary>
    public static void WinGame()
    {
        // Check new record
        int currentLevel = SceneManager.GetActiveScene().buildIndex - 1; // - 1 because the levels starts on scene #2
        bool newRecord = false;
        if (levelsScore[currentLevel - 1] < PlayerLife.lives)
            newRecord = true;

        // General management
        playing = false;
        Pause(true);
        if (newRecord)
            levelsScore[currentLevel - 1] = PlayerLife.lives;
        SavingData.SaveLevelData();

        // Audio and UI settings
        AudioManager.StopLevelSong();
        AudioManager.PlayAudio(AudioManager.GameAudioSource, winAudio, false, 1f);
        scoreImg.sprite = scoreImgs[PlayerLife.lives];
        GameplayMenu.OpenMenuLayer(GameplayMenu.Panels.win);
        if (!newRecord)
            newRecordText.SetActive(false);
    }

    /// <summary>
    /// What happens when losing a live. Call it in another code that triggers it.
    /// </summary>
    public static void LoseLive()
    {
        // Lose a live
        PlayerLife.lives--;
        HUD_Life.RewriteLife();

        var ball = SearchTools.TryFind(Ball.ballPath);
        Ball ballCode = ball.GetComponent<Ball>();

        // Check if all lives are lost for triggering the game lost.
        if (PlayerLife.lives < 1)
        {
            Destroy(ballCode.gameObject);
            LoseGame();
            return;
        }
        // Regular behaviour when losing a live
        else
        {
            AudioManager.PlayAudio(AudioManager.GameAudioSource, PlayerLife.loseLifeAudio, false, 0.4f);
            camCode.StartCoroutine(camCode.Shake(0.2f, 0.07f));
            ResetLevel(ballCode);
        }
    }

    private static void ResetLevel(Ball ballCode)
    {
        //Reset HUD
        GameplayMenu.launchButton.SetActive(true);
        GameObject powersUI_GO = GameObject.Find(HUD_PowerTimer.powerTimersContainerPath);
        if (powersUI_GO != null)
        {
            foreach (HUD_PowerTimer timer in powersUI_GO.GetComponentsInChildren<HUD_PowerTimer>())
            {
                timer.ResetCounter();
                timer.gameObject.SetActive(false);
            }
        }

        // Reset paddle and ball
        ballCode.RestartBall();
        ballCode.StopPower();
        GameObject paddle = GameObject.Find(Paddle.paddlePath);
        if (paddle != null)
            paddle.GetComponent<Paddle>().StopPower();

        // Reset powers
        PowersSystem.ResetPowers();
    }

    #endregion

}