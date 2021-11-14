using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD_Life : MonoBehaviour
{
    private static TextMeshProUGUI lifeTmp;

    void Awake()
    {
        GameObject lifeTmpGO = GameObject.Find("UI/Canvas_HUD/Life");
        if(lifeTmpGO)
            lifeTmp = lifeTmpGO.GetComponent<TextMeshProUGUI>();
    }

    public static void RewriteLife()
    {
        lifeTmp.text = "Life: " + PlayerLife.lives;
    }

}
