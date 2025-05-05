using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    
    [Header("Currency")]
    public int currentCurrency = 0;
    
    [Header("Shop Items")]
    public List<Shop_Item_Data> availableItems = new List<Shop_Item_Data>();
    
    public UnityEvent onCurrencyChanged;
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
            
        LoadCurrency();
    }
    
    public void AddCurrency(int amount)
    {
        currentCurrency += amount;
        SaveCurrency();
        onCurrencyChanged?.Invoke();
    }
    
    public bool TryPurchaseItem(Shop_Item_Data item)
    {
        if (currentCurrency >= item.cost)
        {
            currentCurrency -= item.cost;
            SaveCurrency();
            onCurrencyChanged?.Invoke();
            
         
            switch (item.type)
            {
                case Shop_Item_Data.ItemType.MaxHealth:
                    StatsManager.Instance.maxHP += item.value;
                    StatsManager.Instance.currentHP += item.value;
                    break;
                case Shop_Item_Data.ItemType.Damage:
                    StatsManager.Instance.attack += item.value;
                    break;
                case Shop_Item_Data.ItemType.Armor:
                    StatsManager.Instance.defense += item.value;
                    break;
            }
            
            return true;
        }
        return false;
    }
    
    private void SaveCurrency()
    {
        PlayerPrefs.SetInt("Currency", currentCurrency);
        PlayerPrefs.Save();
    }
    
    private void LoadCurrency()
    {
        currentCurrency = PlayerPrefs.GetInt("Currency", 0);
        onCurrencyChanged?.Invoke();
    }
}