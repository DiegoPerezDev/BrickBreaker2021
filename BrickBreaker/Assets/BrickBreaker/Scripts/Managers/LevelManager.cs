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
    public static bool ballReleased = false;
    public static int lives;

    // Management
    private static LevelManager instance;
    private static IEnumerator setCorroutine;
    public static bool levelReady;


    void Awake()
    {
        instance = this;
    }

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
        ballReleased = false;
        while (!Ball.StartSet)
        {
            yield return null;
        }

        // Tell the GameManager this code is ready
        levelReady = true;

        // Restart some values for the next time we enter the gameplay
        setCorroutine = null;
        ScreenBounds.StartSet = BrickGenerator.StartSet = Paddle.StartSet = Ball.StartSet = false;
    }

    public static void CheckNumberOfBricks()
    {
        
        if (numberOfActiveBricks == 0)
        {
            Destroy(SearchTools.TryFind("LevelDev/Ball_"));
            GameManager.WinGame();
        }
    }

}
