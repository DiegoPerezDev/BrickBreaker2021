using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingData : MonoBehaviour
{
    private static readonly string levelsScoresKey = "levelsScoresKey", levelsUnlockedKey = "levelsUnlockedKey";

    public static void SaveLevelData(int level)
    {
        PlayerPrefs.SetInt(levelsUnlockedKey, LevelManager.levelsUnlocked);
        PlayerPrefs.SetInt(levelsScoresKey, LevelManager.levelsScore[level - 1]);
    }

    public static void LoadData()
    {
        LevelManager.levelsUnlocked = PlayerPrefs.GetInt(levelsUnlockedKey);
        for(int i = 0; i < LevelManager.levelsScore.Length; i++)
            LevelManager.levelsScore[i] = PlayerPrefs.GetInt(levelsScoresKey);
    }

}