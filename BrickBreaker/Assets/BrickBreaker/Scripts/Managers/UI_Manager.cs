using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using MyTools;

public class UI_Manager : MonoBehaviour
{
    /*
    * - - - NOTES - - -
    - Returning to previous panel when using a return trigger input.
    - The opening and closing of panels for any scene, the specific menu codes calls these methods.
    */


    // For transition management
    private static UI_Manager instance;

    // For panel management
    public static int currentMenuLayer;
    public static bool inMenu;

    // Inputs
    public static bool returnTrigger;

    // Audio
    public static AudioSource audioSource;
    [HideInInspector] public enum UiAudioNames { button, pause, unPause }
    public static AudioClip[] uiClips = new AudioClip[Enum.GetNames(typeof(UiAudioNames)).Length];


    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        SetAudio();
    }

    void Update()
    {
        if (GameManager.loadingScene)
            return;

        // Return to previous menu
        if (returnTrigger)
        {
            returnTrigger = false;
            ReturnToPreviousMenu();
        }
    }


    private static void SetAudio()
    {
        audioSource = SearchTools.TryGetComponent<AudioSource>(instance.gameObject);
        string[] uiClipsPaths = { "(UI1) button", "(UI2) Pause", "(UI3) UnPause" };
        foreach (UiAudioNames audioClip in Enum.GetValues(typeof(UiAudioNames)))
        {
            uiClips[(int)audioClip] = SearchTools.TryLoadResource($"Audio/UI/{uiClipsPaths[(int)audioClip]}") as AudioClip;
        }
    }

    private static void ReturnToPreviousMenu()
    {
        if (inMenu)
        {
            AudioManager.PlayAudio(AudioManager.GameAudioSource, uiClips[(int)UiAudioNames.unPause], false, 1f);

            if (GameManager.currentSceneType == GameManager.SceneType.gameplay)
            {
                if (GameplayMenu.openedMenus[UI_Manager.currentMenuLayer] == GameplayMenu.Panels.pause)
                    LevelManager.PauseMenu(false);
                else
                    ClosePanel();
            }
            else if (GameManager.currentSceneType == GameManager.SceneType.Mainmenu)
            {
                if (currentMenuLayer > 1)
                    ClosePanel();
            }
        }
    }


    public static void OpenMenuLayer<T>(T panelToGo)
    {
        inMenu = true;

        GameObject[] panelGameObject = null;
        currentMenuLayer++;

        // Remember menu
        if (GameManager.currentSceneType == GameManager.SceneType.Mainmenu)
        {
            panelGameObject = MainMenu.panelGO;
            MainMenu.openedMenus[currentMenuLayer] = (MainMenu.Panels)((object)panelToGo);
        }
        else if (GameManager.currentSceneType == GameManager.SceneType.gameplay)
        {
            panelGameObject = GameplayMenu.panelGO;
            GameplayMenu.openedMenus[currentMenuLayer] = (GameplayMenu.Panels)((object)panelToGo);
        }

        // Open menu
        try
        {
            panelGameObject[(int)((object)panelToGo)].SetActive(true);
        }
        catch (ArgumentNullException)
        {
            Debug.Log($"Could not open the panel: {nameof(panelToGo)}, because of a null exception");
            return;
        }
        catch (Exception)
        {
            Debug.Log($"Could not open the panel: {nameof(panelToGo)}. Exception different to null.");
            return;
        }
    }

    public static void ClosePanel()
    {
        GameObject[] panelGameObject = null;
        int panelToClose = -1;

        if (currentMenuLayer > 0)
            currentMenuLayer--;

        if (currentMenuLayer == 0)
            inMenu = false;

        if (GameManager.currentSceneType == GameManager.SceneType.Mainmenu)
        {
            panelGameObject = MainMenu.panelGO;
            panelToClose = (int)MainMenu.openedMenus[currentMenuLayer + 1];
            MainMenu.openedMenus[currentMenuLayer + 1] = MainMenu.Panels.none;
        }
        else if (GameManager.currentSceneType == GameManager.SceneType.gameplay)
        {
            panelGameObject = GameplayMenu.panelGO;
            panelToClose = (int)GameplayMenu.openedMenus[currentMenuLayer + 1];
            GameplayMenu.openedMenus[currentMenuLayer] = GameplayMenu.Panels.none;
        }

        if(panelToClose < 0)
        {
            print("Could not close menu because of the \"panelToClose\" var.");
            return;
        }

        try
        {
            panelGameObject[panelToClose].SetActive(false);
        }
        catch (ArgumentNullException)
        {
            Debug.Log($"Could not close the panel because of a null exception");
            return;
        }
        catch (Exception)
        {
            Debug.Log($"Could not close the panel. Exception different to null.");
            return;
        }
    }

}