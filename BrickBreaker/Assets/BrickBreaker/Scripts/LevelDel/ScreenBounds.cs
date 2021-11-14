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
    private RectTransform leftBlock, rightBlock;

    // Screen and camera info
    private float topScreenLimit, bottomScreenLimit, rightScreenLimit, leftScreenLimit;
    public static float blackBlockWidth;
    private float camWidth;
    private Vector2 camPos;

    // Level management
    public static bool StartSet;


    void Awake()
    {
        GetScreenValues();
    }

    void Start()
    {
        SetBlackBordorBlocks();
        CreateBoundObjects();
        ResizeBoundObjects();
        PlaceBoundObjects();

        Paddle.GetScreenLimits();
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

    private void SetBlackBordorBlocks()
    {
        string temp = "UI/Canvas_HUD";
        leftBlock = SearchTools.TryFind($"{temp}/leftBlock").GetComponent<RectTransform>();
        rightBlock = SearchTools.TryFind($"{temp}/Panel_RightBlock").GetComponent<RectTransform>();

        // Set size
        float blockWeight = Camera.main.pixelWidth / 6f;
        blockWeight += blockWeight * 0.2f;
        leftBlock.sizeDelta = rightBlock.sizeDelta = new Vector2(blockWeight, leftBlock.sizeDelta.y);

        // Set pos
        leftBlock.anchoredPosition = new Vector3(leftBlock.rect.width / 2f, Camera.main.transform.position.y);
        rightBlock.anchoredPosition = new Vector3(-rightBlock.rect.width / 2f, Camera.main.transform.position.y);
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
        topBound.transform.localScale = new Vector3(camWidth, 1f, topBound.transform.localScale.z);
        bottomBound.transform.localScale = new Vector3(camWidth, 1f, bottomBound.transform.localScale.z);
        //The lateral bound colliders are not resizing with the screen dynamic resize.
        blackBlockWidth = camWidth * rightBlock.sizeDelta.x / Camera.main.pixelWidth;
        leftBound.transform.localScale = new Vector3(blackBlockWidth, topScreenLimit - bottomScreenLimit,  leftBound.transform.localScale.z);
        rightBound.transform.localScale = new Vector3(blackBlockWidth, topScreenLimit - bottomScreenLimit, rightBound.transform.localScale.z);
    }

    private void PlaceBoundObjects()
    {
        topBound.transform.position = new Vector3(camPos.x, topScreenLimit + 1f / 2f, topBound.transform.position.z);
        bottomBound.transform.position = new Vector3(camPos.x, bottomScreenLimit - 1f, bottomBound.transform.position.z);
        leftBound.transform.position = new Vector3(leftBlock.position.x, camPos.y, leftBound.transform.position.z);
        rightBound.transform.position = new Vector3(rightBlock.position.x, rightBound.transform.position.z);
    }

}