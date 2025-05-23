using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using System.Diagnostics;

/// <summary>
/// This seriously needs to be refactored at some point.
/// If time allows, I'll do so but deadlines are tight. For now, it works.
/// Refactor would include GameUIManager, GameStateManager, SetTrackers etc
/// </summary>

public class imageTrackingToggle : MonoBehaviour
{
    private int initialSpawns = 0;
    private int bulletKills = 0;
    private int shotsMade = 0;
    private int shotsHit = 0;
    private int coinsEarned = 0;
    private int expEarned = 0;
    private int hpLost = 10;
    private int firstStepHP;

    // Splitting of these elements for visual clarity
    // via headers.

    [Header("AR Tracking")]
    public ARTrackedImageManager imageManager;

    [Header("UI Elements")]
    public GameObject generalUI;
    public GameObject scanMenu;
    public GameObject qrScanMenu;
    public GameObject countdownHold;
    public TMP_Text countdownText;
    public TMP_Text remainingSpawnsText;
    public TMP_Text bulletKillsText;
    public TMP_Text structureHpText;
    public Slider hpSlider;
    // public TMP_Text totalKillsText;

    [Header("Shared Stats Panel")]
    public GameObject endPanel;
    public TMP_Text statsShotsMadeText;
    public TMP_Text statsShotsHitText;
    public TMP_Text statsAccuracyText;
    public TMP_Text expEarnedText;
    public TMP_Text coinsEarnedText;
    public TMP_Text hpLostText;

    public TMP_Text resultMessageText;

    // Utilized for subscribing, required for prefab referencing.
    public trackedImageSpawnManager spawnManager;

    private Coroutine countdown;

    /// <summary>
    /// General UI Part 1, logic shared for start. When merging games onto same scene, utilize calls here.
    /// Call your UI and state false, the only one here to be true should be scanMenu.
    /// </summary>
    void Start()
    {

        firstStepHP = PlayerStats.Instance.currentHP;

        UnityEngine.Debug.Log("Max HP at Start: " + firstStepHP);
        /// I should note that while I'm aware of ?.,
        /// this is a bit more clear to everyone 
        /// as this is our first time with Unity / C#.

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

        if (endPanel != null)
        {
            endPanel.SetActive(false);
        }

        if (bulletKillsText != null)
        {
            bulletKillsText.text = "Bullet Kills: 0";
        }

        enemyHealth.OnKill += handleEnemyKill;

    }

    /// <summary>
    /// Subscriptions to prefab events start here.
    /// </summary>
    void OnEnable()
    {
        trackedImageSpawnManager.OnSpawnManagerReady += handleSpawnManagerReady;
        centerTriggerDamage.onStructureDestroyed += handleStructureDestroyed;
        centerTriggerDamage.onStructureDamaged += handleStructureHpUpdate; // tracking HP
        shooting.OnShotFired += handleShotFired;
        enemyHealth.OnEnemyHit += handleEnemyHit;
    }

    void OnDisable()
    {
        trackedImageSpawnManager.OnSpawnManagerReady -= handleSpawnManagerReady;
        centerTriggerDamage.onStructureDestroyed -= handleStructureDestroyed;
        centerTriggerDamage.onStructureDamaged -= handleStructureHpUpdate;
        shooting.OnShotFired -= handleShotFired;
        enemyHealth.OnEnemyHit -= handleEnemyHit;
        enemyHealth.OnKill -= handleEnemyKill;
    }

    private void handleSpawnManagerReady(trackedImageSpawnManager manager)
    {
        spawnManager = manager;

        if (spawnManager != null)
        {
            initialSpawns = spawnManager.remainingSpawns;
            UnityEngine.Debug.Log("Initial Spawns Set To: " + initialSpawns);
        }

        UpdateRemainingSpawnsText();
    }


    private void handleStructureHpUpdate(int currentHp)
    {
        if (structureHpText != null)
        {
            structureHpText.text = $"HP: {currentHp} / {PlayerStats.Instance.maxHP}";
        }

        if (hpSlider != null)
        {
            hpSlider.maxValue = PlayerStats.Instance.maxHP;
            hpSlider.value = currentHp;
        }
    }

    private void handleEnemyKill(int kills)
    {
        // if (totalKillsText != null)
        // {
        //     totalKillsText.text = "Total Kills: " + kills;
        // }

        // Did it die by a bullet?
        if (enemyHealth.lastKillCause == "Bullet")
        {
            bulletKills++;  // If so, +1
            if (bulletKillsText != null)
            {
                bulletKillsText.text = "Bullet Kills: " + bulletKills;
            }
        }

        if (kills == initialSpawns)
        {
            ShowEndUI(true);
        }
    }

    private void handleStructureDestroyed()
    {
        ShowEndUI(false);
    }

    private void ShowEndUI(bool isVictory)
    {
        if (endPanel != null)
        {
            endPanel.SetActive(true);
        }

        if (generalUI != null)
        {
            generalUI.SetActive(false);
        }

        if (resultMessageText != null)
        {
            resultMessageText.text = isVictory ? "You Won!" : "You Lost!";
            if (isVictory)
            {
                UISoundPlayer.Instance.PlayVictorySound();
            }
            else
            {
                UISoundPlayer.Instance.PlayDefeatSound();
            }
        }

        if (isVictory == true)
        {
            coinsEarned = 50;
            expEarned = 5;
            // PlayerStats.Instance.TakeDamage(10);
        }
        else
        {
            coinsEarned = 30;
            expEarned = 3;
            // PlayerStats.Instance.TakeDamage(10);
        }
        Time.timeScale = 0f;

        PlayerStats.Instance.AddCoins(coinsEarned);
        PlayerStats.Instance.GainExperience(expEarned);
        PlayerStats.Instance.TakeDamage(hpLost);

        // Rewards for clear probably go into this method.
        UpdateStatsSummary();
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
    /// General UI P2, Switch to proper QR Scan.
    /// </summary>
    public void OnSurfaceFoundPressed()
    {
        UISoundPlayer.Instance.PlayForwardClickSound();
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
        UISoundPlayer.Instance.PlayForwardClickSound();
        if (imageManager != null)
        {
            imageManager.enabled = true;
        }

        if (qrScanMenu != null)
        {
            qrScanMenu.SetActive(false);
        }

        if (countdown == null)
        {
            if (countdownHold != null)
            {
                countdownHold.SetActive(true);
            }
            countdown = StartCoroutine(startCountdown(5)); // 5 going down
            UnityEngine.Debug.Log("Remaining Spawns: " + initialSpawns);
        }
    }

    private IEnumerator startCountdown(int seconds)
    {
        int current = seconds;

        while (current > 0)
        {
            if (countdownText != null)
            {
                countdownText.text = current.ToString();
                UISoundPlayer.Instance.PlayCountdownTickSound();
            }
            yield return new WaitForSeconds(1f);
            current--;
        }

        yield return new WaitForSeconds(0.5f);
        UISoundPlayer.Instance.PlayGameStartSound();
        if (countdownHold != null)
        {
            countdownHold.SetActive(false);
        }

        if (generalUI != null)
        {
            generalUI.SetActive(true);
        }

        countdown = null;
    }

    private void handleShotFired()
    {
        shotsMade++;
        //UpdateInGameAccuracyUI();
    }

    private void handleEnemyHit()
    {
        shotsHit++;
        //UpdateInGameAccuracyUI();
    }

    private void UpdateStatsSummary()
    {
        if (statsShotsMadeText != null)
            statsShotsMadeText.text = "Shots Made: " + shotsMade;

        if (statsShotsHitText != null)
            statsShotsHitText.text = "Shots Hit: " + shotsHit;

        if (statsAccuracyText != null)
        {
            float accuracy = shotsMade > 0 ? ((float)shotsHit / shotsMade) * 100f : 0f;
            statsAccuracyText.text = $"Accuracy: {accuracy:F1}%";
        }

        if (expEarnedText != null)
        {
            expEarnedText.text = "EXP Earned: " + expEarned;
        }

        if (coinsEarnedText != null)
            coinsEarnedText.text = "Coins Earned: " + coinsEarned;

        if (hpLostText != null)
            hpLostText.text = "HP Lost: " + hpLost;
    }
}