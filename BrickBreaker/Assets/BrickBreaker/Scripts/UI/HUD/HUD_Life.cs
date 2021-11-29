using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD_Life : MonoBehaviour
{
    private static TextMeshProUGUI lifeTmp;

    void Start()
    {
        // Get components
        GameObject lifeTmpGO = GameObject.Find("UI/Canvas_HUD/Panel_LeftBlock/Life");
        if(lifeTmpGO)
            lifeTmp = lifeTmpGO.GetComponent<TextMeshProUGUI>();

        // First live set
        RewriteLife(PlayerLife.liveCap);
    }

    public static void RewriteLife() => lifeTmp.text = $"Lives: {PlayerLife.lives}";
    public static void RewriteLife(int lives) =>  lifeTmp.text = $"Lives: {lives}";

}