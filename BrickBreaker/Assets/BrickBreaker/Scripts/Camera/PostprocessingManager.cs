using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostprocessingManager : MonoBehaviour
{
    public static PostProcessLayer processLayer;

    /// <summary>
    /// Enable and disables the only post processing layer of the main camera.
    /// </summary>
    public static void EnablePostProcessing(bool enable)
    {
        GameObject currentCamera = Camera.main.gameObject;
        processLayer = currentCamera.GetComponent<PostProcessLayer>();

        if (processLayer != null)
            processLayer.enabled = enable;
    }

}