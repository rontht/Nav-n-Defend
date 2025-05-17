using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

// I'd like to note I utilized assistance in debugging this specifically. Very frustrating concept.

public class SinglePlaneDetector : MonoBehaviour
{
    // These allow for assigning within the UI, super helpful.
    
    [Header("AR Managers")]
    public ARPlaneManager planeManager;

    [Header("Plane Settings")]
    public float minPlaneWidth = 0.5f;
    public float minPlaneHeight = 0.5f;

    [Header("Debug")]
    public ARPlane selectedPlane;

    private bool planeLocked = false;

    void OnEnable()
    {
        StartCoroutine(WaitForARReady());
    }

    IEnumerator WaitForARReady()
    {
        while (ARSession.state != ARSessionState.SessionTracking)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1f); // extra buffer

        planeManager.planesChanged += OnPlanesChanged;
        planeManager.enabled = true;
    }

    void OnDisable()
    {
        if (planeManager != null)
        {
            planeManager.planesChanged -= OnPlanesChanged;
        }
    }

    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (planeLocked) return;

        List<ARPlane> candidates = new List<ARPlane>();
        candidates.AddRange(args.updated);
        candidates.AddRange(args.added);

        foreach (var plane in candidates)
        {
            if (IsPlaneBigEnough(plane))
            {
                StartCoroutine(LockOntoPlaneNextFrame(plane));
                break;
            }
        }
    }

    bool IsPlaneBigEnough(ARPlane plane)
    {
        return plane.size.x >= minPlaneWidth && plane.size.y >= minPlaneHeight;
    }

    IEnumerator LockOntoPlaneNextFrame(ARPlane plane)
    {
        yield return null; 

        planeLocked = true;
        selectedPlane = plane;

        // Stop further plane changes
        planeManager.planesChanged -= OnPlanesChanged;
        planeManager.enabled = false;

        // Hide other planes
        foreach (var p in planeManager.trackables)
        {
            if (p.trackableId != plane.trackableId)
                p.gameObject.SetActive(false);
        }
    }
}