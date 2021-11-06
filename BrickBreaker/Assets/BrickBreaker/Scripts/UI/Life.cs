using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Life : MonoBehaviour
{
    private static TextMeshProUGUI lifeTmp;

    void Awake()
    {
        GameObject lifeTmpGO = GameObject.Find("UI/Canvas_HUD/Life");
        if(lifeTmpGO)
            lifeTmp = lifeTmpGO.GetComponent<TextMeshProUGUI>();
    }

    private static void RestartLive()
    {
        if (lifeTmp != null)
            lifeTmp.text = "Life: " + LevelManager.liveCap;
    }

    public static void RewriteLife()
    {
        lifeTmp.text = "Life: " + LevelManager.lives;
    }

}
