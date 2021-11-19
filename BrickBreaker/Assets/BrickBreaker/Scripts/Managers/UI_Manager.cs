using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using MyTools;

public class UI_Manager : MonoBehaviour
{
    /*
    * - - - NOTES - - -
    - This class manage the general info of the UI that all scenes have, such as:
    -   * current menu layer
    -   * inMenu state bool verification
    -   * Audio of the UI
    */


    private static UI_Manager instance;
    public static bool returnTrigger;
    public static int currentMenuLayer;
    public static bool inMenu;

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
        audioSource = SearchTools.TryGetComponent<AudioSource>(instance.gameObject);
        string[] uiClipsPaths = { "(UI1) button", "(UI2) Pause", "(UI3) UnPause" };
        foreach (UiAudioNames audioClip in Enum.GetValues(typeof(UiAudioNames)))
        {
            uiClips[(int)audioClip] = SearchTools.TryLoadResource($"Audio/UI/{uiClipsPaths[(int)audioClip]}") as AudioClip;
        }
    }


}