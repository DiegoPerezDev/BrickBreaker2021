using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTools;

public class BricksSystem : MonoBehaviour
{
    public static AudioSource[] bricksAudioSources = new AudioSource[5];
    public static AudioClip hitAudio, metalHitAudio, destructionAudio;
    private static GameObject bricksContainer;


    void Awake()
    {
        // Get gameobjects
        bricksContainer = GameObject.Find("LevelDev/Bricks_");

        // Get audio components
        for (int i = 0; i < bricksAudioSources.Length; i++)
            bricksAudioSources[i] = GameObject.Find("LevelDev/Bricks_").AddComponent<AudioSource>();
        hitAudio = SearchTools.TryLoadResource("Audio/Level objects/(gs1) brick getting hit") as AudioClip;
        metalHitAudio = SearchTools.TryLoadResource("Audio/Level objects/(lo1) metalBrick") as AudioClip;
        destructionAudio = SearchTools.TryLoadResource("Audio/Level objects/(gs1) brick getting crushed") as AudioClip;
    }
    
    /// <summary>
    /// Check the remaining bricks in the level to know if the player already won.
    /// </summary>
    public static void CheckNumberOfBricks()
    {
        int numberOfActiveBricks = bricksContainer.transform.childCount - 1;
        if (numberOfActiveBricks <= 0)
        {
            GameObject ball = GameObject.Find(Ball.ballPath);
            if(ball != null)
                Destroy(ball);
            LevelManager.WinGame();
        }
    }

}