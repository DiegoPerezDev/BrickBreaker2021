using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BricksSystem : MonoBehaviour
{
    public static AudioSource[] bricksAudioSources = new AudioSource[5];
    public static AudioClip hitAudio, metalHitAudio, destructionAudio;
    private static GameObject bricksContainer;
    private static BricksSystem instance;


    void Awake()
    {
        instance = this;

        // Get gameobjects
        bricksContainer = GameObject.Find("LevelDev/Bricks_");

        // Get audio components
        for (int i = 0; i < bricksAudioSources.Length; i++)
        {
            bricksAudioSources[i] = GameObject.Find("LevelDev/Bricks_").AddComponent<AudioSource>();
            bricksAudioSources[i].outputAudioMixerGroup = AudioManager.sfxMixerGroup;
        }
        hitAudio = Resources.Load<AudioClip>("Audio/Level objects/(gs1) brick getting hit");
        metalHitAudio = Resources.Load<AudioClip>("Audio/Level objects/(lo1) metalBrick");
        destructionAudio = Resources.Load<AudioClip>("Audio/Level objects/(gs1) brick getting crushed");
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
            if (ball != null)
            {
                Destroy(ball);
                LevelManager.WinGame();
            }
        }
        // Check again when there are less than two bricks, so there are no error when destroying the last two bricks at the same time
        else if(numberOfActiveBricks <= 2)
            instance.Invoke("CheckNumberOfBricks", 0.1f);
    }

}