using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTools;
using TMPro;
using UnityEngine.UI;

public class PowerTimer : MonoBehaviour
{
    // Data
    [Tooltip("0 for size power, 1 for speed power")]
    [Range(0, 1)] [SerializeField] private int powerType;
    private int counter;
    private IEnumerator coroutine;

    // Components
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image regressionImage, powerImage;
    [SerializeField] private readonly Sprite[] powersImages = new Sprite[4];


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


    public void StartPowerTimer(Powers.PowerType powerType, Powers.Power newPower)
    {
        // restart time if catch the same active power, or replace it if its a new one
        if (coroutine != null)
        {
            if (Powers.currentSizePower == newPower)
            {
                ResetCounter();
                return;
            }
            else
            {
                StopCoroutine(coroutine);
                ResetPowerCoroutine(powerType);
            }
        }

        // Enable timer
        gameObject.SetActive(true);

        // Change the power image if needed
        if (powerType == Powers.PowerType.size)
        {
            if (newPower != Powers.currentSizePower)
                powerImage.sprite = powersImages[(int)newPower - 1];
        }
        else if (powerType == Powers.PowerType.speed)
        {
            if (newPower != Powers.currentSpeedPower)
                powerImage.sprite = powersImages[(int)newPower - 1];
        }

        // Start the counter
        coroutine = PowerTimerCor(powerType);
        StartCoroutine(coroutine);
    }

    private IEnumerator PowerTimerCor(Powers.PowerType powerType)
    {
        // count each second of the timer
        ResetCounter();
        while(counter < Powers.maxPowerTime)
        {
            float delay = 0;
            while (delay < 1f)
            {
                float delay2 = 0;
                while (delay2 < 0.1f)
                {
                    yield return null;
                    delay += Time.deltaTime;
                    delay2 += Time.deltaTime;
                }
            }
            counter++;
            regressionImage.fillAmount = 1f - (counter / Powers.maxPowerTime);
            timerText.text = (Powers.maxPowerTime - counter).ToString();
        }

        // Disable timer
        regressionImage.fillAmount = 1f;
        timerText.text = Powers.maxPowerTime.ToString();
        gameObject.SetActive(false);

        // Restart coroutine
        ResetPowerCoroutine(powerType);
    }

    private void ResetPowerCoroutine(Powers.PowerType powerType)
    {
        if (powerType == Powers.PowerType.size)
            Powers.currentSizePower = Powers.Power.none;
        else if (powerType == Powers.PowerType.speed)
            Powers.currentSpeedPower = Powers.Power.none;
        coroutine = null;
        counter = 0;
    }

    private void ResetCounter()
    {
        counter = 0;
        regressionImage.fillAmount = 1f;
        timerText.text = Powers.maxPowerTime.ToString();
    }

}