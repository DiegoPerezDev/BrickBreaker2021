using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int levelsUnlocked;
    public int[] levelsScore = new int[10];
    
    public SaveData()
    {
        levelsUnlocked = LevelManager.levelsUnlocked;
        levelsScore = LevelManager.levelsScore;
    }

    public static void LoadData(SaveData data)
    {
        LevelManager.levelsUnlocked = data.levelsUnlocked;
        LevelManager.levelsScore = data.levelsScore;
    }

}
