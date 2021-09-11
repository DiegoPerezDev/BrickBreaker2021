using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBounds : MonoBehaviour
{
    /*
     * NOTES:
     * This code place the screen colliders bounds so the ball cannot get outside of the scrreen view.
     */

    // Screen bounds objects
    private enum Bounds { leftBound, topBound, rightBound }
    private Bounds bounds;
    private GameObject leftBound, rightBound, topBound;
    private readonly float boundThickness = 1f;

    // Screen and camera info
    private float leftScreenLimit, rightScreenLimit, topScreenLimit, bottomScreenLimit;
    private float camWidth;
    private Vector2 camPos;

    // Level management
    public static bool StartSet;


    void Start()
    {
        GetScreenValues();
        GetBoundObjects();
        ResizeBoundObjects();
        PlaceBoundObjects();
        StartSet = true;
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

    private void GetBoundObjects()
    {
        switch(this.gameObject.name)
        {
            case "TopCollider_":
                bounds = Bounds.topBound;
                topBound = GameObject.Find("LevelDev/TopCollider_");
                if(topBound == null)
                {
                    print("topBound game object was not found!");
                    Destroy(this);
                }
                break;

            case "LeftCollider_":
                bounds = Bounds.leftBound;
                leftBound = GameObject.Find("LevelDev/LeftCollider_");
                if (leftBound == null)
                {
                    print("leftBound game object was not found!");
                    Destroy(this);
                }
                break;

            case "RightCollider_":
                bounds = Bounds.rightBound;
                rightBound = GameObject.Find("LevelDev/RightCollider_");
                if (rightBound == null)
                {
                    print("rightBound game object was not found!");
                    Destroy(this);
                }
                break;

            default:
                print("Background object not recognized!");
                Destroy(this);
                break;
        }
    }

    private void ResizeBoundObjects()
    {
        switch(bounds)
        {
            case Bounds.topBound:
                topBound.transform.localScale = new Vector3(camWidth, boundThickness, topBound.transform.localScale.z);
                break;

            case Bounds.leftBound:
                leftBound.transform.localScale = new Vector3(boundThickness, topScreenLimit - bottomScreenLimit,  leftBound.transform.localScale.z);
                break;

            case Bounds.rightBound:
                rightBound.transform.localScale = new Vector3(boundThickness, topScreenLimit - bottomScreenLimit, rightBound.transform.localScale.z);
                break;
        }
    }

    private void PlaceBoundObjects()
    {
        switch (bounds)
        {
            case Bounds.topBound:
                topBound.transform.position = new Vector3(camPos.x, topScreenLimit + boundThickness/2, topBound.transform.position.z);
                break;

            case Bounds.leftBound:
                leftBound.transform.position = new Vector3(leftScreenLimit - boundThickness / 2, camPos.y, leftBound.transform.position.z);
                break;

            case Bounds.rightBound:
                rightBound.transform.position = new Vector3(rightScreenLimit + boundThickness / 2, camPos.y, rightBound.transform.position.z);
                break;
        }
    }

}