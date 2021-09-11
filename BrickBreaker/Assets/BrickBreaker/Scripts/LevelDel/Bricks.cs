using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bricks : MonoBehaviour
{
    [Tooltip("Number of hits that the brick can take. one is the minimum.")] 
    [SerializeField] int lifeCap = 2;
    [Tooltip("With the number of the brick we know which sprite to use when the brick is breaking.")]
    [Range(1,7)] [SerializeField] int brickNum = 2;
    private int currentLife;
    private Sprite brokenBrickSprite;
    private SpriteRenderer spriteRenderer;
    private bool brickBroken;


    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        var num = brickNum.ToString();
        if ( (brickNum > 1) && (brickNum < 6) )
            brokenBrickSprite = Resources.Load<Sprite>($"BrokenBricks/Brick0{num}");
    }

    void Start()
    {
        currentLife = lifeCap;
    }


    public void GotHit()
    {
        // Check if one of the indestructible bricks
        if (brickNum > 6)
            return;

        // substract life from the brick
        currentLife--;

        // Change the brick sprite to another that looks breaking when the brick reach half of its life
        if (currentLife > 0)
        {
            if (!brickBroken)
            {
                if ( currentLife == (lifeCap / 2) )
                {
                    brickBroken = true;
                    spriteRenderer.sprite = brokenBrickSprite;
                }
            }
        }
        // Destroy this brick and substract if in the 'LevelManager' when life = 0
        else
        {
            LevelManager.numberOfActiveBricks--;
            Destroy(this.gameObject);
        }
    }

}