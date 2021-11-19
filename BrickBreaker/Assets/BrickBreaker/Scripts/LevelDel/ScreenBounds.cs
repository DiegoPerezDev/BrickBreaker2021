using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTools;

public class ScreenBounds : MonoBehaviour
{
    /*
     * NOTES:
     * This code place the screen colliders bounds so the ball cannot get outside of the screen view.
     * This code also set the lateral black panels that also work as HUD.
     */

    // Screen bounds objects
    private GameObject leftBound, rightBound, topBound, bottomBound;

    // Screen and camera info
    private float topScreenLimit, bottomScreenLimit;
    private float camWidth;
    private Vector2 camPos;

    // Level management
    public static bool StartSet;


    public void SetLevelBoundColliders()
    {
        GetScreenValues();
        CreateBoundObjects();
        ResizeBoundObjects();
        PlaceBoundObjects();
        StartSet = true;
    }

    void OnDestroy()
    {
        StartSet = false;
    }


    private void GetScreenValues()
    {
        // Get camera values for the screen boundaries
        camPos = Camera.main.gameObject.transform.position;
        float camHeight = Camera.main.orthographicSize * 2;
        camWidth = camHeight * Camera.main.aspect;
        topScreenLimit = camPos.y + (camHeight / 2);
        bottomScreenLimit = camPos.y - (camHeight / 2);
    }

    private void CreateBoundObjects()
    {
        // Create objects
        topBound = new GameObject("TopBound");
        bottomBound = new GameObject("BottomBound");
        bottomBound.tag = "BottomBound";
        leftBound = new GameObject("LeftBound");
        rightBound = new GameObject("RightBound");

        // Set objects
        GameObject respectiveBound = null;
        for(int i = 0; i < 4; i++)
        {
            if (i == 0)
                respectiveBound = topBound;
            else if (i == 1)
                respectiveBound = bottomBound;
            else if (i == 2)
                respectiveBound = leftBound;
            else if (i == 3)
                respectiveBound = rightBound;

            respectiveBound.AddComponent<BoxCollider2D>();
            respectiveBound.transform.parent = gameObject.transform;
        }
    }

    private void ResizeBoundObjects()
    {
        topBound.transform.localScale = new Vector3(camWidth, 1f, topBound.transform.localScale.z);
        bottomBound.transform.localScale = new Vector3(camWidth, 1f, bottomBound.transform.localScale.z);
        float blackPanelWidth = HUD.blackBlockRtf.rect.width * HUD.blackBlockRtf.transform.lossyScale.x;
        leftBound.transform.localScale = new Vector3(blackPanelWidth, topScreenLimit - bottomScreenLimit,  leftBound.transform.localScale.z);
        rightBound.transform.localScale = new Vector3(blackPanelWidth, topScreenLimit - bottomScreenLimit, rightBound.transform.localScale.z);
    }

    private void PlaceBoundObjects()
    {
        topBound.transform.position = new Vector3(camPos.x, topScreenLimit + 1f / 2f, topBound.transform.position.z);
        bottomBound.transform.position = new Vector3(camPos.x, bottomScreenLimit - 1f, bottomBound.transform.position.z);
        leftBound.transform.position = new Vector3(-HUD.blackBlockRtf.position.x, camPos.y, leftBound.transform.position.z);
        rightBound.transform.position = new Vector3(HUD.blackBlockRtf.position.x, rightBound.transform.position.z);
    }

}