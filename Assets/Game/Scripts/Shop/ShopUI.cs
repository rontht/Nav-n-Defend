using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI currencyText;
    public Transform shopItemContainer;
    public GameObject shopItemPrefab;
    
    private void Start()
    {
        UpdateCurrencyText();
        PopulateShop();
        
     
        ShopManager.Instance.onCurrencyChanged.AddListener(UpdateCurrencyText);
    }
    
    private void UpdateCurrencyText()
    {
        currencyText.text = $"Currency: {ShopManager.Instance.currentCurrency}";
    }
    
    private void PopulateShop()
    {
        foreach (Shop_Item_Data item in ShopManager.Instance.availableItems)
        {
            GameObject itemGO = Instantiate(shopItemPrefab, shopItemContainer);
            ShopItemUI itemUI = itemGO.GetComponent<ShopItemUI>();
            itemUI.Initialize(item);
        }
    }
}