using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public GameObject PausePanel;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Pause()
    {

        PausePanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void Continue()
    {

        PausePanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void Load(int SceneIndex)
    {
        SceneManager.LoadScene(SceneIndex);
        Time.timeScale = 1;
    }
}
