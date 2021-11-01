using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power : MonoBehaviour
{
    [Tooltip("The correct strings for powers are: slow, fast, short and large")]
    [SerializeField] private string power;
    


    void OnDestroy()
    {
        if (Powers.powersSpawned > 0)
            Powers.powersSpawned--;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BottomBound"))
        {
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Player"))
        {
            power = power.Trim();
            power = power.ToLower();

            switch (power)
            {
                case "short":
                case "large":
                    collision.gameObject.GetComponent<Paddle>().GetPower(power);
                    break;

                case "slow":
                case "fast":
                    GameObject.Find(Ball.ballPath).GetComponent<Ball>().BallSpeedPower(power);
                    break;

                default:
                    Debug.LogError("power not recognized");
                    return;
            }

            AudioManager.PlayAudio(LevelManager.powersAudioSource, LevelManager.getPowerAudio, false, 0.7f);
            Powers.EnablePowerTimer();
            Destroy(gameObject);
        }
    }
}
