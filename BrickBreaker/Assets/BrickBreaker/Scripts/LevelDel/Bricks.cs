using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTools;

public class Bricks : MonoBehaviour
{
    // General brick data
    [Tooltip("Number of hits that the brick can take. one is the minimum.")] 
    [SerializeField] int lifeCap = 2;
    [Tooltip("With this number we can know which sprite to use when the brick is breaking and also the type of brick.")]
    [Range(1,7)] [SerializeField] int brickNum = 2;
    private int currentLife;

    // Breaking brick
    private Sprite brokenBrickSprite;
    private SpriteRenderer spriteRenderer;
    private bool brickBroken;
    private GameObject breakAnimationPref;
    //Color normalColor;
    //IEnumerator animCorroutine;


    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //normalColor = spriteRenderer.color;
        if ( (brickNum > 1) && (brickNum < 6) )
            brokenBrickSprite = Resources.Load<Sprite>($"BrokenBricks/Brick0{brickNum}");

        breakAnimationPref = Resources.Load<GameObject>($"Prefabs/Bricks/Brick0{brickNum}_ParticleSystem");
    }

    void Start()
    {
        currentLife = lifeCap;
    }

    void OnDestroy()
    {
        StopAllCoroutines();    
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
            AudioManager.PlayAudio(ref LevelManager.brickAudioSources, LevelManager.hitAudio, transform.parent.gameObject, false, 0.75f);

            // Blink animation in the brick when hit
            //if (animCorroutine == null)
            //{
            //    animCorroutine = brickHitFeedback();
            //    StartCoroutine(animCorroutine);
            //}

            // Apply the brocken brick sprite if its case
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
        else if(currentLife == 0)
        {
            AudioManager.PlayAudio(ref LevelManager.brickAudioSources, LevelManager.destructionAudio, transform.parent.gameObject, false, 0.7f);
            Instantiate(breakAnimationPref, transform.position, Quaternion.identity);
            LevelManager.numberOfActiveBricks--;
            Destroy(gameObject);
        }
    }

    /*
    private IEnumerator brickHitFeedback()
    {
        print("started");

        // "blink" animation of the brick when it gets hit
        for (int j = 0; j < 3; j++)
        {
            float delay = 0;
            while (delay < 1f)
            {
                yield return null;
                delay += Time.deltaTime;
                spriteRenderer.color = Color.Lerp(normalColor, normalColor - new Color(100/255f, 100 / 255f, 100 / 255f, 0), Mathf.PingPong(Time.time, delay));
            }
        }
        //spriteRenderer.color

        print("ended");
        //animCorroutine = null;
    }
    */
}