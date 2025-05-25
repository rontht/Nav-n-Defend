using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionLoader : MonoBehaviour
{
    public GameObject Confirmation;
    public void PopUpConfirm(int SceneIndex)
    {
        SceneManager.LoadScene(SceneIndex);
        Confirmation.SetActive(false);
    }

    public void PopUpDecline()
    {
        Confirmation.SetActive(false);
    }

    public void ActivatePopup()
    {
        Confirmation.SetActive(true);
    }
}
