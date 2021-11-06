using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCapsules : MonoBehaviour
{
    [Tooltip("The correct strings for powers are: slow, fast, short and large")]
    [SerializeField] private string power;
    

    void OnDestroy()
    {
        if (LevelManager.powersSpawned > 0)
            LevelManager.powersSpawned--;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BottomBound"))
        {
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Player"))
        {
            switch (power)
            {
                case "small":
                    Powers.nextPower = Powers.Power.small;
                    Powers.newPowerType = Powers.PowerType.size;
                    collision.gameObject.GetComponent<Paddle>().GetPower(power);
                    break;

                case "large":
                    Powers.nextPower = Powers.Power.large;
                    Powers.newPowerType = Powers.PowerType.size;
                    collision.gameObject.GetComponent<Paddle>().GetPower(power);
                    break;

                case "slow":
                    Powers.nextPower = Powers.Power.slow;
                    Powers.newPowerType = Powers.PowerType.speed;
                    GameObject.Find(Ball.ballPath).GetComponent<Ball>().BallSpeedPower(power, Powers.nextPower);
                    break;

                case "fast":
                    Powers.nextPower = Powers.Power.fast;
                    Powers.newPowerType = Powers.PowerType.speed;
                    GameObject.Find(Ball.ballPath).GetComponent<Ball>().BallSpeedPower(power, Powers.nextPower);
                    break;

                default:
                    Debug.LogError("power not recognized");
                    return;
            }

            AudioManager.PlayAudio(LevelManager.powersAudioSource, LevelManager.getPowerAudio, false, 0.7f);
            if (Powers.newPowerType == Powers.PowerType.size)
                Powers.sizePowerTimer.StartPowerTimer(Powers.PowerType.size, Powers.nextPower);
            else if (Powers.newPowerType == Powers.PowerType.speed)
                Powers.speedPowerTimer.StartPowerTimer(Powers.PowerType.speed, Powers.nextPower);
            Destroy(gameObject);
        }
    }

}