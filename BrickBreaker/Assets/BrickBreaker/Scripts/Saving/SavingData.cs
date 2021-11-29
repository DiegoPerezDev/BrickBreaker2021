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
    private static readonly string musicSliderValKey = "musicSliderValKey", sfxSliderValKey = "sfxSliderValKey";


    public static void SaveLevelData()
    {
        PlayerPrefs.SetInt(levelsUnlockedKey, LevelManager.levelsUnlocked);
        for (int i = 0; i < LevelManager.levelsScore.Length; i++)
            PlayerPrefs.SetInt(levelsScoresKey[i], LevelManager.levelsScore[i]);
        PlayerPrefs.SetFloat(musicSliderValKey, Gameplay_UI.musicSliderVal);
        PlayerPrefs.SetFloat(sfxSliderValKey, Gameplay_UI.sfxSliderVal);
    }

    public static void SaveSettingsData()
    {
        PlayerPrefs.SetFloat(musicSliderValKey, Gameplay_UI.musicSliderVal);
        PlayerPrefs.SetFloat(sfxSliderValKey, Gameplay_UI.sfxSliderVal);
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

    public static void LoadSettingData()
    {
        Gameplay_UI.musicSliderVal = PlayerPrefs.GetFloat(musicSliderValKey);
        Gameplay_UI.sfxSliderVal = PlayerPrefs.GetFloat(sfxSliderValKey);
    }

    public static void ResetSavingData()
    {
        PlayerPrefs.SetInt(levelsUnlockedKey, 1);
        for (int i = 0; i < LevelManager.levelsScore.Length; i++)
            PlayerPrefs.SetInt(levelsScoresKey[i], 0);
    }

}