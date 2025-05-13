using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class slimeMovement : MonoBehaviour
{
    public ARTrackedImageManager trackedImageManager;
    public GameObject objectToMove;
    public string targetImageName;
    public float speed = 1f;

    public float totalTime = 4f;
    private float currentTime;

    private Vector3 targetPosition;
    private bool targetFound = false;

    void OnEnable()
    {
        trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            if (trackedImage.referenceImage.name == targetImageName)
            {
                targetPosition = trackedImage.transform.position;
                targetFound = trackedImage.trackingState == TrackingState.Tracking;
            }
        }
    }
    
    void Start()
    {
        currentTime = totalTime;
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
        }

        if (targetFound && currentTime <= 0)
        {
            float step = speed * Time.deltaTime;
            objectToMove.transform.position = Vector3.MoveTowards(
                objectToMove.transform.position,
                targetPosition,
                step
            );
        }
    }

}
