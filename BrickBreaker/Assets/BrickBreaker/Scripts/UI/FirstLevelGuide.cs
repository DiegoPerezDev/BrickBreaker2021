using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirstLevelGuide : MonoBehaviour
{
    private static Image leftIndicator, rightIndicator;
    private static bool fadingOut = true;
    private float timePassed;

    public void Set()
    {
        leftIndicator = GameObject.Find("UI/Canvas_HUD/TouchGuideLeft").gameObject.GetComponent<Image>();
        rightIndicator = GameObject.Find("UI/Canvas_HUD/TouchGuideRight").gameObject.GetComponent<Image>();
        if (SceneManager.GetActiveScene().buildIndex - 1 == 1)
            MoveGuide();
        else
            DestroyIndicators();
    }

    void Update()
    {
        timePassed += Time.deltaTime;
        if(timePassed >= 5f)
        {
            DestroyIndicators();
            Destroy(this);
        }
    }

    private void MoveGuide()
    {
        StartCoroutine(MoveAnimation());
    }

    private void DestroyIndicators()
    {
        Destroy(GameObject.Find("UI/Canvas_HUD/TouchGuideLeft").gameObject);
        Destroy(GameObject.Find("UI/Canvas_HUD/TouchGuideRight").gameObject);
    }

    private IEnumerator MoveAnimation()
    {
        yield return null;

        // delay
        float delay = 0;
        while (delay < 0.05f)
        {
            yield return null;
            delay += Time.deltaTime;
        }

        // color fade
        if (!fadingOut)
        {
            leftIndicator.color += new Color(0f, 0f, 0f, 0.075f);
            rightIndicator.color += new Color(0f, 0f, 0f, 0.075f);
        }
        else
        {
            leftIndicator.color -= new Color(0f, 0f, 0f, 0.075f);
            rightIndicator.color -= new Color(0f, 0f, 0f, 0.075f);
        }

        // Check if fade finished so we can set the fade to the other side.
        if (leftIndicator.color == Color.white)
            fadingOut = true;
        else if (leftIndicator.color == new Color(1f, 1f, 1f, 0.4f))
            fadingOut = false;

        StartCoroutine(MoveAnimation());
    }

}