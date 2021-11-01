using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTools;

public class Paddle : MonoBehaviour
{
    // Movement
    [HideInInspector] public bool? moveDirR = null;
    [HideInInspector] public bool moveBool, canMoveR, canMoveL;
    private readonly float speed = 15f;
    private float lerp;

    // Paddle data
    public static string paddleName = "Paddle", paddlePath = "LevelDev/Paddle";
    private Rigidbody2D rb;
    [HideInInspector] public Vector2 paddleSize;
    private Vector2 collSize;

    // Screen
    private float leftScreenLimit, rightScreenLimit;
    private float camWidth;

    // Powers
    private IEnumerator usingPower;
    private Vector2 normalSize, shortSize, largeSize;

    // Level management
    public static bool StartSet;


    void Awake()
    {
        // Get camera values for the screen boundaries, for limitting the paddle movement
        GetScreenValues();

        // Paddle data
        rb = GetComponent<Rigidbody2D>();
        collSize = GetComponent<CapsuleCollider2D>().size;
    }

    void Start()    
    {
        // Paddle data
        paddleSize = collSize * transform.localScale; // the localscale is not the entire size because it is an sprite.
        normalSize = paddleSize;
        shortSize = normalSize * 0.7f;
        largeSize = new Vector2 (normalSize.x * 1.35f, normalSize.y);
        canMoveR = canMoveL = true;

        // Game management
        StartSet = true;
    }

    void OnDestroy()
    {
        StartSet = false;
    }

    void FixedUpdate()
    {
        if (moveBool)
        {
            if (lerp < 1)
                lerp += Time.fixedDeltaTime * 5;
            Move();
        }
        else
            lerp = 0f;
    }


    private void GetScreenValues()
    {
        Vector2 camPos = Camera.main.gameObject.transform.position;
        float camHeight = Camera.main.orthographicSize * 2f;
        camWidth = camHeight * Camera.main.aspect;
        leftScreenLimit = camPos.x - (camWidth / 2);
        rightScreenLimit = camPos.x + (camWidth / 2);
    }

    private void Move()
    {
        //  Check if the paddle is near the screen limits so it wont go outside of the screen.
        CheckLevelBounds();

        
        var increase = new Vector2(Mathf.Lerp(0, speed, lerp), 0f) * Time.fixedDeltaTime;

        if (moveDirR == true)
        {
            if(canMoveR)
                rb.MovePosition(rb.position + increase);
        }
        else if (moveDirR == false)
        {
            if (canMoveL)
                rb.MovePosition(rb.position - increase);
        }
    }

    private void CheckLevelBounds()
    {
        if ((rb.position.x - (paddleSize.x / 2)) <= leftScreenLimit)
        {
            canMoveR = true;
            canMoveL = false;
        }
        else if ((rb.position.x + (paddleSize.x / 2)) >= rightScreenLimit)
        {
            canMoveR = false;
            canMoveL = true;
        }
        else if (!canMoveR || !canMoveL)
        {
            canMoveR = canMoveL = true;
        }
    }

    /// <param name="power"> strings for powers are: slow, fast, short and large </param>
    public void GetPower(string power)
    {
        if(usingPower != null)
        {
            StopCoroutine(usingPower);
            StopPower();
        }
        usingPower = Power(power);
        StartCoroutine(usingPower);
    }

    private IEnumerator Power(string power)
    {
        // Start power
        switch (power)
        {
            case "short":
                transform.localScale = shortSize / collSize;
                paddleSize = shortSize;
                //print("short");
                break;

            case "large":
                transform.localScale = largeSize / collSize;
                paddleSize = largeSize;
                //print("large");
                break;
        }

        // Delay
        float delay = 0;
        while(delay < Powers.powerTime)
        {
            yield return null;
            delay += Time.deltaTime;
        }

        // Stop power
        StopPower();
        usingPower = null;
    }

    private void StopPower()
    {
        transform.localScale = normalSize / collSize;
        paddleSize = normalSize;
    }

}