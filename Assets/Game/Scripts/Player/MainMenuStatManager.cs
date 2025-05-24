using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenuStatManager : MonoBehaviour
{
    [SerializeField] private TMP_Text attackText;
    [SerializeField] private TMP_Text cashText;

    private void Start()
    {
        if (PlayerStats.Instance != null)
        {
            UpdateUI();
            PlayerStats.Instance.onStatsChanged += UpdateUI;
        }
    }

    private void OnDestroy()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.onStatsChanged -= UpdateUI;
        }
    }

    private void UpdateUI()
    {
        if (PlayerStats.Instance == null) return;

        if (attackText != null)
            attackText.text = $"{PlayerStats.Instance.attack}";
        if (cashText != null)
            cashText.text = $"{PlayerStats.Instance.coins}";
    }
}
