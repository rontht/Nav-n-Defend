using UnityEngine;
using TMPro;

/// <summary>
/// Displays player statistics in the UI:
/// - Shows current/max HP
/// - Shows attack power
/// - Shows defense rating
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
    /// - Index 2: Defense display
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
        UpdateDefense();
    }

    /// <summary>
    /// Updates the HP display with current and maximum health values.
    /// Uses the first slot (index 0) in the statsSlots array.
    /// </summary>
    public void UpdateHP()
    {
        Transform valuePanel = statsSlots[0].transform.Find("ValuePanel");
        TMP_Text valueText = valuePanel.Find("Value").GetComponent<TMP_Text>();
        valueText.text = PlayerStats.Instance.currentHP + " / " + PlayerStats.Instance.maxHP;
    }    
    /// <summary>
    /// Updates the Attack stat display.
    /// Uses the second slot (index 1) in the statsSlots array.
    /// </summary>
    public void UpdateAttack()
    {
        Transform valuePanel = statsSlots[1].transform.Find("ValuePanel");
        TMP_Text valueText = valuePanel.Find("Value").GetComponent<TMP_Text>();
        valueText.text = "" + PlayerStats.Instance.attack;
    }

    public void UpdateDefense()
    {
        Transform valuePanel = statsSlots[2].transform.Find("ValuePanel");
        TMP_Text valueText = valuePanel.Find("Value").GetComponent<TMP_Text>();
        valueText.text = "" + PlayerStats.Instance.defense;
    }
}
