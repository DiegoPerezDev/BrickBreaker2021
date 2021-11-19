using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingData : MonoBehaviour
{
    // Player pref keys
    private static readonly string[] levelsScoresKey = { "level1ScoreKey", "level2ScoreKey",
        "level3ScoreKey", "level4ScoreKey", "level5ScoreKey", "level6ScoreKey", "level7ScoreKey",
        "level8ScoreKey", "level9ScoreKey", "level10ScoreKey" };
    private static readonly string levelsUnlockedKey = "levelsUnlockedKey";


    public static void SaveLevelData()
    {
        PlayerPrefs.SetInt(levelsUnlockedKey, LevelManager.levelsUnlocked);
        for (int i = 0; i < LevelManager.levelsScore.Length; i++)
            PlayerPrefs.SetInt(levelsScoresKey[i], LevelManager.levelsScore[i]);
    }

    public static void LoadLevelData()
    {
        LevelManager.levelsUnlocked = PlayerPrefs.GetInt(levelsUnlockedKey);
        if (LevelManager.levelsUnlocked < 1)
            LevelManager.levelsUnlocked = 1;
        else if (LevelManager.levelsUnlocked > LevelManager.maxLevels)
            LevelManager.levelsUnlocked = LevelManager.maxLevels;
        for (int i = 0; i < LevelManager.levelsScore.Length; i++)
            LevelManager.levelsScore[i] = PlayerPrefs.GetInt(levelsScoresKey[i]);
    }

}