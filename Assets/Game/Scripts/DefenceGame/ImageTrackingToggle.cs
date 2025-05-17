using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using System.Collections;
using System.Collections.Generic;
using TMPro;

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

    private Coroutine countdownCoroutine;

    // Menu States upon Scene Load for layered safety.
    /// If I have time, look into dictionary handling.
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
    }

    // Called when the Surface Found button is pressed.
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

    // Called when the Scan button is pressed.
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

        // Countdown
        while (current > 0)
        {
            if (countdownText != null)
            {
                countdownText.text = current.ToString();
            }
            yield return new WaitForSeconds(1f);
            current--;
        }

        yield return new WaitForSeconds(0.5f); // Pause before showing next UI and hiding self.

        // Hide the countdown UI after the countdown.
        if (countdownHold != null)
        {
            countdownHold.SetActive(false);
        }

        if (generalUI != null)
        {
            generalUI.SetActive(true);
        }

        countdownCoroutine = null; // Related to timer reset.
    }
}