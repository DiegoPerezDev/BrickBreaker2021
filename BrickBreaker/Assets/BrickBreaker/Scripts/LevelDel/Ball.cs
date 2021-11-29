using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    // Ball general data
    public static string ballPath = "LevelDev/Ball";
    public static bool ballReleased;
    private Vector2 ballSize;
    [SerializeField] private bool testingVertical, testingHorizontal;

    // Ball speed changing
    private float speed, startSpeed, maxSpeed, normalSpeed;
    private readonly float speedInc = 0.2f;
    private IEnumerator ballSpeedCor;
    private bool maxSpeedReached, usingSpeedPower;

    // Components
    private Rigidbody2D rb;
    private TrailRenderer trail;
    private Paddle paddleCode;
    private Rigidbody2D paddleRb;
    private AudioSource audioSource;
    private AudioClip ballHitAudio, ballReleaseAudio;
    
    // Unstuck process
    public static bool unstuckBallTrigger, hitObject;
    public static bool stuck;
    private IEnumerator stuckCor;

    // Game Management
    public static bool StartSet;


    void Awake()
    {
        // Audiovisuals
        ballHitAudio = Resources.Load<AudioClip>("Audio/Level objects/(lo1) ballHit");
        ballReleaseAudio = Resources.Load<AudioClip>("Audio/Level objects/(lo1) releaseBall");
        audioSource = GetComponent<AudioSource>();
        trail = GetComponent<TrailRenderer>();

        // Ball
        rb = GetComponent<Rigidbody2D>();
        float ballRad = GetComponent<CircleCollider2D>().radius;
        ballSize = ballRad * transform.localScale;
    }

    void Start()
    {
        ballReleased = false;
        trail.enabled = false;
        stuck = false;

        // For unstuck
        stuckCor = CheckStuck();
        StartCoroutine(stuckCor);

        // Set speed
        int level = SceneManager.GetActiveScene().buildIndex - 1;
        if (level >= 5)
            level = 5;
        startSpeed = 4.75f + (level * 0.25f);
        normalSpeed = startSpeed;
        maxSpeed = 2.5f + startSpeed;
        speed = startSpeed;

        // Final setting
        StartCoroutine(SetBall());
    }

    void Update()
    {
        if (unstuckBallTrigger)
        {
            unstuckBallTrigger = false;
            UnstuckBall();
        }

        //if (speed < startSpeed)
        //    print("slow ball");
        //else if (speed > maxSpeed)
        //    print("fast ball");
        //else
        //    print("normal ball");
    }

    void FixedUpdate()
    {
        if (!StartSet)
            return;

        if (!ballReleased)
            FollowPaddle();
    }

    void OnDestroy()
    {
        // Reset some data
        StartSet = false;
        ballReleased = false;
        stuck = false;
        PowersSystem.currentSpeedPower = PowersSystem.Power.none;

        // Stop coroutines
        if (stuckCor != null)
            StopCoroutine(stuckCor);
        if (ballSpeedCor != null)
            StopCoroutine(ballSpeedCor);
        StopAllCoroutines();
    }


    #region Start functions

    /// <summary>
    /// Waits for te paddle code to be done and then places the ball over the paddle and finally tells the levelManager that the ball is set.
    /// </summary>
    private IEnumerator SetBall()
    {
        // Get paddle components
        while (!Paddle.StartSet)
            yield return null;
        GameObject paddle = GameObject.Find(Paddle.paddlePath);
        paddleRb = paddle.GetComponent<Rigidbody2D>();
        paddleCode = paddle.GetComponent<Paddle>();

        // Place ball at the start
        if (testingVertical)
        {
            rb.velocity = Vector2.up * maxSpeed;
            ballSpeedCor = SpeedIncrese();
            StartCoroutine(ballSpeedCor);
            ballReleased = true;
        }
        else if (testingHorizontal)
        {
            rb.velocity = Vector2.right * maxSpeed;
            ballSpeedCor = SpeedIncrese();
            StartCoroutine(ballSpeedCor);
            ballReleased = true;
        }
        else
        {
            ballReleased = false;
            PlaceBallAtPaddle();
        }

        StartSet = true;
    }

    private void PlaceBallAtPaddle()
    {
        Vector3 paddlePos = paddleRb.transform.position;
        transform.position = paddlePos + (Vector3.up * (paddleCode.paddleSize.y / 2 + ballSize.y / 2 + 0.1f));
    }

    #endregion

    #region Collision functions

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Evade colliding when we have not released the ball
        if (!ballReleased)
            return;

        // Ball outside of the screen
        if (collision.gameObject.CompareTag("BottomBound"))
        {
            trail.Clear();
            trail.enabled = false;
            // Prevent multiple losing or losing when setting the game
            if (PlayerLife.lives > 0)
                LevelManager.LoseLive();
            return;
        }

        AudioManager.PlayAudio(audioSource, ballHitAudio, false, 0.5f);

        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Break();

            // For unstuck purposes
            hitObject = true;

            // Get the reflect direction of the ball in the paddle, its different because it may change the ball dir in the x axis
            if (VelAfterPaddleColl(collision.GetContact(0)))
                return;
        }
        else if (collision.gameObject.CompareTag("Brick"))
        {
            // For unstuck purposes
            hitObject = true;

            // Count the remaining bricks for knowing when to win
            collision.gameObject.GetComponent<Bricks>().GotHit();
        }
        else if(collision.gameObject.CompareTag("MetalBrick"))
        {
            collision.gameObject.GetComponent<Bricks>().GotHit();
        }
    }

    private bool VelAfterPaddleColl(ContactPoint2D contact)
    {
        Vector2 finalDir, inNormal;
        Vector2 contactPoint = contact.point;

        // Check if the ball is hitting the paddle upwards or downwards for setting the normal manually, evading weird reflections in the borders.
        Vector2 paddlePos = paddleRb.position;
        if (contactPoint.y > paddlePos.y)
            inNormal = Vector2.up;
        else
            inNormal = Vector2.down;

        // Change the horizontal direction of the ball if the collision was with the paddle sides
        float paddleSizeX = paddleCode.paddleSize.x;
        if ((contactPoint.x < paddlePos.x - paddleSizeX / 14) || (contactPoint.x > paddlePos.x + paddleSizeX / 14))
        {
            // set new vel based in the hypotenuse formula, knowing that we dont want the ball to go further than 45° relative to the paddle (0.707)
            var xVel = MapValue(contactPoint.x, paddlePos.x - paddleSizeX / 2f, paddlePos.x + paddleSizeX / 2f, -0.707f, 0.707f);
            var yVel = Mathf.Sqrt((Mathf.Pow(xVel, 2) + 1)) * inNormal.y;

            // Check for obstacles so we can change the direction of the ball so it wont get blocked with the level bounds
            if ((xVel > 0) && (DetectObstacles()[1]))
                xVel = -xVel;
            else if ((xVel < 0) && (DetectObstacles()[0]))
                xVel = -xVel;

            finalDir = Vector3.Normalize(new Vector2(xVel, yVel));
            rb.velocity = finalDir * speed;
            return true;
        }
        else
            return false;
    }

    /// <summary>
    /// Detect if there are obstacles near the ball in the four directions.
    /// </summary>
    /// <returns>Returns a bool array of size 4. Each one means a directin: 0 for left, 1 for right, 2 for up, 3 for down.</returns>
    private bool[] DetectObstacles()
    {
        // each slot means a near object from a direction: 1 left, 2 right, 3 up, 4 down
        bool[] answer = new bool[4];

        float rayDistance = ballSize.x/2 + 0.25f;
        Vector2 pos = transform.position;
        //int layerMask = 1 << 6;

        // Left detection
        if (Physics2D.Raycast(pos, -Vector2.right, rayDistance))
            answer[0] = true;
        // Right detection
        if (Physics2D.Raycast(pos, Vector2.right, rayDistance))
            answer[1] = true;
        // Up detection
        if(Physics2D.Raycast(pos, Vector2.up, rayDistance))
            answer[2] = true;
        // Down detection
        if(Physics2D.Raycast(pos, -Vector2.up, rayDistance))
            answer[3] = true;

        return answer; 
    }

    private float MapValue(float value, float min1, float max1, float min2, float max2)
    {
        return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
    }

    #endregion

    #region Ball normal functions

    public void RestartBall()
    {
        // Stop the ball
        rb.velocity = Vector2.zero;

        // Stop coroutines
        if(ballSpeedCor != null)
            StopCoroutine(ballSpeedCor);
        if (stuckCor != null)
            StopCoroutine(stuckCor);
        StopAllCoroutines();

        // Restart behaviour
        Start();
    }

    private void FollowPaddle() => rb.position = new Vector2(paddleRb.position.x, rb.position.y);

    public void ReleaseBall()
    {
        // Audiovisuals setting
        trail.enabled = true;
        AudioManager.PlayAudio(audioSource, ballReleaseAudio, false, 0.9f);

        // Release ball
        rb.velocity = new Vector2(0f, speed = startSpeed);
        ballSpeedCor = SpeedIncrese();
        StartCoroutine(ballSpeedCor);
    }

    private IEnumerator SpeedIncrese()
    {
        // Avoid using this coroutine by mistake after reaching max speed
        if (!ballReleased || maxSpeedReached)
        {
            yield return null;
            yield break;
        }

        // Delay
        float delay = 0;
        while (delay < 1f)
        {
            //print("Increasing ball speed.");
            float delay2 = 0;
            while (delay2 < 0.05f)
            {
                yield return null;
                delay += Time.deltaTime;
                delay2 += Time.deltaTime;
            }
        }

        //Set the speed in the ball
        speed += speedInc;
        var vel = rb.velocity;
        
        // Limit the speed increase
        if (speed + speedInc >= maxSpeed)
        {
            //print("maxSpeed reached");
            speed = maxSpeed;
            maxSpeedReached = true;
            rb.velocity = Vector3.Normalize(vel) * maxSpeed;
        }
        else
        {
            if (vel.magnitude > 0)
                rb.velocity = Vector3.Normalize(vel) * speed;
            RestartSpeedIncreseCor();
        }
    }

    private void RestartSpeedIncreseCor()
    {
        if (!usingSpeedPower)
        {
            if (ballSpeedCor != null)
            {
                StopCoroutine(ballSpeedCor);
                ballSpeedCor = null;
            }
            ballSpeedCor = SpeedIncrese();
            StartCoroutine(ballSpeedCor);
        }
    }

    #endregion

    #region Unstuck functions

    /// <summary>
    /// Check if the ball has been a lot of time moving without losing or hitting the paddle.
    /// </summary>
    private IEnumerator CheckStuck()
    {
        // delay
        float delay = 0f;
        while (delay < 0.05f)
        {
            yield return null;
            delay += Time.deltaTime;
        }

        // Dont check stuck if we haven't released the ball
        if(!ballReleased)
            goto RepeatCor;

        if (!stuck)
        {
            // Check if we have hit something important in the past seconds
            for (int i = 0; i < 4; i++)
            {
                // delay
                delay = 0f;
                while (delay < 1f)
                {
                    float delay2 = 0f;
                    while (delay2 < 0.1f)
                    {
                        yield return null;
                        delay += Time.deltaTime;
                        delay2 += Time.deltaTime;
                    }
                }

                if (hitObject)
                {
                    hitObject = false;
                    Gameplay_UI.unstuckButton.interactable = false;
                    goto RepeatCor;
                }
            }

            // If we are stuck for a few time
            stuck = true;
            Gameplay_UI.unstuckButton.interactable = true;
        }
        else
        {
            // Unstuck by button or if we hit an object
            if (hitObject)
            {
                hitObject = stuck = false;
                Gameplay_UI.unstuckButton.interactable = false;
            }
        }

    // If we are not stuck
    RepeatCor:
        RepeatStuckCheck();
        
    }

    private void RepeatStuckCheck()
    {
        if (stuckCor != null)
        {
            StopCoroutine(stuckCor);
            stuckCor = null;
        }
        stuckCor = CheckStuck();
        StartCoroutine(stuckCor);
    }

    private void UnstuckBall()
    {
        AudioManager.PlayAudio(audioSource, ballReleaseAudio, false, 0.8f);

        // Redirect the ball
        float speedLeg = speed / Mathf.Sqrt(2);
        float horSpeed = speedLeg, verSpeed = speedLeg;
        if (DetectObstacles()[1])
            horSpeed = -horSpeed;
        if (DetectObstacles()[2])
            verSpeed = -verSpeed;
        rb.velocity = new Vector2(horSpeed, verSpeed);

        // Close the unstuck button
        if(Gameplay_UI.unstuckButton != null)
            Gameplay_UI.unstuckButton.interactable = false;
    }

    #endregion

    #region powers

    public void BallSpeedPower(PowersSystem.Power power)
    {
        if (ballSpeedCor != null)
        {
            StopCoroutine(ballSpeedCor);
            ballSpeedCor = null;
        }

        SetBallPowerSpeed(power);
        ballSpeedCor = BallPowerDelay();
        StartCoroutine(ballSpeedCor);
    }

    private void SetBallPowerSpeed(PowersSystem.Power power)
    {
        var vel = rb.velocity;
        switch (power)
        {
            case PowersSystem.Power.slow:
                if (!usingSpeedPower && (speed >= startSpeed) && (speed <= maxSpeed))
                    normalSpeed = speed;
                speed = startSpeed * 0.7f;
                rb.velocity = Vector3.Normalize(vel) * speed;
                break;

            case PowersSystem.Power.fast:
                if (!usingSpeedPower && (speed >= startSpeed) && (speed <= maxSpeed))
                    normalSpeed = speed;
                speed = maxSpeed * 1.5f;
                rb.velocity = Vector3.Normalize(vel) * speed;
                break;
        }

        usingSpeedPower = true;
    }

    private IEnumerator BallPowerDelay()
    {
        // Delay
        for (int i = 0; i < PowersSystem.maxPowerTime; i++)
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
        }

        // Stop power
        StopPower();
    }

    public void StopPower()
    {
        usingSpeedPower = false;
        PowersSystem.currentSpeedPower = PowersSystem.Power.none;

        if (ballSpeedCor != null)
        {
            StopCoroutine(ballSpeedCor);
            ballSpeedCor = null;
        }

        ballSpeedCor = SpeedIncrese();
        StartCoroutine(ballSpeedCor);
        speed = normalSpeed;
        rb.velocity = Vector3.Normalize(rb.velocity) * speed;
        //print("speed power ended.");
    }

    #endregion

}