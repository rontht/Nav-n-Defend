using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class ShopItemUI : MonoBehaviour
{
    [Header("Item Configuration")]
    public Shop_Item_Data itemData;    [Header("UI Components")]
    public Image iconImage;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI costText;
    public Button purchaseButton;
    public Image itemBackground;
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable xrInteractable;
    
    [Header("Visual States")]
    public Color purchasedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    public Color defaultColor = Color.white;
    
    
    private void Awake()
    {
        if (xrInteractable != null)
        {
            xrInteractable.selectEntered.AddListener(OnXRSelectEntered);
        }

        if (purchaseButton != null)
        {
            purchaseButton.onClick.AddListener(OnPurchaseClicked);
        }
    }    private void Start()
    {
        if (itemData == null)
        {
            Debug.LogError("Shop Item Data not assigned to " + gameObject.name);
            return;
        }

        if (iconImage != null) 
        {
            iconImage.sprite = itemData.icon;
            iconImage.color = itemData.isPurchased ? purchasedColor : defaultColor;
        }

        if (descriptionText != null)
        {
            descriptionText.text = $"{itemData.description}\n+{itemData.value} {itemData.type}";
        }
        
        UpdateUI();
        
        ShopManager.Instance.onCurrencyChanged.AddListener(UpdateUI);
        ShopManager.Instance.onItemPurchased.AddListener(UpdateUI);
    }
    
    private void OnPurchaseClicked()
    {
        if (!itemData.isPurchased && ShopManager.Instance.currentCurrency >= itemData.cost)
        {
            ShopManager.Instance.ShowPurchaseConfirmation(itemData);
        }
    }

    private void OnXRSelectEntered(SelectEnterEventArgs args)
    {
        OnPurchaseClicked();
    }
    
    private void UpdateUI()
    {
        if (itemData.isPurchased)
        {
            if (costText != null) costText.text = "Purchased";
            if (purchaseButton != null) purchaseButton.interactable = false;
            if (iconImage != null) iconImage.color = purchasedColor;
            if (xrInteractable != null) xrInteractable.enabled = false;
        }
        else
        {
            if (costText != null) costText.text = $"Cost: {itemData.cost}";
            if (purchaseButton != null) 
                purchaseButton.interactable = ShopManager.Instance.currentCurrency >= itemData.cost;
            if (iconImage != null) iconImage.color = defaultColor;
            if (xrInteractable != null) xrInteractable.enabled = true;
        }
    }
    
    private void OnDestroy()
    {
        if (ShopManager.Instance != null)
        {
            ShopManager.Instance.onCurrencyChanged.RemoveListener(UpdateUI);
            ShopManager.Instance.onItemPurchased.RemoveListener(UpdateUI);
        }
    }
}
