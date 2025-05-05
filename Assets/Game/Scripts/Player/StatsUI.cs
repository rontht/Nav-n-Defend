using UnityEngine;
using TMPro;

public class StatsUI : MonoBehaviour
{
    public GameObject[] statsSlots;

    void Start()
    {
        UpdateHP();
        UpdateAttack();
        UpdateDefense();
    }
    public void UpdateHP()
    {
        Transform valuePanel = statsSlots[0].transform.Find("ValuePanel");
        TMP_Text valueText = valuePanel.Find("Value").GetComponent<TMP_Text>();
        valueText.text = StatsManager.Instance.currentHP + " / " + StatsManager.Instance.maxHP;
    }

    public void UpdateAttack()
    {
        Transform valuePanel = statsSlots[1].transform.Find("ValuePanel");
        TMP_Text valueText = valuePanel.Find("Value").GetComponent<TMP_Text>();
        valueText.text = "" + StatsManager.Instance.attack;
    }

    public void UpdateDefense()
    {
        Transform valuePanel = statsSlots[2].transform.Find("ValuePanel");
        TMP_Text valueText = valuePanel.Find("Value").GetComponent<TMP_Text>();
        valueText.text = "" + StatsManager.Instance.defense;
    }
}
