using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExpManager : MonoBehaviour
{
    [SerializeField] private Slider expSlider;
    [SerializeField] private TMP_Text levelText;

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

        int currentExp = PlayerStats.Instance.currentExp;
        int expToLevelUp = PlayerStats.Instance.expToLevelUp;
        int level = PlayerStats.Instance.level;

        expSlider.maxValue = expToLevelUp;
        expSlider.value = currentExp;
        levelText.text = $"Level: {level}";
    }
}
