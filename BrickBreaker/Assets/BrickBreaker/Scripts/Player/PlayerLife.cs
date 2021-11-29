using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    public static int lives;
    public static readonly int liveCap = 3;
    public static AudioClip loseLifeAudio;

    void Awake()
    {
        loseLifeAudio = Resources.Load<AudioClip>("Audio/Level general/(gs1) losing life");
    }

    void Start()
    {
        lives = liveCap;
    }

}