using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PowerTimer : MonoBehaviour
{
    // Data
    private int counter;
    private IEnumerator coroutine;

    // Components
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image regressionImage, powerImage;
    private readonly Sprite[] powersImages = new Sprite[4];


    void Awake()
    {
        // Get images
        powersImages[0] = Resources.Load<Sprite>("Art2D/Powers/46-Breakout-Tiles");
        powersImages[1] = Resources.Load<Sprite>("Art2D/Powers/47-Breakout-Tiles");
        powersImages[2] = Resources.Load<Sprite>("Art2D/Powers/41-Breakout-Tiles");
        powersImages[3] = Resources.Load<Sprite>("Art2D/Powers/42-Breakout-Tiles");
    }

    void Start()
    {
        gameObject.SetActive(false);
    }

    void OnDestroy()
    {
        // reset all coroutines
        StopAllCoroutines();
    }


    public void StartPowerTimer(PowersSystem.PowerType powerType, PowersSystem.Power newPower)
    {
        // restart time if paddle catches the same active power, or replace it if its a new one
        if (coroutine != null)
        {
            switch(powerType)
            {
                case PowersSystem.PowerType.size:
                    if (PowersSystem.currentSizePower == newPower)
                    {
                        ResetCounter();
                        return;
                    }
                    break;

                case PowersSystem.PowerType.speed:
                    if (PowersSystem.currentSpeedPower == newPower)
                    {
                        ResetCounter();
                        return;
                    }
                    break;
            }

            if(coroutine != null)
                StopCoroutine(coroutine);
            ResetPowerCoroutine(powerType);
        }

        // Enable timer
        gameObject.SetActive(true);

        // Change the power image if needed
        switch (powerType)
        {
            case PowersSystem.PowerType.size:
                if (newPower != PowersSystem.previousSizePower)
                    powerImage.sprite = powersImages[(int)newPower - 1];
                break;

            case PowersSystem.PowerType.speed:
                if (newPower != PowersSystem.previousSpeedPower)
                    powerImage.sprite = powersImages[(int)newPower - 1];
                break;
        }

        // Start the counter
        coroutine = PowerTimerCor(powerType);
        StartCoroutine(coroutine);
    }

    private IEnumerator PowerTimerCor(PowersSystem.PowerType powerType)
    {
        // count each second of the timer
        ResetCounter();
        while(counter < PowersSystem.maxPowerTime)
        {
            float delay = 0;
            while (delay < 1f)
            {
                float delay2 = 0;
                while (delay2 < 0.05f)
                {
                    yield return null;
                    delay += Time.deltaTime;
                    delay2 += Time.deltaTime;
                }
            }
            counter++;
            regressionImage.fillAmount = 1f - (counter / PowersSystem.maxPowerTime);
            timerText.text = (PowersSystem.maxPowerTime - counter).ToString();
        }

        // Disable timer
        regressionImage.fillAmount = 1f;
        timerText.text = PowersSystem.maxPowerTime.ToString();
        gameObject.SetActive(false);

        // Restart coroutine
        ResetPowerCoroutine(powerType);
    }

    private void ResetPowerCoroutine(PowersSystem.PowerType powerType)
    {
        if (powerType == PowersSystem.PowerType.size)
            PowersSystem.currentSizePower = PowersSystem.Power.none;
        else if (powerType == PowersSystem.PowerType.speed)
            PowersSystem.currentSpeedPower = PowersSystem.Power.none;
        coroutine = null;
        counter = 0;
    }

    private void ResetCounter()
    {
        counter = 0;
        regressionImage.fillAmount = 1f;
        timerText.text = PowersSystem.maxPowerTime.ToString();
    }

}