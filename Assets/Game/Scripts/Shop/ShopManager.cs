using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;
    
    [Header("Currency")]
    public int currentCurrency = 0;
    
    [Header("Shop Items")]
    public List<Shop_Item_Data> availableItems = new List<Shop_Item_Data>();
    
    [Header("UI References")]
    public TextMeshProUGUI currencyText;
    public GameObject purchaseConfirmationDialog;
    public TextMeshProUGUI confirmationText;
    
    public UnityEvent onCurrencyChanged;
    public UnityEvent onItemPurchased;

    private Shop_Item_Data pendingPurchaseItem;
    private const string CURRENCY_PREFS_KEY = "Currency";
    private const string PURCHASED_ITEMS_PREFS_KEY = "PurchasedItems";
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
            
        LoadData();
    }

    public void AddCurrency(int amount)
    {
        currentCurrency += amount;
        SaveCurrency();
        onCurrencyChanged?.Invoke();
        UpdateCurrencyText();
    }

    public void ShowPurchaseConfirmation(Shop_Item_Data item)
    {
        pendingPurchaseItem = item;
        confirmationText.text = $"Purchase {item.itemName} for {item.cost} coins?";
        purchaseConfirmationDialog.SetActive(true);
    }

    public void ConfirmPurchase()
    {
        if (pendingPurchaseItem != null && TryPurchaseItem(pendingPurchaseItem))
        {
            pendingPurchaseItem.isPurchased = true;
            SavePurchasedItems();
            onItemPurchased?.Invoke();
        }
        ClosePurchaseConfirmation();
    }

    public void ClosePurchaseConfirmation()
    {
        purchaseConfirmationDialog.SetActive(false);
        pendingPurchaseItem = null;
    }
    
    private bool TryPurchaseItem(Shop_Item_Data item)
    {
        if (currentCurrency >= item.cost)
        {
            currentCurrency -= item.cost;
            SaveCurrency();
            onCurrencyChanged?.Invoke();
            
            switch (item.type)
            {
                case Shop_Item_Data.ItemType.maxHP:
                    StatsManager.Instance.maxHP += item.value;
                    StatsManager.Instance.currentHP += item.value;
                    break;
                case Shop_Item_Data.ItemType.attack:
                    StatsManager.Instance.attack += item.value;
                    break;
                case Shop_Item_Data.ItemType.defense:
                    StatsManager.Instance.defense += item.value;
                    break;
            }

            SaveStats();
            return true;
        }
        return false;
    }
    
    private void UpdateCurrencyText()
    {
        if (currencyText != null)
            currencyText.text = $"Coins: {currentCurrency}";
    }
    
    private void SaveCurrency()
    {
        PlayerPrefs.SetInt(CURRENCY_PREFS_KEY, currentCurrency);
        PlayerPrefs.Save();
    }
    
    private void LoadData()
    {
        // Load currency (Default to 0 for new players)
        currentCurrency = PlayerPrefs.GetInt(CURRENCY_PREFS_KEY, 500);
        UpdateCurrencyText();
        
        // Load purchased items
        string purchasedItemsData = PlayerPrefs.GetString(PURCHASED_ITEMS_PREFS_KEY, "");
        if (!string.IsNullOrEmpty(purchasedItemsData))
        {
            string[] purchasedIds = purchasedItemsData.Split(',');
            foreach (var item in availableItems)
            {
                item.isPurchased = purchasedIds.Contains(item.id);
            }
        }

        
        LoadStats();
    }

    private void SavePurchasedItems()
    {
        string purchasedItemsData = string.Join(",", 
            availableItems.Where(item => item.isPurchased).Select(item => item.id));
        PlayerPrefs.SetString(PURCHASED_ITEMS_PREFS_KEY, purchasedItemsData);
        PlayerPrefs.Save();
    }

    private void SaveStats()
    {
        PlayerPrefs.SetInt("MaxHP", StatsManager.Instance.maxHP);
        PlayerPrefs.SetInt("Attack", StatsManager.Instance.attack);
        PlayerPrefs.SetInt("Defense", StatsManager.Instance.defense);
        PlayerPrefs.Save();
    }

    private void LoadStats()
    {
        StatsManager.Instance.maxHP = PlayerPrefs.GetInt("MaxHP", StatsManager.Instance.maxHP);
        StatsManager.Instance.attack = PlayerPrefs.GetInt("Attack", StatsManager.Instance.attack);
        StatsManager.Instance.defense = PlayerPrefs.GetInt("Defense", StatsManager.Instance.defense);
        StatsManager.Instance.currentHP = StatsManager.Instance.maxHP;
    }
}
