using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{
    public TextMeshProUGUI statsText;

    void Update()
    {
        statsText.text = $"HP: {StatsManager.Instance.currentHP}/{StatsManager.Instance.maxHP}\n" +
                         $"Damage: {StatsManager.Instance.damage}\n" +
                         $"Armor: {StatsManager.Instance.armor}";
    }
}
