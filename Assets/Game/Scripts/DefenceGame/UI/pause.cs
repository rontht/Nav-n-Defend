using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// This should work globally and be reused.
/// Ensure everything involves timeScale in its movement.
/// </summary>

public class pause : MonoBehaviour
{

    public GameObject pausePanel;

    public void Pause()
    {
        UISoundPlayer.Instance.PlayPauseSound();
        pausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void Continue()
    {
        UISoundPlayer.Instance.PlayForwardClickSound();
        pausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void Load(int SceneIndex)
    {
        UISoundPlayer.Instance.PlayBackwardClickSound();
        SceneManager.LoadScene(SceneIndex);
        Time.timeScale = 1;
    }
}