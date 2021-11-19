using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    Vector3 originalPos;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    void OnDestroy()
    {
        transform.localPosition = originalPos;
        StopAllCoroutines();    
    }

    /// <summary>
    /// Shake the camera with a certain magnitude for a set time. Perfect for getting hit fx or tension scenes.
    /// </summary>
    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }

}