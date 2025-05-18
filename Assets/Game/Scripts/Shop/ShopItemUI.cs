using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

/// Controls the UI for individual shop items.
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
    }    private void Start()
    {
        if (itemData == null)
        {
            Debug.LogError("Shop Item Data not assigned to " + gameObject.name);
            return;
        }

        InitializeUI();
        UpdateUI();
        
        if (purchaseButton != null)
        {
            purchaseButton.onClick.AddListener(OnPurchaseClicked);
        }
        else
        {
            Debug.LogWarning("Purchase Button reference is missing on " + gameObject.name + ", cannot add onClick listener.");
        }
        
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.onCoinsChanged += UpdateUI;
            PlayerStats.Instance.onStatsChanged += UpdateUI;
        }
        else
        {
            Debug.LogWarning("PlayerStats.Instance is null, cannot add listeners");
        }
    }    /// Sets up the initial UI state for the shop item.
    private void InitializeUI()
    {
        if (iconImage != null) 
        {
            iconImage.sprite = itemData.icon;
            iconImage.color = itemData.isPurchased ? purchasedColor : defaultColor;
        }

        if (descriptionText != null)
        {
            descriptionText.text = $"{itemData.description}\n+{itemData.value} {itemData.type}";
        }
    }    
    /// Handles the purchase button click or XR selection.
    public void OnPurchaseClicked()
    {
        Debug.Log($"OnPurchaseClicked for item: {itemData?.itemName ?? "NULL ITEM"}");

        if (itemData == null)
        {
            Debug.LogError("ItemData is null!");
            return;
        }
        if (ShopManager.Instance == null)
        {
            Debug.LogError("ShopManager.Instance is null!");
            return;
        }

        if (!itemData.isPurchased && PlayerStats.Instance.CanAfford(itemData.cost))
        {
            ShopManager.Instance.ShowPurchaseConfirmation(itemData);
        }
        else
        {
            Debug.LogWarning($"Conditions NOT MET for purchase. isPurchased={itemData.isPurchased}, currentCoins={PlayerStats.Instance.coins}, cost={itemData.cost}");
        }
    }

    private void OnXRSelectEntered(SelectEnterEventArgs args)
    {
        OnPurchaseClicked();
    }    
    /// Updates the item's UI state.
    private void UpdateUI()
    {
        if (itemData == null) return;

        if (itemData.isPurchased)
        {
            if (costText != null) costText.text = "Purchased";
            if (purchaseButton != null) purchaseButton.interactable = false;
            if (iconImage != null) iconImage.color = purchasedColor;
            if (xrInteractable != null) xrInteractable.enabled = false;
        }
        else
        {
            if (costText != null) costText.text = $"{itemData.cost} Coins";
            if (purchaseButton != null && PlayerStats.Instance != null)
                purchaseButton.interactable = PlayerStats.Instance.CanAfford(itemData.cost);
            else if (purchaseButton != null)
                purchaseButton.interactable = false;

            if (iconImage != null) iconImage.color = defaultColor;
            if (xrInteractable != null) xrInteractable.enabled = true;
        }
    }
      private void OnDestroy()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.onCoinsChanged -= UpdateUI;
            PlayerStats.Instance.onStatsChanged -= UpdateUI;
        }
        if (xrInteractable != null)
        {
            xrInteractable.selectEntered.RemoveListener(OnXRSelectEntered);
        }
        
        if (purchaseButton != null)
        {
            purchaseButton.onClick.RemoveListener(OnPurchaseClicked);
        }
    }
}
