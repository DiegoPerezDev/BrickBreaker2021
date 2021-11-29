using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public static bool ready;
    public static RectTransform blackBlockRtf;
    private static float borderScale = 0.16f;
    private static GameObject screenBoundsPref;
    private RectTransform HUDrt;

    void Start()
    {
        // Set HUD Codes
        HUDrt = gameObject.GetComponent<RectTransform>();
        SetHUD_Transform(ref HUDrt);
        SetBlackBorders(ref HUDrt);

        // Set other Codes that need the HUD data.
        InstanceAndSet_ScreenBounds();
        Paddle.GetScreenLimits(); // The paddle needs to know the size of the black borders for movement limit.

        ready = true;
    }

    private void SetHUD_Transform(ref RectTransform HUDrectTransform)
    {
        RectTransform otherCanvasRt = GameObject.Find("UI/Canvas_Menu").GetComponent<RectTransform>();
        HUDrectTransform.localScale = otherCanvasRt.localScale;
        HUDrectTransform.sizeDelta = otherCanvasRt.sizeDelta;
    }

    private void SetBlackBorders(ref RectTransform HUDrectTransform)
    {
        var rightBlackBlockGO = GameObject.Find("UI/Canvas_HUD/Panel_RightBlock");
        RectTransform rightBlackBlockRtf;
        RectTransform leftBlackBlockRtf = GameObject.Find("UI/Canvas_HUD/Panel_LeftBlock").GetComponent<RectTransform>();
        rightBlackBlockRtf = rightBlackBlockGO.GetComponent<RectTransform>();
        blackBlockRtf = rightBlackBlockRtf;
#if UNITY_STANDALONE
    borderScale = 0.16f;
#endif
#if UNITY_ANDROID
    borderScale = 0.20f;
#endif
        rightBlackBlockRtf.sizeDelta = leftBlackBlockRtf.sizeDelta = new Vector2(HUDrectTransform.sizeDelta.x * borderScale, blackBlockRtf.sizeDelta.y);
        leftBlackBlockRtf.anchoredPosition = new Vector3(leftBlackBlockRtf.sizeDelta.x / 2f, leftBlackBlockRtf.transform.position.y, leftBlackBlockRtf.transform.position.z);
        rightBlackBlockRtf.anchoredPosition = new Vector3(-leftBlackBlockRtf.sizeDelta.x / 2f, rightBlackBlockRtf.transform.position.y, rightBlackBlockRtf.transform.position.z);
    }

    private void InstanceAndSet_ScreenBounds()
    {
        screenBoundsPref = Resources.Load<GameObject>("Prefabs/LevelDev/ScreenBounds");
        GameObject screenBounds = Instantiate(screenBoundsPref, screenBoundsPref.transform.position, Quaternion.identity);
        screenBounds.transform.parent = GameObject.Find("LevelDev").transform;
        screenBounds.GetComponent<ScreenBounds>().SetLevelBoundColliders();
    }

}