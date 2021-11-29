using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public sealed class UI_Manager : MonoBehaviour
{
    /*
    * - - - NOTES - - -
    - This class manage the general info of the UI that all scenes have, such as:
    -   * General UI info: current menu layer, inMenu state bool verification, and else.
    -   * Audio of the UI: source and clips.
    */


    private static UI_Manager instance;

    // General UI info
    public static bool returnTrigger, inMenu;
    public static int currentMenuLayer;
    public static GameObject currentPanel;

    // Audio
    public static AudioSource audioSource;
    [HideInInspector] public enum UiAudioNames { button, pause, unPause }
    public static AudioClip[] uiClips = new AudioClip[Enum.GetNames(typeof(UiAudioNames)).Length];


    void Awake()
    {
        // Set one instance at a time
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        else
            instance = this;

        // Get audio components
        audioSource = instance.gameObject.GetComponent<AudioSource>();
        string[] uiClipsPaths = { "(UI1) button", "(UI2) Pause", "(UI3) UnPause" };
        foreach (UiAudioNames audioClip in Enum.GetValues(typeof(UiAudioNames)))
        {
            uiClips[(int)audioClip] = Resources.Load<AudioClip>($"Audio/UI/{uiClipsPaths[(int)audioClip]}");
        }
    }

}