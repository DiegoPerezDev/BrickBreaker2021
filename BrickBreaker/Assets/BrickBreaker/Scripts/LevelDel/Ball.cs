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
    private float speed, startSpeed, maxSpeed, previousSpeed;
    private readonly float speedInc = 0.2f;
    private Vector2 ballVel;
    private bool changeVel;
    private Vector2 nextVel;
    private IEnumerator usingPower;

    // Paddle data
    private Paddle paddleCode;
    private Rigidbody2D paddleRb;

    // Audiovisuals
    private AudioSource audioSource;
    private AudioClip ballHitAudio, ballReleaseAudio;
    
    // Unstuck process
    public static bool unstuckBallTrigger;
    private GameObject unstuckButton;

    // Game Management
    public static bool StartSet;


    private void Awake()
    {
        // For unstuck
        unstuckButton = SearchTools.TryFind("UI/UI_Gameplay/Canvas_InPlay/Panel_Messages/ButtonUnstuck");
        unstuckButton.SetActive(false);
        StartCoroutine(CheckBallStuck());

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
        if (level > 7)
            level = 8;
        startSpeed = 5f + level * 0.5f;
        maxSpeed = 7.75f + level * 0.75f;
        speed = startSpeed;
    }

    void Update()
    {
        if (unstuckBallTrigger)
        {
            unstuckBallTrigger = false;
            UnstuckBall();
        }
    }

    void FixedUpdate()
    {
        if (!StartSet)
            return;

        if (!ballReleased)
        {
            FollowPaddle();
            return;
        }

        // Change the vel of the ball after a collision
        if (changeVel)
        {
            changeVel = false;
            rb.velocity = ballVel = nextVel;
        }
        // Remember the ball vel when we collide an object so we can get the reflection.
        else
            ballVel = rb.velocity;
    }

    void OnDestroy()
    {
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
            ballReleased = true;
        }
        else if (testingHorizontal)
        {
            rb.velocity = Vector2.right * maxSpeed;
            ballReleased = true;
        }
        else
          PlaceBallAtPaddle();

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
        StartCoroutine(SpeedIncrese());
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
            trail.enabled = false;
            // Prevent multiple losing or losing when setting the game
            if (LevelManager.lives > 0)
                LevelManager.LoseLive();
            return;
        }

        AudioManager.PlayAudio(audioSource, ballHitAudio, false, 0.5f);
        ContactPoint2D contact = collision.GetContact(0);

        if (collision.gameObject.CompareTag("Player"))
        {
            // Get the reflect direction of the ball in the paddle, its different because it may change the ball dir in the x axis
            VelAfterPaddleColl(contact, ref nextVel);
        }
        else
        {
            // Get the reflect direction of the ball if it did not hit the paddle
            VelReflected(contact, ref nextVel);

            if (collision.gameObject.CompareTag("Brick"))
            {
                // Count the remaining bricks for knowing when to win
                collision.gameObject.GetComponent<Bricks>().GotHit();
                LevelManager.CheckNumberOfBricks();
            }
        }

        changeVel = true;
    }

    private void VelAfterPaddleColl(ContactPoint2D contact, ref Vector2 nextVel)
    {
        Vector2 finalDir, inNormal, ballReflectedVel;
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
            var yVel = Mathf.Sqrt( (Mathf.Pow(xVel, 2) + 1) ) * inNormal.y;

            // Check for obstacles so we can change the direction of the ball so it wont get blocked with the level bounds
            if ((xVel > 0) && (DetectObstacles()[1]))
                xVel = -xVel;
            else if ((xVel < 0) && (DetectObstacles()[0]))
                xVel = -xVel;

            finalDir = Vector3.Normalize(new Vector2(xVel, yVel));
        }
        // Make a normal reflection in the ball in case of colliding in the paddle center
        else
        {
            ballReflectedVel = Vector2.Reflect(ballVel, inNormal);
            finalDir = Vector3.Normalize(ballReflectedVel);
        }

        nextVel = finalDir * speed;
    }

    private void VelReflected(ContactPoint2D contact, ref Vector2 nextVel)
    {
        Vector2 finalDir, inNormal, ballReflectedVel;
        inNormal = contact.normal;
        ballReflectedVel = Vector2.Reflect(ballVel, inNormal);
        finalDir = Vector3.Normalize(ballReflectedVel);
        nextVel = finalDir * speed;
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

    #region Meanwhile functions

    public void RestartBall()
    {
        StopAllCoroutines();
        ballReleased = false;
        rb.velocity = Vector3.zero;
        PlaceBallAtPaddle();
    }

    private IEnumerator SpeedIncrese()
    {
        // Delay
        float delay = 0;
        while(delay < 1f)
        {
            yield return null;
            delay += Time.deltaTime;
        }

        // Increase speed and restart the corroutine till reaching the maxSpeed
        speed += speedInc;
        if (speed >= maxSpeed)
            print("maxSpeed reached");
        else
            StartCoroutine(SpeedIncrese());
    }

    #endregion

    #region Unstuck functions

    /// <summary>
    /// Check if the ball is stucked moving completely horizontal.
    /// </summary>
    private IEnumerator CheckBallStuck()
    {
        // delay for performance
        float delay = 0f;
        while (delay < 0.5f)
        {
            yield return null;
            delay += Time.deltaTime;
        }

        // Check if the ball is stuck HORIZONTALLY
        float ballSpeed = Mathf.Abs(Vector3.Normalize(rb.velocity).x);
        if (ballSpeed < 0.9f)
            goto NextCheck;

        // If the ball is stuck moving horizontally, check if it stays like that for a certain time
        delay = 0f;
        while (delay < 5f)
        {
            yield return null;
            delay  += Time.deltaTime;
        }
        ballSpeed = Mathf.Abs(Vector3.Normalize(rb.velocity).x);
        if (ballSpeed >= 0.9f)
        {
            unstuckButton.SetActive(true);

            delay = 0f;
            while (delay < 7f)
            {
                yield return null;
                delay += Time.deltaTime;
            }

            unstuckButton.SetActive(false);
        }
        NextCheck:

        // Check if the ball is stuck VERTICALLY
        ballSpeed = Mathf.Abs(Vector3.Normalize(rb.velocity).y);
        if (ballSpeed < 0.9f)
            goto NotStuck;

        // If the ball is stuck moving horizontally, check if it stays like that for a certain time
        delay = 0f;
        while (delay < 4f)
        {
            yield return null;
            delay += Time.deltaTime;
        }
        ballSpeed = Mathf.Abs(Vector3.Normalize(rb.velocity).y);
        if (ballSpeed >= 0.9f)
        {
            unstuckButton.SetActive(true);

            delay = 0f;
            while (delay < 4f)
            {
                yield return null;
                delay += Time.deltaTime;
            }

            unstuckButton.SetActive(false);
        }

    NotStuck:
        StartCoroutine(CheckBallStuck());
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
        unstuckButton.SetActive(false);
    }

    #endregion

    #region powers

    public void BallSpeedPower(string power)
    {
        if (usingPower != null)
        {
            StopCoroutine(usingPower);
            StopPower();
        }
        usingPower = BallPower(power);
        StartCoroutine(usingPower);
    }

    private IEnumerator BallPower(string power)
    {
        switch (power)
        {
            case "slow":
                StopCoroutine(SpeedIncrese());
                previousSpeed = speed;
                speed = startSpeed * 0.8f;
                //print("slow");
                break;

            case "fast":
                StopCoroutine(SpeedIncrese());
                previousSpeed = speed;
                speed = maxSpeed * 1.3f;
                //print("fast");
                break;
        }

        // Delay
        float delay = 0;
        while (delay < Powers.powerTime)
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
        speed = previousSpeed;
        StartCoroutine(SpeedIncrese());
    }

    #endregion

}