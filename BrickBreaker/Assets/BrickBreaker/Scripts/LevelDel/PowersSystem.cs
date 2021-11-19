using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTools;

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
    public static HUD_PowerTimer sizePowerTimer, speedPowerTimer;
    public static AudioSource powersAudioSource;
    public static AudioClip getPowerAudio;
    public static GameObject fastPowerCapsule, slowPowerCapsule, smallPowerCapsule, largePowerCapsule;


    void Awake()
    {
        // Get power prefabs
        string path = "Prefabs/LevelDev/Powers";
        fastPowerCapsule = Resources.Load<GameObject>($"{path}/PowerFast");
        slowPowerCapsule = Resources.Load<GameObject>($"{path}/PowerSlow");
        smallPowerCapsule = Resources.Load<GameObject>($"{path}/PowerSmall");
        largePowerCapsule = Resources.Load<GameObject>($"{path}/PowerLarge");

        // Timmers components
        string TimersGOpath = "UI/Canvas_HUD/Panel_RightBlock/Timers/";
        GameObject tempGO = GameObject.Find($"{TimersGOpath}SizePowerTimer");
        sizePowerTimer = tempGO.GetComponent<HUD_PowerTimer>();
        tempGO = GameObject.Find($"{TimersGOpath}SpeedPowerTimer");
        speedPowerTimer = tempGO.GetComponent<HUD_PowerTimer>();

        // Audio components
        powersAudioSource = GameObject.Find("LevelDev/Bricks_").AddComponent<AudioSource>();
        getPowerAudio = SearchTools.TryLoadResource("Audio/Level objects/(lo1) get power") as AudioClip;
    }

    private void OnDestroy()
    {
        powersSpawned = 0;
        ResetPowers();
    }

    public static void ResetPowers() => currentSizePower = currentSpeedPower = previousSizePower = previousSpeedPower = Power.none;

}