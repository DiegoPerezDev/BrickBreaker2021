using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTools;

public class HUD : MonoBehaviour
{
    public static bool ready;
    public static RectTransform blackBlockRtf;
    private static GameObject screenBoundsPref;

    void Start()
    {
        // Set HUD
        RectTransform HUDrt = gameObject.GetComponent<RectTransform>();
        RectTransform otherCanvasRt = GameObject.Find("UI/Canvas_Menu").GetComponent<RectTransform>();
        HUDrt.localScale = otherCanvasRt.localScale;
        HUDrt.sizeDelta = otherCanvasRt.sizeDelta;

        // Set black borders
        var rightBlackBlockGO = SearchTools.TryFind("UI/Canvas_HUD/Panel_RightBlock");
        RectTransform rightBlackBlockRtf;
        RectTransform leftBlackBlockRtf = SearchTools.TryFind("UI/Canvas_HUD/Panel_LeftBlock").GetComponent<RectTransform>();
        rightBlackBlockRtf = rightBlackBlockGO.GetComponent<RectTransform>();
        blackBlockRtf = rightBlackBlockRtf;
        rightBlackBlockRtf.sizeDelta = leftBlackBlockRtf.sizeDelta = new Vector2(HUDrt.sizeDelta.x * 0.16f, blackBlockRtf.sizeDelta.y);
        leftBlackBlockRtf.anchoredPosition = new Vector3(leftBlackBlockRtf.sizeDelta.x / 2f, leftBlackBlockRtf.transform.position.y, leftBlackBlockRtf.transform.position.z);
        rightBlackBlockRtf.anchoredPosition = new Vector3(-leftBlackBlockRtf.sizeDelta.x / 2f, rightBlackBlockRtf.transform.position.y, rightBlackBlockRtf.transform.position.z);

        // Set screen bound lateral colliders
        screenBoundsPref = SearchTools.TryLoadResource("Prefabs/LevelDev/ScreenBounds") as GameObject;
        GameObject screenBounds = Instantiate(screenBoundsPref, screenBoundsPref.transform.position, Quaternion.identity);
        screenBounds.transform.parent = GameObject.Find("LevelDev").transform;
        ScreenBounds screenBoundsCode = SearchTools.TryGetComponent<ScreenBounds>(screenBounds);
        screenBoundsCode.SetLevelBoundColliders();

        // Set paddle
        Paddle.GetScreenLimits(); // The paddle needs to know the size of the black borders for movement limit.

        ready = true;
    }

}