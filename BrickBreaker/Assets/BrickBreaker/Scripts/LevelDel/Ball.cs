using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MyTools;

public class Ball : MonoBehaviour
{
    // Ball general data
    public static string ballName = "Ball", ballPath = "LevelDev/Ball";
    private Rigidbody2D rb;  
    public static bool ballReleased;
    private TrailRenderer trail;
    private Vector2 ballSize;
    [SerializeField] private bool testingVertical, testingHorizontal;

    // Ball speed changing
    private float speed, startSpeed, maxSpeed, currentNormalSpeed;
    private readonly float speedInc = 0.2f;
    private IEnumerator ballSpeedCor;
    private bool usingSpeedPower, maxSpeedReached;

    // Paddle data
    private Paddle paddleCode;
    private Rigidbody2D paddleRb;

    // Audiovisuals
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
        // For unstuck
        stuckCor = CheckStuck();
        StartCoroutine(stuckCor);

        // Audiovisuals
        ballHitAudio = Resources.Load<AudioClip>("Audio/Level objects/(lo1) ballHit");
        ballReleaseAudio = Resources.Load<AudioClip>("Audio/Level objects/(lo1) releaseBall");
        audioSource = GetComponent<AudioSource>();
        trail = GetComponent<TrailRenderer>();

        // Ball
        trail.enabled = false;
        rb = GetComponent<Rigidbody2D>();
        float ballRad = GetComponent<CircleCollider2D>().radius;
        ballSize = ballRad * transform.localScale;
        StartCoroutine(SetBall());
    }

    void Start()
    {
        // Speed
        int level = SceneManager.GetActiveScene().buildIndex - 1;
        if (level >= 6)
            level = 6;
        startSpeed = 4.5f + level * 0.25f;
        maxSpeed = 2.5f + startSpeed;
        speed = startSpeed;
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
        StopCoroutine(stuckCor);
        StopAllCoroutines();
        StartSet = false;
        ballReleased = false;
    }


    #region Start functions

    /// <summary>
    /// Waits for te paddle code to be done and then places the ball over the paddle and finally tells the levelManager that the ball is set.
    /// </summary>
    private IEnumerator SetBall()
    {
        //Wait for paddle to be set because we are going to use its size that is set in the start function.
        while (!Paddle.StartSet)
        {
            yield return new WaitForSecondsRealtime(0.1f);
        }

        // Get paddle components
        GameObject paddle = GameObject.Find(Paddle.paddlePath);
        paddleRb = paddle.GetComponent<Rigidbody2D>();
        paddleCode = paddle.GetComponent<Paddle>();

        if(testingVertical)
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

    private void FollowPaddle() => rb.position = new Vector2(paddleRb.position.x, rb.position.y);

    public void ReleaseBall()
    {
        trail.enabled = true;
        rb.velocity = new Vector2(0f, speed = startSpeed);
        if(ballSpeedCor != null)
            StopCoroutine(ballSpeedCor);
        ballSpeedCor = SpeedIncrese();
        StartCoroutine(ballSpeedCor);
        AudioManager.PlayAudio(audioSource, ballReleaseAudio, false, 0.9f);
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
            BricksSystem.CheckNumberOfBricks();
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

    #region Ball settings functions

    public void RestartBall()
    {
        if(ballSpeedCor != null)
            StopCoroutine(ballSpeedCor);
        StopAllCoroutines();
        ballReleased = false;
        rb.velocity = Vector3.zero;
        PlaceBallAtPaddle();
    }

    private IEnumerator SpeedIncrese()
    {
        // Avoid using this coroutine by mistake after reaching max speed
        if (maxSpeedReached)
            yield break;

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
        if (vel.magnitude > 0)
            rb.velocity = Vector3.Normalize(vel) * speed;

        // Limit the speed increase
        if (speed + speedInc >= maxSpeed)
        {
            print("maxSpeed reached");
            maxSpeedReached = true;
            rb.velocity = Vector3.Normalize(vel) * maxSpeed;
        }
        else
            RestartSpeedIncreseCor();
    }

    private void RestartSpeedIncreseCor()
    {
        if(ballSpeedCor != null)
            StopCoroutine(ballSpeedCor);
        if (!usingSpeedPower)
        {
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
            for (int i = 0; i < 3; i++)
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
                    if(GameplayMenu.unstuckButton.activeInHierarchy)
                        GameplayMenu.unstuckButton.SetActive(false);
                    goto RepeatCor;
                }
            }

            // If we are stuck for a few time
            stuck = true;
            GameplayMenu.unstuckButton.SetActive(true);
        }
        else
        {
            // Unstuck by button or if we hit an object
            if (hitObject)
            {
                hitObject = stuck = false;
                GameplayMenu.unstuckButton.SetActive(false);
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
        // Redirect the ball vertically if it had a non stop horizontal movement for five seconds
        float speedX = -speed;
        if (rb.velocity.x > 0)
            speedX = speed;

        if(DetectObstacles()[2])
            rb.velocity = new Vector2(speedX / 2f, -speed / 2f);
        else
            rb.velocity = new Vector2(speedX / 2f, speed / 2f);

        AudioManager.PlayAudio(audioSource, ballReleaseAudio, false, 0.8f);

        // Close the unstuck button
        GameplayMenu.unstuckButton.SetActive(false);
    }

    #endregion

    #region powers

    public void BallSpeedPower(PowersSystem.Power power)
    {
        if(ballSpeedCor != null)
            StopCoroutine(ballSpeedCor);
        if (usingSpeedPower)
            StopSpeedPower();
        ballSpeedCor = BallPower(power);
        StartCoroutine(ballSpeedCor);
    }

    private IEnumerator BallPower(PowersSystem.Power power)
    {
        var vel = rb.velocity;
        switch (power)
        {
            case PowersSystem.Power.slow:
                if (!usingSpeedPower)
                    currentNormalSpeed = speed;
                speed = startSpeed * 0.7f;
                rb.velocity = Vector3.Normalize(vel) * speed;
                break;

            case PowersSystem.Power.fast:
                if (!usingSpeedPower)
                    currentNormalSpeed = speed;
                speed = maxSpeed * 1.4f;
                rb.velocity = Vector3.Normalize(vel) * speed;
                break;
        }

        usingSpeedPower = true;

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
        StopSpeedPower();
    }

    private void StopSpeedPower()
    {
        usingSpeedPower = false;
        PowersSystem.currentSpeedPower = PowersSystem.Power.none;

        speed = currentNormalSpeed;
        if (!maxSpeedReached)
        {
            ballSpeedCor = SpeedIncrese();
            rb.velocity = Vector3.Normalize(rb.velocity) * speed;
            StartCoroutine(ballSpeedCor);
        }
        else
        {
            ballSpeedCor = null;
            rb.velocity = Vector3.Normalize(rb.velocity) * maxSpeed;
        }
        //print("speed power ended.");
    }

    #endregion

}