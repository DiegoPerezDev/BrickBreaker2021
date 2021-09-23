using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTools;

public class LevelManager : MonoBehaviour
{
    /*
    * - - - NOTES - - -
    - This class is for everything that happens only in the gameplay scenes (levels).
    - This class starts void with just one void method because everything for setting is different in each game.
    */

    // Level data
    public static int numberOfActiveBricks;
    
    public static int lives;

    // Level objects
    private static GameObject paddlePref, ballPref, screenBoundsPref;

    // Management
    private static LevelManager instance;
    private static IEnumerator setCorroutine;
    public static bool levelReady;

    // Audio
    public static AudioSource[] brickAudioSources;
    public static AudioClip hitAudio, destructionAudio;


    void Awake()
    {
        instance = this;

        // Audio
        brickAudioSources = GameObject.Find("LevelDev/Bricks_").GetComponents<AudioSource>();
        hitAudio = SearchTools.TryLoadResource("Audio/Level objects/(gs1) brick getting hit") as AudioClip;
        destructionAudio = SearchTools.TryLoadResource("Audio/Level objects/(gs1) brick getting crushed") as AudioClip;

        // Prefabs
        paddlePref = SearchTools.TryLoadResource("Prefabs/LevelDev/Paddle_") as GameObject;
        ballPref = SearchTools.TryLoadResource("Prefabs/LevelDev/Ball_") as GameObject;
        screenBoundsPref = SearchTools.TryLoadResource("Prefabs/LevelDev/ScreenBounds") as GameObject;
    }

    void Start()
    {
        InstantiateLevelObjects();
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
        // Set lifes
        lives = 3;
        UI_Manager.RewriteLife();

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
        setCorroutine = null;
    }

    private static void InstantiateLevelObjects()
    {
        GameObject paddle = Instantiate(paddlePref, paddlePref.transform.position, Quaternion.identity);
        paddle.transform.parent = instance.gameObject.transform;
        paddle.GetComponent<Paddle>().Begin();
        GameObject ball = Instantiate(ballPref, ballPref.transform.position, Quaternion.identity);
        ball.transform.parent = instance.gameObject.transform;
        ball.GetComponent<Ball>().Begin();
        GameObject screenBounds = Instantiate(screenBoundsPref, screenBoundsPref.transform.position, Quaternion.identity);
        screenBounds.transform.parent = instance.gameObject.transform;
        screenBounds.GetComponent<ScreenBounds>().Begin();
    }

    #endregion

    public static void CheckNumberOfBricks()
    {
        
        if (numberOfActiveBricks == 0)
        {
            Destroy(SearchTools.TryFind("LevelDev/Ball_(Clone)"));
            GameManager.WinGame();
        }
    }

}
