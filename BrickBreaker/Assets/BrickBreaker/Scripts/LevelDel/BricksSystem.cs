using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTools;

public class BricksSystem : MonoBehaviour
{
    // Audio
    public static AudioSource[] bricksAudioSources = new AudioSource[5];
    public static AudioClip hitAudio, metalHitAudio, destructionAudio;


    void Awake()
    {
        // Get audio components
        for (int i = 0; i < bricksAudioSources.Length; i++)
            bricksAudioSources[i] = GameObject.Find("LevelDev/Bricks_").AddComponent<AudioSource>();
        hitAudio = SearchTools.TryLoadResource("Audio/Level objects/(gs1) brick getting hit") as AudioClip;
        metalHitAudio = SearchTools.TryLoadResource("Audio/Level objects/(lo1) metalBrick") as AudioClip;
        destructionAudio = SearchTools.TryLoadResource("Audio/Level objects/(gs1) brick getting crushed") as AudioClip;
    }

    public static void CheckNumberOfBricks()
    {
        int numberOfActiveBricks = GameObject.Find("LevelDev/Bricks_").transform.childCount - 1;
        if (numberOfActiveBricks <= 0)
        {
            Destroy(SearchTools.TryFind(Ball.ballPath));
            LevelManager.WinGame();
        }
        //else if (numberOfActiveBricks <= 3)
        //{
        //    if (!lastBrickCounter)
        //    {
        //        lastBrickCounter = true;
        //        GameObject.Find("UI/Canvas_HUD/Panel_RightBlock/Timers/FinalTimer").GetComponent<LastBricksCounter>().StartTimer();
        //    }
        //}
    }

}