using UnityEngine;

public class SettingManager : MonoBehaviour
{
    public void ResetButtonFunction()
    {
        if (UISoundPlayer.Instance != null)
            UISoundPlayer.Instance.PlayForwardClickSound();
        else
            Debug.Log("UISoundPlayer Instance not found.");
        
        if (PlayerStats.Instance != null)
            PlayerStats.Instance.ResetStats();
        else
            Debug.Log("PlayerStats Instance not found.");
    }

    public void LevelButtonFunction()
    {
        if (UISoundPlayer.Instance != null)
            UISoundPlayer.Instance.PlayVictorySound();
        else
            Debug.Log("UISoundPlayer Instance not found.");

        if (PlayerStats.Instance != null)
            PlayerStats.Instance.GainExperience(10);
        else
            Debug.Log("PlayerStats Instance not found.");
    }

    public void CashButtonFunction()
    {
        if (UISoundPlayer.Instance != null)
            UISoundPlayer.Instance.PlayCashSound();
        else
            Debug.Log("UISoundPlayer Instance not found.");

        if (PlayerStats.Instance != null)
            PlayerStats.Instance.AddCoins(10);
        else
            Debug.Log("Player Stats Instance not found.");
    }

    // add something like changing number of nodes and grid for puzzle game
}
