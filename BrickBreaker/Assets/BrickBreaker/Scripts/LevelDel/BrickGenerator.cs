using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTools;

public class BrickGenerator : MonoBehaviour
{
    // Bricks data
    private readonly float gapX = 0.2f, gapY = 0.2f;
    private readonly int row = 4, column = 4;
    private readonly float firstPosX = -4f, firstPosY = 3f;
    private float brickHeight, brickWidth;
    private GameObject brickPrefab;

    // Level management
    public static bool StartSet;


    void Awake()
    {
        GetBricksData();
    }

    void Start()
    {
        SetBricks();
        StartSet = true;
    }


    private void GetBricksData()
    {
        brickPrefab = SearchTools.TryFind("LevelDev/Brick");
        SpriteRenderer brickSprite = SearchTools.TryGetComponent<SpriteRenderer>(brickPrefab);
        brickHeight = brickSprite.size.y * brickPrefab.transform.localScale.y;
        brickWidth = brickSprite.size.x * brickPrefab.transform.localScale.x;
    }

    private void SetBricks()
    {
        for (int a = 0; a < row; a++)
        {
            for (int b = 0; b < column; b++)
            {
                Instantiate(brickPrefab, new Vector3(firstPosX + ((brickWidth + gapX) * a), firstPosY - ((brickHeight + gapY) * b), 0), Quaternion.identity);
                LevelManager.numberOfActiveBricks++;
            }
        }
    }

}