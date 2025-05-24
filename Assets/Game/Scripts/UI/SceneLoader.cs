using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void Load(int SceneIndex)
    {
        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f; // Unpause before loading a new scene.
        }

        SceneManager.LoadScene(SceneIndex);
        UISoundPlayer.Instance.PlayForwardClickSound();
    }
}
