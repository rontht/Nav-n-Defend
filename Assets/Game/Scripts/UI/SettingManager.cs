using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public void ResetButtonFunction()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.ResetStats();
        else
            Debug.Log("Player Stats Instance not found.");
    }

    public void LevelButtonFunction()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.GainExperience(10);
        else
            Debug.Log("Player Stats Instance not found.");
    }

    public void CashButtonFunction()
    {
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.AddCoins(10);
        else
            Debug.Log("Player Stats Instance not found.");
    }

    // add something like changing number of nodes and grid for puzzle game
}
