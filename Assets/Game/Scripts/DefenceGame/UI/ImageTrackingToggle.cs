using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;

public class ImageTrackingToggle : MonoBehaviour
{
    private int initialSpawns = 0;
    [Header("AR Tracking")]
    public ARTrackedImageManager imageManager;

    [Header("UI Elements")]
    public GameObject generalUI;
    public GameObject scanMenu;
    public GameObject qrScanMenu;
    public GameObject countdownHold;
    public TMP_Text countdownText;
    public TMP_Text remainingSpawnsText;
    public TMP_Text totalKillsText;  

    
    // Utilized for subscribing, required for prefab referencing.
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

        // Subscribe to the enemyHealth.OnKill event to listen for kills.
        enemyHealth.OnKill += HandleEnemyKill;
    }

    void OnEnable()
    {
        TrackedImageSpawnManager.OnSpawnManagerReady += HandleSpawnManagerReady;
    }

    void OnDisable()
    {
        TrackedImageSpawnManager.OnSpawnManagerReady -= HandleSpawnManagerReady;

        // Unsubscribe from the OnKill.
        enemyHealth.OnKill -= HandleEnemyKill;
    }

    private void HandleSpawnManagerReady(TrackedImageSpawnManager manager)
    {
        spawnManager = manager;

        if (spawnManager != null)
        {
            initialSpawns = spawnManager.remainingSpawns;
            UnityEngine.Debug.Log("Initial Spawns Set To: " + initialSpawns);
        }

        UpdateRemainingSpawnsText();
    }

    // Method to handle enemy kills and update the UI.
    private void HandleEnemyKill(int kills)
    {
        // Update the UI to show the total kills
        if (totalKillsText != null)
        {
            totalKillsText.text = "Total Kills: " + kills;
        }

        UnityEngine.Debug.Log("Total Kills Updated: " + kills);
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
            UnityEngine.Debug.Log("Remaining Spawns: " + initialSpawns);
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