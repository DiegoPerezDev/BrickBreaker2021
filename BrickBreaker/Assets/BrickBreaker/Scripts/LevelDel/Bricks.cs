using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bricks : MonoBehaviour
{
    /*
     * NOTE: to know which brick is which with the enumeration and the amount of life it suppouse to hae, see the documentation.
     */

    // General brick data
    [Tooltip("Number of hits that the brick can take. one is the minimum.")] 
    [SerializeField] int lifeCap = 2;
    [Tooltip("With this number we can know which sprite to use when the brick is breaking and also the type of brick.")]
    [Range(1,7)] [SerializeField] int brickNum = 2;
    private int currentLife;
    private Vector2 startPos;

    // Breaking or hitting brick
    private IEnumerator animCorroutine;
    private Sprite brokenBrickSprite;
    private SpriteRenderer spriteRenderer;
    private bool brickBroken;
    private GameObject breakAnimationPref;


    void Awake()
    {
        // Get resources
        if ( (brickNum > 1) && (brickNum < 6) )
            brokenBrickSprite = Resources.Load<Sprite>($"Art2D/BrokenBricks/Brick0{brickNum}");
        breakAnimationPref = Resources.Load<GameObject>($"Prefabs/LevelDev/Bricks_ParticleSystems/Brick0{brickNum}_ParticleSystem");

        // Get components
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // Set values
        startPos = transform.position;
        currentLife = lifeCap;
    }

    void OnDestroy()
    {
        StopAllCoroutines();
        if (animCorroutine != null)
            StopCoroutine(animCorroutine);
    }


    public void GotHit()
    {
        // Check if this brick is one of the indestructible bricks
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
            AudioManager.PlayAudio_WithoutInterruption(ref BricksSystem.bricksAudioSources, BricksSystem.hitAudio, transform.parent.gameObject, false, 0.8f);

            // Animation of the brick when hit
            if (animCorroutine != null)
            {
                StopCoroutine(animCorroutine);
                animCorroutine = null;
            }
            animCorroutine = BrickHitFeedback();
            StartCoroutine(animCorroutine);

            // Apply the brocken brick sprite if its the case
            if (!brickBroken)
            {
                if ( currentLife == (lifeCap / 2) )
                {
                    brickBroken = true;
                    spriteRenderer.sprite = brokenBrickSprite;
                }
            }
        }
        else
            DestroyBrick();
    }

    public void DestroyBrick()
    {
        AudioManager.PlayAudio_WithoutInterruption(ref BricksSystem.bricksAudioSources, BricksSystem.destructionAudio, transform.parent.gameObject, false, 0.8f);
        Instantiate(breakAnimationPref, transform.position, Quaternion.identity);
        BricksSystem.CheckNumberOfBricks();
        MaySpawnPower();
        Destroy(gameObject);
    }

    private void MaySpawnPower()
    {
        // Limit the amount of powers in the scene
        if (PowersSystem.powersSpawned >= 2)
            return;

        // Chance to spawn a power
        if ((Random.Range(0, 100)) > 8)
            return;

        // Spawn random power
        PowersSystem.powersSpawned++;
        int power = Random.Range(1, 5);
        GameObject powerToSpawn;
        switch (power)
        {
            case 1:
                powerToSpawn = PowersSystem.fastPowerCapsule;
                break;

            case 2:
                powerToSpawn = PowersSystem.slowPowerCapsule;
                break;

            case 3:
                powerToSpawn = PowersSystem.smallPowerCapsule;
                break;

            case 4:
                powerToSpawn = PowersSystem.largePowerCapsule;
                break;

            default:
                PowersSystem.powersSpawned--;
                return;
        } 
        if(powerToSpawn != null)
            Instantiate(powerToSpawn, transform.position, Quaternion.identity);
    }

    private IEnumerator BrickHitFeedback()
    {
        // Brick vibration for a certain time
        float speed = 15f;
        float amount = 0.035f;

        float delay = 0f;
        while(delay < 0.16f)
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
    }

}