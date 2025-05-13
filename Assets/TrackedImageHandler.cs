using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class TrackedImageHandler : MonoBehaviour
{
    public ARTrackedImageManager imageManager;
    public GameObject trackedPrefab;       // Your cube prefab with spawn points
    public GameObject objectToSpawn;       // The object to spawn at spawn points

    void OnEnable()
    {
        imageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    void OnDisable()
    {
        imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs args)
    {
        foreach (var trackedImage in args.added)
        {
            SpawnAtTrackedImage(trackedImage);
        }

        foreach (var trackedImage in args.removed)
        {
            StopSpawningFromImage(trackedImage);
        }
    }

    void SpawnAtTrackedImage(ARTrackedImage trackedImage)
    {
        Vector3 position = trackedImage.transform.position;
        Quaternion rotation = trackedImage.transform.rotation;

        GameObject spawned = Instantiate(trackedPrefab, position, rotation);

        var spawnManager = spawned.GetComponent<TrackedImageSpawnManager>();
        if (spawnManager != null)
        {
            spawnManager.Initialize(objectToSpawn); // Start timed spawning
        }
        else
        {
            UnityEngine.Debug.LogWarning("TrackedImageSpawnManager not found on prefab.");
        }
    }

    private void StopSpawningFromImage(ARTrackedImage trackedImage)
    {
        // Try to find the spawn manager in the children of the tracked image
        var spawnManager = trackedImage.GetComponentInChildren<TrackedImageSpawnManager>();

        if (spawnManager != null)
        {
            spawnManager.StopSpawning();
            UnityEngine.Debug.Log("Stopped spawning for lost image: " + trackedImage.referenceImage.name);
        }
        else
        {
            UnityEngine.Debug.LogWarning("No spawn manager found to stop for image: " + trackedImage.referenceImage.name);
        }
    }
}
