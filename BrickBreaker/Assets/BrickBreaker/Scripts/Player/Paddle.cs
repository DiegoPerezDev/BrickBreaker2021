using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    // Movement
    [HideInInspector] public bool? moveDirR = null;
    [HideInInspector] public bool moveBool, canMoveR = true, canMoveL = true;
    private readonly float speed = 16f;
    private float lerp;

    // Paddle
    public static string paddlePath = "LevelDev/Paddle";
    private Rigidbody2D rb;
    [HideInInspector] public Vector2 paddleSize;

    // Screen
    private static float leftScreenLimit, rightScreenLimit;
    private static float camWidth;

    // Powers
    private IEnumerator powerCor;
    private Vector2 normalSize, shortSize, largeSize, normalScale, shortScale, largeScale;

    // Level management
    public static bool StartSet;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()    
    {
        // Paddle size data
#if UNITY_ANDROID
        transform.localScale = new Vector3(transform.localScale.x * 1.2f, transform.localScale.y, transform.localScale.z);
#endif
        paddleSize = GetComponent<SpriteRenderer>().bounds.size;
        float smallMult = 0.66f, largeMult = 1.33f;
        normalSize = paddleSize;
        shortSize = new Vector2(normalSize.x * smallMult, normalSize.y);
        largeSize = new Vector2 (normalSize.x * largeMult, normalSize.y);
        normalScale = transform.lossyScale;
        shortScale = new Vector2(normalScale.x * smallMult, normalScale.y);
        largeScale = new Vector2(normalScale.x * largeMult, normalScale.y);

        // Game management
        StartSet = true;
    }

    void OnDestroy()
    {
        StartSet = false;
        if (powerCor != null)
            StopCoroutine(powerCor);
        StopAllCoroutines();
    }

    void FixedUpdate()
    {
        if (moveBool)
            Move();
        else
            lerp = 0f;
    }


    #region Movement

    /// <summary>
    /// Wait for the bounds code to set values and then call this method so it can use their values.
    /// </summary>
    public static void GetScreenLimits()
    {
        Vector2 camPos = Camera.main.gameObject.transform.position;
        float camHeight = Camera.main.orthographicSize * 2f;
        camWidth = camHeight * Camera.main.aspect;
        float blackPanelWidth = HUD.blackBlockRtf.rect.width * HUD.blackBlockRtf.transform.lossyScale.x;
        leftScreenLimit = camPos.x - (camWidth / 2) + blackPanelWidth;
        rightScreenLimit = camPos.x + (camWidth / 2) - blackPanelWidth;
    }

    /// <summary>
    /// This movement has lerp. Move the paddle until it gets to the borders.
    /// </summary>
    private void Move()
    {
        var increase = new Vector2(Mathf.Lerp(0, speed, lerp), 0f) * Time.fixedDeltaTime;
        bool moved = false;

        if (moveDirR == true)
        {
            Vector2 nextPos = rb.position + increase;
            var rightBorderNextPos = nextPos.x + (paddleSize.x / 2);
            if (rightBorderNextPos <= rightScreenLimit)
            {
                // Move if you would not surpass the limit of the movement in that next move.
                rb.MovePosition(nextPos);
                moved = true;
            }
            else
            {
                // If next position would make you go out of the border, then take you exactly there, if you are not currently there.
                Vector2 rightPaddleLimit = new Vector2(rightScreenLimit - (paddleSize.x / 2), rb.position.y);
                if(rb.position.x < rightPaddleLimit.x)
                    rb.position = rightPaddleLimit;
            }
        }
        else if (moveDirR == false)
        {
            Vector2 nextPos = rb.position - increase;
            var leftBorderNextPos = nextPos.x - (paddleSize.x / 2);
            if (leftBorderNextPos >= leftScreenLimit)
            {
                // Move if you would not surpass the limit of the movement in that next move.
                rb.MovePosition(nextPos);
                moved = true;
            }
            else
            {
                // If next position would make you go out of the border, then take you exactly there, if you are not currently there.
                Vector2 leftPaddleLimit = new Vector2(leftScreenLimit + (paddleSize.x / 2), rb.position.y);
                if (rb.position.x > leftPaddleLimit.x)
                    rb.position = leftPaddleLimit;
            }
        }
        else
            return;

        // Increase lerp of the movement only when we move
        if (moved)
        {
#if UNITY_STANDALONE
if (lerp < 1)
                lerp += Time.fixedDeltaTime * 5;
#endif
#if UNITY_ANDROID
if (lerp < 1)
                lerp += Time.fixedDeltaTime * 8.5f;
#endif
        }
    }

#endregion

#region powers

    /// <summary> Paddle acquire a size power. </summary>
    /// <param name="power"> strings for powers are: slow, fast, short and large </param>
    public void GetPower(PowersSystem.Power power)
    {
        // Remove the previous power if there is one.
        if(powerCor != null)
        {
            StopCoroutine(powerCor);
            powerCor = null;
        }

        // Start power
        switch (power)
        {
            case PowersSystem.Power.small:
                transform.localScale = shortScale;
                paddleSize = shortSize;
                //print("small");
                break;

            case PowersSystem.Power.large:
                transform.localScale = largeScale;
                paddleSize = largeSize;
                CheckIfPaddleOutside();
                //print("large");
                break;

            default:
                print("power name not recognized in the paddle code.");
                powerCor = null;
                return;
        }

        powerCor = PowerDelay(power);
        StartCoroutine(powerCor);
    }

    private IEnumerator PowerDelay(PowersSystem.Power power)
    {
        // Delay
        float delay = 0;
        while(delay < PowersSystem.maxPowerTime)
        {
            float delay2 = 0;
            while (delay2 < 0.05f)
            {
                yield return null;
                delay += Time.deltaTime;
                delay2 += Time.deltaTime;
            }
        }

        // Stop power
        StopPower();
    }

    public void StopPower()
    {
        if (powerCor != null)
        {
            StopCoroutine(powerCor);
            powerCor = null;
        }
        PowersSystem.currentSizePower = PowersSystem.Power.none;

        transform.localScale = normalScale;
        paddleSize = normalSize;
        CheckIfPaddleOutside();
    }

    /// <summary>
    /// Check if the paddle is outside of the playing view when changing to a bigger side.
    /// </summary>
    private void CheckIfPaddleOutside()
    {
        Vector2 leftPaddleLimit = new Vector2(leftScreenLimit + (paddleSize.x / 2), rb.position.y);
        if (rb.position.x < leftPaddleLimit.x)
            rb.position = leftPaddleLimit;
        else
        {
            Vector2 rightPaddleLimit = new Vector2(rightScreenLimit - (paddleSize.x / 2), rb.position.y);
            if (rb.position.x > rightPaddleLimit.x)
                rb.position = rightPaddleLimit;
        }
    }

#endregion

}