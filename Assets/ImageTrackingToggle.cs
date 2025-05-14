using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ImageTrackingToggle : MonoBehaviour
{
    public ARTrackedImageManager imageManager;

    void Start()
    {
        if (imageManager != null)
        {
            imageManager.enabled = false; // Disable tracking at startup
        }
    }

    public void EnableImageTracking()
    {
        if (imageManager != null)
        {
            imageManager.enabled = true; // Enable tracking when user clicks a button
            UnityEngine.Debug.Log("Image tracking enabled.");
        }
    }
}
