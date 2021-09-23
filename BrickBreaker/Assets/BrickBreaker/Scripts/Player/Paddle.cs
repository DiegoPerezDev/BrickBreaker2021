using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    // Movement
    [HideInInspector] public bool? moveDirR = null;
    [HideInInspector] public bool moveBool, canMoveR, canMoveL;
    [SerializeField] private float speed = 2f;
    private Rigidbody2D rb;

    // Paddle set 
    private float leftScreenLimit, rightScreenLimit;
    private float camWidth;
    [HideInInspector] public Vector2 paddleSize;

    // Level management
    public static bool StartSet;


    public void Begin()
    {
        // Get components
        rb = GetComponent<Rigidbody2D>();

        // Get camera values for the screen boundaries, for limitting the paddle movement
        GetScreenValues();

        // Paddle
        //ResizePaddle();
        var collSize = GetComponent<CapsuleCollider2D>().size;
        paddleSize = new Vector2(collSize.x * transform.localScale.x, collSize.y * transform.localScale.y);
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
        if(moveBool)
            Move();
    }


    private void GetScreenValues()
    {
        Vector2 camPos = Camera.main.gameObject.transform.position;
        float camHeight = Camera.main.orthographicSize * 2f;
        camWidth = camHeight * Camera.main.aspect;
        leftScreenLimit = camPos.x - (camWidth / 2);
        rightScreenLimit = camPos.x + (camWidth / 2);
    }

    private void ResizePaddle()
    {
        paddleSize = new Vector2(camWidth / 6f, camWidth / 30f);
        transform.localScale = new Vector3(paddleSize.x, paddleSize.y, transform.localScale.z);
    }

    private void Move()
    {
        //  Check if the paddle is near the screen limits so it wont go outside of the screen.
        CheckLevelBounds();

        if(moveDirR == true)
        {
            if(canMoveR)
                rb.MovePosition(rb.position + new Vector2(speed, 0f) * Time.fixedDeltaTime);
        }
        else if (moveDirR == false)
        {
            if (canMoveL)
                rb.MovePosition(rb.position - new Vector2(speed, 0f) * Time.fixedDeltaTime);
        }
    }

    private void CheckLevelBounds()
    {
        if ((rb.position.x - (paddleSize.x / 2)) <= leftScreenLimit)
        {
            canMoveR = true;
            canMoveL = false;
        }
        else if((rb.position.x + (paddleSize.x / 2)) >= rightScreenLimit)
        {
            canMoveR = false;
            canMoveL = true;
        }
        else if(!canMoveR || !canMoveL)
        {
            canMoveR = canMoveL = true;
        }
    }

}