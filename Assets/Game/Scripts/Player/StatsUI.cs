using UnityEngine;
using TMPro;

/// Displays player statistics in the UI.
public class StatsUI : MonoBehaviour
{
    /// Array of UI slots for each stat.
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
    /// Updates all stat displays.
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

    /// Updates the HP display.
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
    /// Updates the Attack stat display.
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
