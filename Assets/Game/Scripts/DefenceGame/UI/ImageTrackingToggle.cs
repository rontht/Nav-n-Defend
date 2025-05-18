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
    private int bulletKills = 0;

    [Header("AR Tracking")]
    public ARTrackedImageManager imageManager;

    [Header("UI Elements")]
    public GameObject generalUI;
    public GameObject scanMenu;
    public GameObject qrScanMenu;
    public GameObject countdownHold;
    public GameObject victoryUI;
    public GameObject gameOverUI;
    public TMP_Text countdownText;
    public TMP_Text remainingSpawnsText;
    public TMP_Text totalKillsText;
    public TMP_Text bulletKillsText;

    // Utilized for subscribing, required for prefab referencing.
    public TrackedImageSpawnManager spawnManager;

    private Coroutine countdownCoroutine;

   /// <summary>
   /// General UI P1, logic shared for start. When merging games onto same scene, utilize calls here.
   /// Call your UI and state false, the only one here to be true should be scanMenu.
   /// </summary>
    
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

        if (victoryUI != null)
        {
            victoryUI.SetActive(false);
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
            // Subscribe to the enemyHealth.OnKill event to listen for kills.
            enemyHealth.OnKill += HandleEnemyKill;
    }

    /// <summary>
    /// Subscripions to prefabs start here.
    /// Prefab UI calls found below.
    /// </summary>
    void OnEnable()
    {
        TrackedImageSpawnManager.OnSpawnManagerReady += HandleSpawnManagerReady;
        CenterTriggerDamage.OnStructureDestroyed += HandleStructureDestroyed;
    }

    void OnDisable()
    {
        TrackedImageSpawnManager.OnSpawnManagerReady -= HandleSpawnManagerReady;
        CenterTriggerDamage.OnStructureDestroyed -= HandleStructureDestroyed;
        // Unsubscribe
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
        // Update the total kills text.
        if (totalKillsText != null)
        {
            totalKillsText.text = "Total Kills: " + kills;
        }

        // Check if the cause of death is "Bullet".
        if (enemyHealth.lastKillCause == "Bullet")
        {
            bulletKills++;  // Increment the bullet kills counter.
            if (bulletKillsText != null)
            {
                bulletKillsText.text = "Bullet Kills: " + bulletKills;
            }
        }

        UnityEngine.Debug.Log("Total Kills Updated: " + kills);
        UnityEngine.Debug.Log("Bullet Kills Updated: " + bulletKills);

        if (kills == initialSpawns)
        {
            if (victoryUI != null)
            {
                victoryUI.SetActive(true);  // VICTORY AT LAST AAAAAAAAAAAAAAAAH.
                generalUI.SetActive(false);
            }
        }
    }

    private void HandleStructureDestroyed()
    {
        UnityEngine.Debug.Log("Structure destroyed triggering Game Over UI.");
        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        if (generalUI != null)
            generalUI.SetActive(false);
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

    /// <summary>
    /// Prefab UI calls end here.
    /// </summary>

    /// <summary>
    /// General UI P2, Switch to proper QR Scan.
    /// </summary>
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

    /// <summary>
    /// Switch for what game UI should show goes here.
    /// Sort out later, if time allows.
    /// </summary>
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