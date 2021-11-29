using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PanelManagement : MonoBehaviour
{

    /// <summary>
    /// Activate the gameObject that should be a menu panel and also close the current opened menu panel.
    /// </summary>
    protected static void OpenNewMenu(GameObject newPanelGO, GameObject currentPanelGO)
    {
        // Open new menu
        newPanelGO.SetActive(true);

        // Close previous menu
        if (UI_Manager.currentMenuLayer > 0)
            currentPanelGO.SetActive(false);

        // Deselect any selected button
        UI_Navigation.DeselectButton();

        // Set manager info
        UI_Manager.currentPanel = newPanelGO;
        UI_Manager.inMenu = true;
        UI_Manager.currentMenuLayer++;
    }

    /// <summary>
    /// Open the panel from the previous layer and close the current one.
    /// </summary>
    protected static void ReturnToPreviousPanel(GameObject panelToOpen, GameObject panelToClose)
    {
        // Avoid continue if there is no previous panel.
        if (UI_Manager.currentMenuLayer < 1)
            return;

        // Audio
        AudioManager.PlayAudio(AudioManager.GameAudioSource, UI_Manager.uiClips[(int)UI_Manager.UiAudioNames.unPause], false, 1f);

        // Deselect any selected button
        UI_Navigation.DeselectButton();

        // Open previous panel
        if (panelToOpen != null)
        {
            UI_Manager.currentPanel = panelToOpen;
            panelToOpen.SetActive(true);
        }

        // Close current panel
        if (panelToClose != null)
            panelToClose.SetActive(false);

        // General UI settings
        UI_Manager.currentMenuLayer--;
        if (UI_Manager.currentMenuLayer == 0)
            UI_Manager.inMenu = false;
    }

}