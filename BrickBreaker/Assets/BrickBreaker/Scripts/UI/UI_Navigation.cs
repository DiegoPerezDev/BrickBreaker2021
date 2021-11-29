using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Navigation
{
    public static void DeselectButton()
    {
        GameObject SelectedButtonGO = EventSystem.current.currentSelectedGameObject;
        if (SelectedButtonGO == null)
            return;
        Button currentButton = SelectedButtonGO.GetComponent<Button>();
        if (currentButton != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    public static void SelectFirstButton()
    {
        // Check if there is no button already selected
        GameObject currentButtonGO = EventSystem.current.currentSelectedGameObject;
        if (currentButtonGO != null)
            return;

        // Get the upper lefter button in the active panel
        float camHalfHeight = Camera.main.orthographicSize;
        float camHalfWidth = camHalfHeight * Camera.main.aspect;
        Vector2 firstButtonPos = new Vector2(camHalfWidth, -camHalfHeight);
        Button firstPanelButton = null;
        foreach (Button button in UI_Manager.currentPanel.GetComponentsInChildren<Button>())
        {
            Vector3 buttonPos = button.gameObject.transform.position;
            if (buttonPos.y <= firstButtonPos.y)
            {
                if (buttonPos.x >= firstButtonPos.x)
                    continue;
            }
            firstButtonPos = button.gameObject.transform.position;
            firstPanelButton = button;
        }

        // Select the first button
        if (firstPanelButton != null)
            firstPanelButton.Select();
    }
}
