using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ball : MonoBehaviour
{
    // Balleneral data
    private Rigidbody2D rb;
    private float speed;
    private readonly float startSpeed = 5.5f, maxSpeed = 12f, speedInc = 0.075f;
    private float ballRad;
    public static bool ballReleased;

    // Paddle data
    private Paddle paddleCode;
    private Rigidbody2D paddleRb;

    // Ball speed changing
    private Vector2 ballVel;
    private bool changeVel;
    private Vector2 nextVel;

    //Audio
    private AudioSource audioSource;
    private AudioClip loseLifeAudio, ballHitAudio, ballReleaseAudio;

    // Unstuck process
    private int NumberOfcollisions;
    private enum ColSide { left, right, up, down }
    private ColSide collisionSide;
    private enum PaddleCol { none, left, right, center }

    // Management
    public static bool StartSet;


    public void Begin()
    {
        //Audio
        loseLifeAudio = Resources.Load<AudioClip>("Audio/Level general/(gs1) losing life");
        ballHitAudio = Resources.Load<AudioClip>("Audio/Level objects/(lo1) ballHit");
        ballReleaseAudio = Resources.Load<AudioClip>("Audio/Level objects/(lo1) releaseBall");
        audioSource = GetComponent<AudioSource>();

        // Ball
        rb = GetComponent<Rigidbody2D>();
        ballRad = GetComponent<CircleCollider2D>().radius;
        StartCoroutine(SetBall());
    }

    void FixedUpdate()
    {
        if (!StartSet)
            return;

        if (!ballReleased)
            FollowPaddle();
        else
        {
            if (changeVel)
            {
                changeVel = false;
                rb.velocity = nextVel;
            }
            ballVel = rb.velocity;
        }
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
        GameObject paddle = GameObject.Find("LevelDev/Paddle_(Clone)");
        paddleRb = paddle.GetComponent<Rigidbody2D>();
        paddleCode = paddle.GetComponent<Paddle>();

        PlaceBallAtPaddle();
        
        StartSet = true;
    }

    private void PlaceBallAtPaddle()
    {
        Vector3 paddlePos = paddleRb.transform.position;
        transform.position = paddlePos + (Vector3.up * (paddleCode.paddleSize.y / 2 + transform.localScale.y / 2 + 0.1f));
    }

    private void FollowPaddle() => rb.position = new Vector2(paddleRb.position.x, rb.position.y);

    public void ReleaseBall()
    {
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

        // Data needed for the ball reflection
        ContactPoint2D contact = collision.GetContact(0);
        Vector2 inNormal = Vector2.zero;
        Vector2 ballReflectedVel = Vector2.zero;
        Vector2 finalDir = Vector2.zero;

        // Remember how many objects the ball is colliding with for unstuck purposes
        NumberOfcollisions++;

        if (collision.gameObject.CompareTag("Player"))
        {
            // Get the reflect direction of the ball in the paddle, its different because it may change the ball dir in the x axis
            GetBallDirAfterPaddleColl(contact, ref inNormal, ref finalDir, ref ballReflectedVel);
        }
        else
        {
            // Get the reflect direction of the ball if it did not hit the paddle
            GetBallReflecterDir(contact, ref inNormal, ref finalDir, ref ballReflectedVel);

            if (collision.gameObject.CompareTag("Brick"))
            {
                // Count the remaining bricks for knowing when to win
                collision.gameObject.GetComponent<Bricks>().GotHit();
                LevelManager.CheckNumberOfBricks();
            }
            else if(collision.gameObject.CompareTag("BottomBound"))
            {
                // Prevent multiple losing or losing when setting the game
                if (LevelManager.lives <= 0)
                    return;

                LoseLive();
                return;
            }
        }

        // Redirection of the ball when there is something in the way of the ball, then it goes the other way
        BlockingCollisionRedirection(contact, ref finalDir);

        // Finally redirect the ball after all the collision processes
        nextVel = finalDir * speed;
        changeVel = true;

        // Audio
        AudioManager.PlayAudio(audioSource, ballHitAudio, false, 0.5f);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Remember how many objects the ball is colliding with for unstuck purposes
        NumberOfcollisions--;
    }

    private void GetBallDirAfterPaddleColl(ContactPoint2D contact, ref Vector2 inNormal, ref Vector2 finalDir, ref Vector2 ballReflectedVel)
    {
        Vector2 contactPoint = contact.point;

        // Check if the ball is hitting the paddle upwards or downwards for setting the normal manually, evading weird reflections in the borders.
        Vector2 paddlePos = paddleRb.position;
        if (contactPoint.y > paddlePos.y)
            inNormal = Vector2.up;
        else
            inNormal = Vector2.down;

        // Change the horizontal direction of the ball if the collision was with the paddle sides
        float paddleSizeX = paddleCode.paddleSize.x;
        if ((contactPoint.x < paddlePos.x - paddleSizeX / 8) || (contactPoint.x > paddlePos.x + paddleSizeX / 8))
        {
            // 0.707 stands for 45° normalized dir in one axis
            var xVel = MapValue(contactPoint.x, paddlePos.x - paddleSizeX / 2f, paddlePos.x + paddleSizeX / 2f, -0.707f, 0.707f);
            // find the leg from the hypotenuse formula
            var yVel = Mathf.Sqrt( (Mathf.Pow(xVel, 2) + 1) ) * inNormal.y;
            Vector2 vel = new Vector2(xVel, yVel);
            // Should be already normalized but still do it in code just in case.
            finalDir = Vector3.Normalize(vel);
        }

        // Make a normal reflection in the ball in case of colliding in the paddle center
        else
        {
            ballReflectedVel = Vector2.Reflect(ballVel, inNormal);
            finalDir = Vector3.Normalize(ballReflectedVel);
        }
    }

    private void GetBallReflecterDir(ContactPoint2D contact, ref Vector2 inNormal, ref Vector2 finalDir, ref Vector2 ballReflectedVel)
    {
        inNormal = contact.normal;
        ballReflectedVel = Vector2.Reflect(ballVel, inNormal);
        finalDir = Vector3.Normalize(ballReflectedVel);
    }

    private float MapValue(float value, float min1, float max1, float min2, float max2)
    {
        return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
    }

    /// <summary>
    /// Redirect the ball direction when trying to move that ball in a direction in which there is something blocking the path.
    /// </summary>
    /// <param name="collContact"> Contact point of the ball collision. </param>
    /// <param name="finalDirection"> The final direction variable for later adding the speed to get the new velocity of the ball.  </param>
    private void BlockingCollisionRedirection(ContactPoint2D collContact, ref Vector2 finalDirection)
    {
        if (NumberOfcollisions == 1)
        {
            // remember the colision side of the first collision, so i know were the obstacle is if i have another collision
            if (collContact.point.x >= rb.position.x + ballRad - 0.1f)
                collisionSide = ColSide.right;
            else if (collContact.point.x <= rb.position.x - ballRad + 0.1f)
                collisionSide = ColSide.left;
            else if (collContact.point.y >= rb.position.y + ballRad - 0.1f)
                collisionSide = ColSide.up;
            else if (collContact.point.y <= rb.position.y - ballRad + 0.1f)
                collisionSide = ColSide.down;
        }
        else if(NumberOfcollisions > 1)
        {
            // redirect the ball if there is an obstacle
            switch (collisionSide)
            {
                case ColSide.right:
                    if(finalDirection.x > 0f)
                    {
                        print("the ball was colliding with something in the RIGHT before the new collision.");
                        finalDirection -= (2 * finalDirection.x * Vector2.right);
                    }
                    break;

                case ColSide.left:
                    if (finalDirection.x < 0f)
                    {
                        print("the ball was colliding with something in the LEFT before the new collision.");
                        finalDirection -= (2 * finalDirection.x * Vector2.right);
                    }
                    break;

                case ColSide.up:
                    if (finalDirection.y > 0f)
                    {
                        print("the ball was colliding with something in the UPSIDE before the new collision.");
                        finalDirection -= (2 * finalDirection.y * Vector2.up);
                    }
                    break;

                case ColSide.down:
                    if (finalDirection.y < 0f)
                    {
                        print("the ball was colliding with something in the DOWNSIDE before the new collision.");
                        finalDirection -= (2 * finalDirection.y * Vector2.up);
                    }
                    break;

                default:
                    print("This should never happen");
                    break;
            }
        }
    }

    private void LoseLive()
    {
        //Lose lives
        LevelManager.lives--;
        UI_Manager.RewriteLife();
        if (LevelManager.lives < 1)
        {
            GameManager.LoseGame();
            Destroy(this.gameObject);
            return;
        }
        else
        {
            AudioManager.PlayAudio(AudioManager.GameAudioSource, loseLifeAudio, false, 0.4f);
        }

        // Restart the ball
        StopAllCoroutines();
        ballReleased = false;
        rb.velocity = Vector3.zero;
        PlaceBallAtPaddle();
    }

    #endregion

    #region  during functions

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
        //print("speed: " + speed);
        if (speed >= maxSpeed)
        {
            print("maxSpeed reached");
            yield break;
        }
        StartCoroutine(SpeedIncrese());
    }

    /// <summary>
    /// Check if the ball is stucked moving completely horizontal.
    /// </summary>
    private IEnumerator CheckBallStuck()
    {
        // delay for performance
        float delay = 0f;
        while (delay < 1f)
        {
            yield return null;
            delay += Time.deltaTime;
        }

        // Check if the ball is stuck moving horizontally endlessly for 5 seconds
        float ballSpeed = Mathf.Abs(Vector3.Normalize(rb.velocity).x);
        float delay1 = 0f;
        while ( (ballSpeed >= 0.95f) || (delay1 < 5f))
        {
            float delay2 = 0f;
            while ((delay2 < 0.5f))
            {
                yield return null;
                delay2 = delay1  += Time.deltaTime;
            }

            ballSpeed = Mathf.Abs(Vector3.Normalize(rb.velocity).x);

            // Redirect the ball vertically if it had a non stop horizontal movement for five second
            print("completely horizontal hit, redirect verticaly");
            if (NumberOfcollisions >= 1)
            {
                if (collisionSide == ColSide.up)
                    rb.velocity -= 0.707f * speed * Vector2.one;
                else if (collisionSide == ColSide.down)
                    rb.velocity += 0.707f * speed * Vector2.one;
            }
        }

        StartCoroutine(CheckBallStuck());
    }

    #endregion

}