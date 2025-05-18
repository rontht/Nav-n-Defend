using UnityEngine;
using TMPro;

/// <summary>
/// Displays player statistics in the UI:
/// - Shows current/max HP
/// - Shows attack power
/// - Updates automatically when stats change
/// 
/// Each stat should have its own UI slot in the inspector array,
/// with ValuePanel/Value text components in the hierarchy.
/// </summary>
public class StatsUI : MonoBehaviour
{
    /// <summary>
    /// Array of UI slots for each stat:
    /// - Index 0: HP display
    /// - Index 1: Attack display
    /// Each slot should have a ValuePanel/Value text hierarchy
    /// </summary>
    public GameObject[] statsSlots;

    void Start()
    {
        UpdateAllStats();
        
        // Subscribe to stat changes
        PlayerStats.Instance.onStatsChanged += UpdateAllStats;
    }

    void OnDestroy()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.onStatsChanged -= UpdateAllStats;
        }
    }    
    /// <summary>
    /// Updates all stat displays in response to stat changes.
    /// Called automatically when any player stat changes.
    /// </summary>
    private void UpdateAllStats()
    {
        UpdateHP();
        UpdateAttack();
        
        // Adjust array length check if Defense slot is removed
        if (statsSlots.Length > 2 && statsSlots[2] != null)
        {
            statsSlots[2].SetActive(false); // Optionally hide the defense slot
        }
    }

    /// <summary>
    /// Updates the HP display with current and maximum health values.
    /// Uses the first slot (index 0) in the statsSlots array.
    /// </summary>
    public void UpdateHP()
    {
        if (statsSlots.Length > 0 && statsSlots[0] != null)
        {
            Transform valuePanel = statsSlots[0].transform.Find("ValuePanel");
            if (valuePanel != null)
            {
                TMP_Text valueText = valuePanel.Find("Value").GetComponent<TMP_Text>();
                if (valueText != null && PlayerStats.Instance != null)
                {
                    valueText.text = PlayerStats.Instance.currentHP + " / " + PlayerStats.Instance.maxHP;
                }
            }
        }
    }    
    /// <summary>
    /// Updates the Attack stat display.
    /// Uses the second slot (index 1) in the statsSlots array.
    /// </summary>
    public void UpdateAttack()
    {
        if (statsSlots.Length > 1 && statsSlots[1] != null)
        {
            Transform valuePanel = statsSlots[1].transform.Find("ValuePanel");
            if (valuePanel != null)
            {
                TMP_Text valueText = valuePanel.Find("Value").GetComponent<TMP_Text>();
                if (valueText != null && PlayerStats.Instance != null)
                {
                    valueText.text = "" + PlayerStats.Instance.attack;
                }
            }
        }
    }
}
