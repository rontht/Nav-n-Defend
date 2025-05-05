using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI Components")]
    public Image iconImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI costText;
    public Button purchaseButton;
    
    private Shop_Item_Data itemData;
    
    public void Initialize(Shop_Item_Data data)
    {
        itemData = data;
        
    
        if (iconImage != null) iconImage.sprite = data.icon;
        if (itemNameText != null) itemNameText.text = data.itemName;
        if (descriptionText != null) descriptionText.text = data.description;
        if (costText != null) costText.text = $"Cost: {data.cost}";
        
      
        if (purchaseButton != null)
        {
            purchaseButton.onClick.AddListener(OnPurchaseClicked);
            UpdateButtonInteractable();
            ShopManager.Instance.onCurrencyChanged.AddListener(UpdateButtonInteractable);
        }
    }
    
    private void OnPurchaseClicked()
    {
        if (ShopManager.Instance.TryPurchaseItem(itemData))
        {
    
            Debug.Log($"Purchased {itemData.itemName}!");
        }
        else
        {
       
            Debug.Log("Not enough currency!");
        }
    }
    
    private void UpdateButtonInteractable()
    {
        purchaseButton.interactable = ShopManager.Instance.currentCurrency >= itemData.cost;
    }
    
    private void OnDestroy()
    {
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.onCurrencyChanged.RemoveListener(UpdateButtonInteractable);
        }
    }
}