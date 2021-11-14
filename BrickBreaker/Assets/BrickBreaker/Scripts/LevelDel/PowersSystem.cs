using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTools;
using TMPro;
using UnityEngine.UI;

public class PowersSystem : MonoBehaviour
{
    // Data
    public static readonly float maxPowerTime = 8;
    public static bool newSizePower, newSpeedPower;
    public static int powersSpawned;

    // Enumerators
    public enum Power { none, small, large, slow, fast }
    public static Power currentSizePower, currentSpeedPower, previousSizePower, previousSpeedPower;
    public enum PowerType { size, speed }

    // Objects
    public static PowerTimer sizePowerTimer, speedPowerTimer;

    // Audio
    public static AudioSource powersAudioSource;
    public static AudioClip getPowerAudio;


    void Awake()
    {
        // GameObject components
        string TimersGOpath = "UI/Canvas_HUD/Panel_RightBlock/Timers/";
        GameObject tempGO = GameObject.Find($"{TimersGOpath}SizePowerTimer");
        sizePowerTimer = tempGO.GetComponent<PowerTimer>();
        tempGO = GameObject.Find($"{TimersGOpath}SpeedPowerTimer");
        speedPowerTimer = tempGO.GetComponent<PowerTimer>();

        // Audio components
        powersAudioSource = GameObject.Find("LevelDev/Bricks_").AddComponent<AudioSource>();
        getPowerAudio = SearchTools.TryLoadResource("Audio/Level objects/(lo1) get power") as AudioClip;
    }

}