using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTools;
using TMPro;
using UnityEngine.UI;

public class Powers : MonoBehaviour
{
    // Data
    public static readonly float maxPowerTime = 8;

    // Enumerators
    public enum Power { none, small, large, slow, fast }
    public static Power nextPower, currentSizePower, currentSpeedPower;
    public enum PowerType { size, speed }
    public static PowerType newPowerType;

    // Objects
    public static PowerTimer sizePowerTimer, speedPowerTimer;


    void Awake()
    {
        string TimersGOpath = "UI/Canvas_HUD/Panel_RightBlock/Timers/";
        GameObject tempGO = GameObject.Find($"{TimersGOpath}SizePowerTimer");
        sizePowerTimer = tempGO.GetComponent<PowerTimer>();
        tempGO = GameObject.Find($"{TimersGOpath}SpeedPowerTimer");
        speedPowerTimer = tempGO.GetComponent<PowerTimer>();
    }

}