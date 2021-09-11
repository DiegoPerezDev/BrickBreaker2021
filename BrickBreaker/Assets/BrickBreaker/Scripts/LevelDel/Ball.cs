using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Ball : MonoBehaviour
{
    // General data
    private Rigidbody2D rb, paddleRb;
    private float speed, maxSpeed = 12f, speedInc = 0.075f;
    private readonly float startSpeed = 5.5f;
    private Paddle paddleCode;
    private float ballSize;

    // Ball speed changing
    private Vector2 ballVel;
    private bool changeVel;
    private Vector2 nextVel; 

    // Unstuck process
    private bool collidingUp;
    private int NumberOfcollisions;
    private enum ColSide { horizontal, vertical }
    private ColSide collisionSide;
    private enum PaddleCol { none, left, right, center }

    // Management
    public static bool StartSet;


    void Awake()
    {
        GameObject paddle = GameObject.Find("Paddle_");
        paddleRb = paddle.GetComponent<Rigidbody2D>();
        paddleCode = paddle.GetComponent<Paddle>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        StartCoroutine(SetBall());
        ballSize = GetComponent<CircleCollider2D>().radius;
    }

    void FixedUpdate()
    {
        if(!LevelManager.ballReleased)
            FollowPaddle();

        if(changeVel)
        {
            changeVel = false;
            rb.velocity = nextVel;
        }
        ballVel = rb.velocity;
    }

    // If the ball goes out of the field of view then lose a live, restart the ball or maybe even lose the game
    void OnBecameInvisible()
    {
        // Prevent losing when setting the game
        if (LevelManager.lives <= 0)
            return;

        //Lose lives
        LevelManager.lives--;
        UI_Manager.RewriteLife();
        if (LevelManager.lives < 1)
        {
            GameManager.LoseGame();
            Destroy(this.gameObject);
            return;
        }

        // Restart the ball
        StopAllCoroutines();
        rb.velocity = Vector3.zero;
        PlaceBallAtPaddle();
        LevelManager.ballReleased = false;
    }

    void OnDestroy()
    {
        StopAllCoroutines();
    }


    #region Start functions

    private IEnumerator SetBall()
    {
        //Wait for paddle to be set because we are going to use its size that is set in the start function.
        while (!Paddle.StartSet)
        {
            yield return null;
        }

        //ResizeBall();
        PlaceBallAtPaddle();

        StartSet = true;
    }

    private void ResizeBall() => transform.localScale = new Vector3(paddleCode.paddleSize.y, paddleCode.paddleSize.y, transform.localScale.z);

    private void PlaceBallAtPaddle()
    {
        Vector3 paddlePos = paddleRb.transform.position;
        transform.position = paddlePos + (Vector3.up * (paddleCode.paddleSize.y / 2 + transform.localScale.y / 2 + 0.1f));
    }

    #endregion

    #region Main functions

    private void FollowPaddle() => rb.position = new Vector2(paddleRb.position.x, rb.position.y);

    public void ReleaseBall()
    {
        rb.velocity = new Vector2(0f, speed = startSpeed);
        StartCoroutine(SpeedIncrese());
        //StartCoroutine(CheckBallStuck());
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Evade colliding when we have not released the ball
        if (!LevelManager.ballReleased)
            return;

        ContactPoint2D contact = collision.GetContact(0);
        Vector2 inNormal;
        Vector2 ballReflectedVel;
        Vector2 finalDir;

        // need coment
        NumberOfcollisions++;

        // Change the horizontal direction of the ball if the collision was with the paddle
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 contactPoint = contact.point;

            // Check if the ball is hitting the paddle upwards or downwards
            Vector2 paddlePos = paddleRb.position;
            if (contactPoint.y > paddlePos.y)
                inNormal = Vector2.up;
            else
                inNormal = Vector2.down;

            // Check if the paddle is hitting the paddle in the sides
            float paddleSizeX = paddleCode.paddleSize.x;
            if ( (contactPoint.x < paddlePos.x - paddleSizeX / 3) || (contactPoint.x > paddlePos.x + paddleSizeX / 3) )
            {
                var xVel = MapValue(contactPoint.x, paddlePos.x - paddleSizeX / 2f, paddlePos.x + paddleSizeX / 2f, -0.707f, 0.707f); // 0.707 = 45° normalized dir
                var yVel = Mathf.Sqrt((Mathf.Pow(xVel, 2) + 1)); // find the leg from the hypotenuse formula
                Vector2 vel = new Vector2(xVel, yVel);
                finalDir = Vector3.Normalize(vel);
            }
            else
            {
                ballReflectedVel = Vector2.Reflect(ballVel, inNormal);
                finalDir = Vector3.Normalize(ballReflectedVel);
            }

            goto Redirection;
        }
        else if (collision.gameObject.CompareTag("Brick"))
        {
            // Count the remaining bricks for knowing when to win
            collision.gameObject.GetComponent<Bricks>().GotHit();
            LevelManager.CheckNumberOfBricks();
        }

        // Reflect the ball as it comes if it did not hit the paddle sides
        inNormal = contact.normal;
        ballReflectedVel = Vector2.Reflect(ballVel, inNormal);
        finalDir = Vector3.Normalize(ballReflectedVel);

        // Redirection of the ball
        Redirection:
        SecondCollisionRedirection(contact, ref finalDir);

        // Finally redirect the ball after all the collision processes
        changeVel = true;
        nextVel = finalDir * speed;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // need comment
        NumberOfcollisions--;
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
        //print("SPEED: " + speed);
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

        // Check if the ball is stuck, if so, then check if it remains stuck for 5 seconds and then unstuck it
        while( Mathf.Abs(Vector3.Normalize(rb.velocity).x) >= 0.95f)
        {
            delay = 0f;
            while ( (delay < 5f) )
            {
                yield return null;
                delay += Time.deltaTime;
            }

            if(collidingUp)
                rb.velocity += new Vector2(0f, -startSpeed/2f);
            else
                rb.velocity += new Vector2(0f, startSpeed);
            print("completely horizontal hit, redirect verticaly");
            break;
        }

        StartCoroutine(CheckBallStuck());
    }

    #endregion

    private float MapValue(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }

    /// <summary>
    /// Redirect the ball direction when trying to move that ball in a direction in which there is something blocking the path.
    /// </summary>
    /// <param name="collContact"> Contact point of the ball collision. </param>
    /// <param name="finalDirection"> The final direction variable for later adding the speed to get the new velocity of the ball.  </param>
    private void SecondCollisionRedirection(ContactPoint2D collContact, ref Vector2 finalDirection)
    {
        if (NumberOfcollisions <= 1)
        {
            // remember the colision side
            if ((collContact.point.x >= rb.position.x + ballSize - 0.05f) || (collContact.point.x <= rb.position.x - ballSize + 0.05f))
            {
                collisionSide = ColSide.horizontal;
            }
            else if ((collContact.point.y >= rb.position.y + ballSize - 0.05f) || (collContact.point.y <= rb.position.y - ballSize + 0.05f))
            {
                collisionSide = ColSide.vertical;
            }
        }
        else
        {
            //print($"colliding multiple things: {NumberOfcollisions}");

            // redirect the ball if there is an obstacle
            switch (collisionSide)
            {
                case ColSide.horizontal:
                    //print("the ball was colliding with something horizontally.");
                    finalDirection -= (Vector2.right * finalDirection.x * 2);
                    break;

                case ColSide.vertical:
                    //print("the ball was colliding with something vertically.");
                    finalDirection -= (Vector2.up * finalDirection.y * 2);
                    break;

                default:
                    print("This should never happen");
                    break;
            }
        }
    }

}