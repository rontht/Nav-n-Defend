using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARResetHelper : MonoBehaviour
{
    public ARTrackedImageManager imageManager;

    void OnEnable()
    {
        StartCoroutine(ResetTracking());
    }

    private IEnumerator ResetTracking()
    {
        // Wait one frame to ensure scene has reloaded
        yield return null;

        if (imageManager != null)
        {
            imageManager.enabled = false;
            yield return new WaitForSeconds(0.5f); // Small delay
            imageManager.enabled = true;
            UnityEngine.Debug.Log("ARTrackedImageManager reset.");
        }
    }
}
