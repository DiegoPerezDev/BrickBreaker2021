using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTools;

public class ScreenBounds : MonoBehaviour
{
    /*
     * NOTES:
     * This code place the screen colliders bounds so the ball cannot get outside of the screen view.
     */

    // Screen bounds objects
    private GameObject leftBound, rightBound, topBound, bottomBound;
    private readonly float boundThickness = 1f;

    // Screen and camera info
    private float leftScreenLimit, rightScreenLimit, topScreenLimit, bottomScreenLimit;
    private float camWidth;
    private Vector2 camPos;

    // Level management
    public static bool StartSet;


    public void Begin()
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
        leftScreenLimit = camPos.x - (camWidth / 2);
        rightScreenLimit = camPos.x + (camWidth / 2);
        topScreenLimit = camPos.y + (camHeight / 2);
        bottomScreenLimit = camPos.y - (camHeight / 2);
    }

    private void CreateBoundObjects()
    {
        topBound = new GameObject("TopBound");
        topBound.AddComponent<BoxCollider2D>();
        topBound.transform.parent = gameObject.transform;

        bottomBound = new GameObject("BottomBound");
        bottomBound.AddComponent<BoxCollider2D>();
        bottomBound.tag = "BottomBound";
        bottomBound.transform.parent = gameObject.transform;

        leftBound = new GameObject("LeftBound");
        leftBound.AddComponent<BoxCollider2D>();
        leftBound.transform.parent = gameObject.transform;

        rightBound = new GameObject("RightBound");
        rightBound.AddComponent<BoxCollider2D>();
        rightBound.transform.parent = gameObject.transform;
    }

    private void ResizeBoundObjects()
    {
        topBound.transform.localScale = new Vector3(camWidth, boundThickness, topBound.transform.localScale.z);
        bottomBound.transform.localScale = new Vector3(camWidth, boundThickness, bottomBound.transform.localScale.z);
        leftBound.transform.localScale = new Vector3(boundThickness, topScreenLimit - bottomScreenLimit,  leftBound.transform.localScale.z);
        rightBound.transform.localScale = new Vector3(boundThickness, topScreenLimit - bottomScreenLimit, rightBound.transform.localScale.z);
    }

    private void PlaceBoundObjects()
    {
        topBound.transform.position = new Vector3(camPos.x, topScreenLimit + boundThickness/2, topBound.transform.position.z);
        bottomBound.transform.position = new Vector3(camPos.x, bottomScreenLimit - boundThickness, bottomBound.transform.position.z);
        leftBound.transform.position = new Vector3(leftScreenLimit - boundThickness / 2, camPos.y, leftBound.transform.position.z);
        rightBound.transform.position = new Vector3(rightScreenLimit + boundThickness / 2, camPos.y, rightBound.transform.position.z);
    }

}