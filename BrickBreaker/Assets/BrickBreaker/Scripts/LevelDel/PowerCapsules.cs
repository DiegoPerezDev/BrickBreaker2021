using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerCapsules : MonoBehaviour
{
    [Tooltip("The correct strings for powers are: slow, fast, small and large")]
    [SerializeField] private string power;
    private PowersSystem.Power capsulePower;


    void Start()
    {
        switch(power)
        {
            case "slow":
                capsulePower = PowersSystem.Power.slow;
                break;

            case "fast":
                capsulePower = PowersSystem.Power.fast;
                break;

            case "small":
                capsulePower = PowersSystem.Power.small;
                break;

            case "large":
                capsulePower = PowersSystem.Power.large;
                break;

            default:
                print("Power name not recognized by a power capsule.");
                Debug.Break();
                break;
        }
    }

    void OnDestroy()
    {
        if (PowersSystem.powersSpawned > 0)
            PowersSystem.powersSpawned--;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BottomBound"))
        {
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Player"))
        {
            switch (capsulePower)
            {
                case PowersSystem.Power.small:
                    PowersSystem.previousSizePower = PowersSystem.currentSizePower;
                    PowersSystem.currentSizePower = capsulePower;
                    PowersSystem.sizePowerTimer.StartPowerTimer(PowersSystem.PowerType.size, PowersSystem.currentSizePower);
                    collision.gameObject.GetComponent<Paddle>().GetPower(capsulePower);
                    break;

                case PowersSystem.Power.large:
                    PowersSystem.previousSizePower = PowersSystem.currentSizePower;
                    PowersSystem.currentSizePower = capsulePower;
                    PowersSystem.sizePowerTimer.StartPowerTimer(PowersSystem.PowerType.size, PowersSystem.currentSizePower);
                    collision.gameObject.GetComponent<Paddle>().GetPower(capsulePower);
                    break;

                case PowersSystem.Power.slow:
                    PowersSystem.previousSpeedPower = PowersSystem.currentSpeedPower;
                    PowersSystem.currentSpeedPower = capsulePower;
                    PowersSystem.speedPowerTimer.StartPowerTimer(PowersSystem.PowerType.speed, PowersSystem.currentSpeedPower);
                    GameObject.Find(Ball.ballPath).GetComponent<Ball>().BallSpeedPower(capsulePower);
                    break;

                case PowersSystem.Power.fast:
                    PowersSystem.previousSpeedPower = PowersSystem.currentSpeedPower;
                    PowersSystem.currentSpeedPower = capsulePower;
                    PowersSystem.speedPowerTimer.StartPowerTimer(PowersSystem.PowerType.speed, PowersSystem.currentSpeedPower);
                    GameObject.Find(Ball.ballPath).GetComponent<Ball>().BallSpeedPower(capsulePower);
                    break;

                default:
                    Debug.LogError("power not recognized");
                    return;
            }

            AudioManager.PlayAudio(PowersSystem.powersAudioSource, PowersSystem.getPowerAudio, false, 0.7f);
            Destroy(gameObject);
        }
    }

}