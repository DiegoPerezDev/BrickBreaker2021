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

}
