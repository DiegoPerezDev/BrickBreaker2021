using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_Manager : MonoBehaviour
{
    /*
     * INSTRUCTIONS:
     * This class manage the behaviour of the scene itself, for the UI management use the 'MainMenu_UI' script instead.
     */

    private static IEnumerator SetCor;

    void Start()
    {
        // Input set
        InputsManager.gameplayInputsDisabled = true;

        // Camera set
        PostprocessingManager.EnablePostProcessing(false);

        // UI set
        UI_Manager.inMenu = true;
        SetCor = MainmenuSetDelay();
        StartCoroutine(SetCor);
    }

    void OnDestroy()
    {
        if (SetCor != null)
            StopCoroutine(SetCor);
        StopAllCoroutines();
    }

    /// <summary>
    /// Set the main menu scene. Only for using it in the 'LoadScene' corroutine.
    /// </summary>
    private static IEnumerator MainmenuSetDelay()
    {
        // Wait for the main menu UI setting
        while (!MainMenu_UI.ready)
            yield return null;
        MainMenu_UI.ready = false;

        // Start the scene
        GameManager.settingScene = false;
        GameManager.StartScene(0);
    }

}