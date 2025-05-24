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
    }    /// Sets the initial UI state for the shop item.
    private void InitializeUI()
    {
        if (iconImage != null) 
        {
            iconImage.sprite = itemData.icon;
            iconImage.color = itemData.isPurchased ? purchasedColor : defaultColor;
        }

        if (descriptionText != null)
        {
            string typeText = itemData.type.ToString();
            if (itemData.type == ItemType.Heal)
            {
                typeText = "HP Potion";
            }
            descriptionText.text = $"{itemData.description}\n+{itemData.value} {typeText}";
        }
    }    
    /// Handles purchase button click.
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

        // Allow showing confirmation for Temp items even if purchased
        // as long as the player can afford it and it's not past the max ownership limit.
        bool canShowConfirmation = PlayerStats.Instance.CanAfford(itemData.cost);
        if (itemData.type != ItemType.Heal && itemData.isPurchased)
        {
            canShowConfirmation = false; // For non-Temp items, don't show if already purchased.
        }
        
        // Further check for maxPlayerOwns limit for all items
        if (itemData.maxPlayerOwns != -1 && PlayerStats.Instance.GetOwnedItemCount(itemData.id) >= itemData.maxPlayerOwns)
        {
            canShowConfirmation = false;
            Debug.LogWarning($"Max owned limit reached for {itemData.itemName}. Cannot purchase more.");
        }


        if (canShowConfirmation)
        {
            ShopManager.Instance.ShowPurchaseConfirmation(itemData);
        }
        else
        {
            Debug.LogWarning($"Conditions NOT MET for purchase confirmation. Item: {itemData.itemName}, isPurchased (for non-Temp): {itemData.isPurchased}, currentCoins: {PlayerStats.Instance.coins}, cost: {itemData.cost}, ownedCount: {PlayerStats.Instance.GetOwnedItemCount(itemData.id)}, maxOwns: {itemData.maxPlayerOwns}");
        }
    }

    private void OnXRSelectEntered(SelectEnterEventArgs args)
    {
        OnPurchaseClicked();
    }    
    /// Updates the item's UI.
    private void UpdateUI()
    {
        if (itemData == null || PlayerStats.Instance == null) return;

        int ownedCount = PlayerStats.Instance.GetOwnedItemCount(itemData.id);
        bool canAfford = PlayerStats.Instance.CanAfford(itemData.cost);

        if (itemData.type == ItemType.Heal)
        {
            if (itemData.maxPlayerOwns != -1 && ownedCount >= itemData.maxPlayerOwns)
            {
                // Max owned for Temp item
                if (costText != null) costText.text = "Maxed Out";
                if (purchaseButton != null) purchaseButton.interactable = false;
                if (iconImage != null) iconImage.color = purchasedColor;
                if (xrInteractable != null) xrInteractable.enabled = false;
            }
            else
            {
                // Can still purchase Temp item
                if (costText != null) costText.text = $"{itemData.cost} Coins";
                if (purchaseButton != null) purchaseButton.interactable = canAfford;
                if (iconImage != null) iconImage.color = defaultColor;
                if (xrInteractable != null) xrInteractable.enabled = true;
            }
        }
        else // For non-Temp items (HP, attack, etc.)
        {
            if (itemData.isPurchased) // This implies it's a non-Temp item that has been bought once
            {
                if (costText != null) costText.text = "Purchased";
                if (purchaseButton != null) purchaseButton.interactable = false;
                if (iconImage != null) iconImage.color = purchasedColor;
                if (xrInteractable != null) xrInteractable.enabled = false;
            }
            else
            {
                if (costText != null) costText.text = $"{itemData.cost} Coins";
                if (purchaseButton != null) purchaseButton.interactable = canAfford;
                if (iconImage != null) iconImage.color = defaultColor;
                if (xrInteractable != null) xrInteractable.enabled = true;
            }
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
