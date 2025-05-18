using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPManager : MonoBehaviour
{
    [SerializeField] private Slider hpSlider;
    [SerializeField] private TMP_Text hpText;

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

        hpSlider.maxValue = PlayerStats.Instance.maxHP;
        hpSlider.value = PlayerStats.Instance.currentHP;

        if (hpText != null)
            hpText.text = $"HP: {PlayerStats.Instance.currentHP} / {PlayerStats.Instance.maxHP}";
    }
}
