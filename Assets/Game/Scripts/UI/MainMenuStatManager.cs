using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuStatManager : MonoBehaviour
{
    [Header("Cash Stat")]
    [SerializeField] private TMP_Text cashText;

    [Header("Attack Stat")]
    [SerializeField] private TMP_Text attackText;

    [Header("Experience Stat")]
    [SerializeField] private Slider expSlider;
    [SerializeField] private TMP_Text levelText;

    [Header("HP Stat")]
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

        // Attack Stat
        if (attackText != null)
            attackText.text = $"{PlayerStats.Instance.attack}";

        // Cash Stat
        if (cashText != null)
            cashText.text = $"{PlayerStats.Instance.coins}";

        // Experience Stat
        int currentExp = PlayerStats.Instance.currentExp;
        int expToLevelUp = PlayerStats.Instance.expToLevelUp;
        int level = PlayerStats.Instance.level;
        expSlider.maxValue = expToLevelUp;
        expSlider.value = currentExp;
        levelText.text = $"Level: {level}";

        // HP Stat
        hpSlider.maxValue = PlayerStats.Instance.maxHP;
        hpSlider.value = PlayerStats.Instance.currentHP;
        if (hpText != null)
            hpText.text = $"HP: {PlayerStats.Instance.currentHP} / {PlayerStats.Instance.maxHP}";
    }
}
