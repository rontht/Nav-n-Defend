using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Diagnostics;
using System;

public class ImageTrackingToggle : MonoBehaviour
{
    [Header("AR Tracking")]
    public ARTrackedImageManager imageManager;

    [Header("UI Elements")]
    public GameObject generalUI;
    public GameObject scanMenu;
    public GameObject qrScanMenu;
    public GameObject countdownHold;
    public TMP_Text countdownText;
    public TMP_Text remainingSpawnsText;

    public TrackedImageSpawnManager spawnManager;

    private Coroutine countdownCoroutine;

    void Start()
    {
        if (imageManager != null)
        {
            imageManager.enabled = false;
        }

        if (generalUI != null)
        {
            generalUI.SetActive(false);
        }

        if (scanMenu != null)
        {
            scanMenu.SetActive(true);
        }

        if (qrScanMenu != null)
        {
            qrScanMenu.SetActive(false);
        }

        if (countdownHold != null)
        {
            countdownHold.SetActive(false);
        }

        // Donft try to access spawnManager here if it doesnft exist yet!
    }

    // 
    void OnEnable()
    {
        TrackedImageSpawnManager.OnSpawnManagerReady += HandleSpawnManagerReady;
    }

    void OnDisable()
    {
        TrackedImageSpawnManager.OnSpawnManagerReady -= HandleSpawnManagerReady;
    }

    private void HandleSpawnManagerReady(TrackedImageSpawnManager manager)
    {
        spawnManager = manager;
        UpdateRemainingSpawnsText();
    }

    void Update()
    {
        if (spawnManager != null)
        {
            UpdateRemainingSpawnsText();
        }
    }

    public void UpdateRemainingSpawnsText()
    {
        if (remainingSpawnsText != null && spawnManager != null)
        {
            remainingSpawnsText.text = "Remaining Spawns: " + spawnManager.remainingSpawns;
        }
    }

    public void OnSurfaceFoundPressed()
    {
        if (imageManager != null)
        {
            imageManager.enabled = false;
        }

        if (generalUI != null)
        {
            generalUI.SetActive(false);
        }

        if (scanMenu != null)
        {
            scanMenu.SetActive(false);
        }

        if (qrScanMenu != null)
        {
            qrScanMenu.SetActive(true);
        }
    }

    public void OnScanButtonPressed()
    {
        if (imageManager != null)
        {
            imageManager.enabled = true;
        }

        if (qrScanMenu != null)
        {
            qrScanMenu.SetActive(false);
        }

        if (countdownCoroutine == null)
        {
            if (countdownHold != null)
            {
                countdownHold.SetActive(true);
            }
            countdownCoroutine = StartCoroutine(StartCountdown(5)); // 5 going down
        }
    }

    private IEnumerator StartCountdown(int seconds)
    {
        int current = seconds;

        while (current > 0)
        {
            if (countdownText != null)
            {
                countdownText.text = current.ToString();
            }
            yield return new WaitForSeconds(1f);
            current--;
        }

        yield return new WaitForSeconds(0.5f);

        if (countdownHold != null)
        {
            countdownHold.SetActive(false);
        }

        if (generalUI != null)
        {
            generalUI.SetActive(true);
        }

        countdownCoroutine = null;
    }
}