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
    private IEnumerator animCorroutine;
    private Vector2 startPos;

    // Breaking brick
    private Sprite brokenBrickSprite;
    private SpriteRenderer spriteRenderer;
    private bool brickBroken;
    private GameObject breakAnimationPref;


    void Awake()
    {
        startPos = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        if ( (brickNum > 1) && (brickNum < 6) )
            brokenBrickSprite = Resources.Load<Sprite>($"Art2D/BrokenBricks/Brick0{brickNum}");
        breakAnimationPref = Resources.Load<GameObject>($"Prefabs/LevelDev/Bricks_ParticleSystems/Brick0{brickNum}_ParticleSystem");
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
        {
            AudioManager.PlayAudio_WithoutInterruption(ref BricksSystem.bricksAudioSources, BricksSystem.metalHitAudio, transform.parent.gameObject, false, 0.35f);
            return;
        }

        // substract life from the brick
        currentLife--;

        // Change the brick sprite to another that looks breaking when the brick reach half of its life
        if (currentLife > 0)
        {
            AudioManager.PlayAudio_WithoutInterruption(ref BricksSystem.bricksAudioSources, BricksSystem.hitAudio, transform.parent.gameObject, false, 0.7f);

            // Animation of the brick when hit
            if (animCorroutine == null)
            {
                animCorroutine = BrickHitFeedback();
                StartCoroutine(animCorroutine);
            }

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
            DestroyBrick();
        }
    }

    public void DestroyBrick()
    {
        AudioManager.PlayAudio_WithoutInterruption(ref BricksSystem.bricksAudioSources, BricksSystem.destructionAudio, transform.parent.gameObject, false, 0.7f);
        Instantiate(breakAnimationPref, transform.position, Quaternion.identity);
        MaySpawnPower();
        Destroy(gameObject);
    }

    private void MaySpawnPower()
    {
        // Limit the amount of powers in the scene
        if (PowersSystem.powersSpawned >= 2)
            return;

        // Chance to spawn a power
        if ((Random.Range(0, 100)) > 10)
            return;

        // Spawn random power
        PowersSystem.powersSpawned++;
        int power = Random.Range(1, 5);
        GameObject powerToSpawn = null;
        string path = "Prefabs/LevelDev/PowersSystem";
        switch (power)
        {
            case 1:
                powerToSpawn = Resources.Load<GameObject>($"{path}/PowerFast");
                break;

            case 2:
                powerToSpawn = Resources.Load<GameObject>($"{path}/PowerSlow");
                break;

            case 3:
                powerToSpawn = Resources.Load<GameObject>($"{path}/PowerShort");
                break;

            case 4:
                powerToSpawn = Resources.Load<GameObject>($"{path}/PowerLarge");
                break;

            default:
                return;
        } 
        if(powerToSpawn != null)
            Instantiate(powerToSpawn, transform.position, Quaternion.identity);
    }

    private IEnumerator BrickHitFeedback()
    {
        // Brick vibration for a certain time
        float speed = 15f;
        float amount = 0.03f;
        float delay = 0f;
        while(delay < 0.2f)
        {
            float delay2 = 0f;
            while (delay2 < 0.05f)
            {
                yield return null;
                delay += Time.deltaTime;
                delay2 += Time.deltaTime;
            }
            float formula = Mathf.Sin(delay * 8 * speed) * amount;
            transform.position = startPos + new Vector2(formula, formula);
        }

        // Go back to the normal position
        transform.position = startPos;

        animCorroutine = null;
    }

}