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
    - This class manage the gameplay scenes (levels).
    - Its only for the general management, things like:
        * Pausing
        * Level data: levels unlocked, levels score, etc.
        * Level entering and leaving behaviour
        * Win and lose behaviour
    */

    // - - - GENERAL LEVEL MANAGEMENT - - -

    // Pause
    public static bool pauseTrigger, playing, paused;

    // Win and lose
    private static AudioClip loseAudio, winAudio;

    // For setting this class
    private static LevelManager instance;
    private static IEnumerator setCorroutine;
    public static bool levelReady;
    private static GameObject screenBoundsPref;

    // Level data
    public static int levelsUnlocked;
    public static readonly int maxLevels = 10;
    public static int[] levelsScore = new int[maxLevels];
    private static GameObject newRecordText;
    private static Image scoreImg;
    private static readonly Sprite[] scoreImgs = new Sprite[4];


    void Awake()
    {
        instance = this;

        // Get level entering text
        GameObject temp1 = SearchTools.TryFind("UI/Canvas_Menu/Panel_LevelEntering/TitleTMP");
        int actualScene = SceneManager.GetActiveScene().buildIndex - 1;
        if (temp1)
        {
            TextMeshProUGUI temp2 = SearchTools.TryGetComponent<TextMeshProUGUI>(temp1);
            if(actualScene < 10)
                temp2.text = $"Level 0{actualScene}";
            else
                temp2.text = $"Level {actualScene}";
        }

        // Get audio components
        loseAudio = SearchTools.TryLoadResource("Audio/Level general/(lg1) lose") as AudioClip;
        winAudio = SearchTools.TryLoadResource("Audio/Level general/(lg3) win") as AudioClip;

        // Get prefabs for instancing
        screenBoundsPref = SearchTools.TryLoadResource("Prefabs/LevelDev/ScreenBounds") as GameObject;

        // Get score components
        newRecordText = SearchTools.TryFind("UI/Canvas_Menu/Panel_Win/NewRecord");
        var temp = SearchTools.TryFind("UI/Canvas_Menu/Panel_Win/Score");
        scoreImg = SearchTools.TryGetComponent<Image>(temp);
        for (int i = 0; i < 4; i++)
            scoreImgs[i] = Resources.Load<Sprite>($"Art2D/Stars/{i}stars");
    }

    void Start()
    {
        SetLevelObjects();
        Pause(false);
        playing = true;
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
        //reset some values
        playing = false;

        // stop coroutines
        StopAllCoroutines();
        if(instance != null)
            instance.StopAllCoroutines();
    }


    #region start settings functions

    /// <summary>
    /// <para> For setting everything for the gameplay level when starting those levels.</para> 
    /// <para> This method should be called when making a transition to a gameplay scene from the GameManager class.</para> 
    /// </summary>
    public static void SetGameplay()
    {
        // Set lifes
        PlayerLife.lives = PlayerLife.liveCap;
        HUD_Life.RewriteLife();

        // Wait while the other codes of the gameplay finish setting things
        if (instance != null)
        {
            if (setCorroutine != null)
            {
                instance.StopCoroutine(setCorroutine);
                setCorroutine = null;
            }
            setCorroutine = SetGameplayCorr();
            instance.StartCoroutine(setCorroutine);
        }
        
    }

    /// <summary>
    /// Corroutine for waiting till all the codes that has things to set in the level finish setting those things.
    /// </summary>
    private static IEnumerator SetGameplayCorr()
    {
        // Set saving data
        int currentLevel = SceneManager.GetActiveScene().buildIndex - 1; // - 1 because the levels starts on scene #2
        if (currentLevel > levelsUnlocked)
        {
            levelsUnlocked++;
            if (levelsUnlocked > maxLevels)
                levelsUnlocked = maxLevels;
        }
        SavingData.SaveLevelData();

        // Set Screen bounds
        while (!ScreenBounds.StartSet)
        {
            yield return null;
        }

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
        if(SceneManager.GetActiveScene().buildIndex <= maxLevels + 1)
        {
            yield return new WaitForSecondsRealtime(2f);
        }
        GameObject temp = SearchTools.TryFind("UI/Canvas_Menu/Panel_LevelEntering");
        if (temp)
            temp.SetActive(false);

        // Start the game
        GameManager.StartScene(SceneManager.GetActiveScene().buildIndex);
        setCorroutine = null;
    }

    private static void SetLevelObjects()
    {
        // Add some codes to this gameObject
        if (instance.gameObject.GetComponent<PowersSystem>() == null)
            instance.gameObject.AddComponent<PowersSystem>();
        if (instance.gameObject.GetComponent<BricksSystem>() == null)
            instance.gameObject.AddComponent<BricksSystem>();

        // Instantiate some objects
        GameObject screenBounds = Instantiate(screenBoundsPref, screenBoundsPref.transform.position, Quaternion.identity);
        screenBounds.transform.parent = instance.gameObject.transform;

        // Enable postProcessing
        PostprocessingManager.EnablePostProcessing(true);
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
            paused = true;
            playing = false;
            AudioManager.PlayAudio(AudioManager.GameAudioSource, UI_Manager.uiClips[(int)UI_Manager.UiAudioNames.pause], false, 1f);
            UI_Manager.OpenMenuLayer<GameplayMenu.Panels>(GameplayMenu.Panels.pause);
        }
        else
        {
            paused = false;
            playing = true;
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

    void OnApplicationPause(bool pauseStatus)
    {
        if (!pauseStatus || GameManager.loadingScene)
            return;
        if (!paused)
            PauseMenu(true);
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
        UI_Manager.OpenMenuLayer(GameplayMenu.Panels.lose);
    }

    /// <summary>
    /// What happens when winning the game. Call it in another code that triggers the winning.
    /// </summary>
    public static void WinGame()
    {
        playing = false;

        // New record
        int currentLevel = SceneManager.GetActiveScene().buildIndex - 1; // - 1 because the levels starts on scene #2
        bool newRecord = false;
        if (levelsScore[currentLevel - 1] < PlayerLife.lives)
            newRecord = true;

        // Audio and UI settings
        AudioManager.StopLevelSong();
        AudioManager.PlayAudio(AudioManager.GameAudioSource, winAudio, false, 1f);
        scoreImg.sprite = scoreImgs[PlayerLife.lives];
        UI_Manager.OpenMenuLayer<GameplayMenu.Panels>(GameplayMenu.Panels.win);
        if (!newRecord)
            newRecordText.SetActive(false);

        // General management
        Pause(true);
        if (newRecord)
            levelsScore[currentLevel - 1] = PlayerLife.lives;
        SavingData.SaveLevelData();
    }

    public static void LoseLive()
    {
        var ball = SearchTools.TryFindInGameobject(instance.gameObject, Ball.ballName);
        Ball ballCode = ball.GetComponent<Ball>();

        PlayerLife.lives--;
        HUD_Life.RewriteLife();
        if (PlayerLife.lives < 1)
        {
            LoseGame();
            Destroy(ballCode.gameObject);
            return;
        }
        else
        {
            AudioManager.PlayAudio(AudioManager.GameAudioSource, PlayerLife.loseLifeAudio, false, 0.4f);
            var camCode = Camera.main.GetComponent<CameraShake>();
            camCode.StartCoroutine(camCode.Shake(0.25f, 0.07f));
        }
        ballCode.RestartBall();
    }

    #endregion

}