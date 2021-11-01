using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTools;
using TMPro;
using UnityEngine.UI;

public class Powers : MonoBehaviour
{
    public static float powerTime = 8;
    public static int powersSpawned;

    public static GameObject powerTimer;
    private static TextMeshProUGUI timerText;
    private static Image timer;

    private static Powers instance;
    private static IEnumerator timerCor;


    void Awake()
    {
        instance = this;
        powerTimer = SearchTools.TryFind("UI/UI_Gameplay/Canvas_InPlay/Panel_HUD/PowerTimer");
        var temp = SearchTools.TryFindInGameobject(powerTimer, "Timer");
        timer = SearchTools.TryGetComponent<Image>(temp);
        var temp2 = SearchTools.TryFindInGameobject(powerTimer, "Text");
        timerText = SearchTools.TryGetComponent<TextMeshProUGUI>(temp2);

        // Disable timer
        timer.fillAmount = 1f;
        timerText.text = powerTime.ToString();
        powerTimer.SetActive(false);
    }

    void OnDestroy()
    {
        // reset all coroutines
        instance.StopAllCoroutines();
        StopAllCoroutines();
        timerCor = null;
    }

    public static void EnablePowerTimer()
    {
        if(timerCor == null)
        {
            timerCor = PowerTimer();
            instance.StartCoroutine(timerCor);
        }
    }

    private static IEnumerator PowerTimer()
    {
        float seconds = 0f;

        // Enable timer
        powerTimer.SetActive(true);

        // count each second of the timer
        for(int i = 0; i < powerTime; i++)
        {
            float delay = 0;
            while (delay < 1f)
            {
                yield return null;
                delay += Time.deltaTime;
            }
            seconds++;
            timer.fillAmount = 1f - (seconds / powerTime);
            timerText.text = (powerTime - seconds).ToString();
        }

        // Disable timer
        timer.fillAmount = 1f;
        timerText.text = powerTime.ToString();
        powerTimer.SetActive(false);

        // Restart coroutine
        timerCor = null;
    }

}