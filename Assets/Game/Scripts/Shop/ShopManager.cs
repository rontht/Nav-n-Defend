using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using TMPro;

/// <summary>
/// Manages the in-game shop system, handling:
/// - Purchase confirmation flow
/// - Available items list
/// - Integration with PlayerStats for purchases
/// 
/// This manager coordinates between ShopItemUI components and the PlayerStats
/// system to process purchases and update the game state.
/// </summary>
public class ShopManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the shop manager
    /// </summary>
    public static ShopManager Instance;
    
    [Header("Shop Items")]
    public List<Shop_Item_Data> availableItems = new List<Shop_Item_Data>();
    
    [Header("UI References")]
    public GameObject purchaseConfirmationDialog;
    public TextMeshProUGUI confirmationText;
    
    public UnityEvent onItemPurchased;

    private Shop_Item_Data pendingPurchaseItem;
      private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }    
    /// <summary>
    /// Displays the purchase confirmation dialog for an item.
    /// Shows the item name and cost, and stores the item for the confirmation callback.
    /// </summary>
    /// <param name="item">The shop item the player wants to purchase</param>
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
            onItemPurchased?.Invoke();
        }
        ClosePurchaseConfirmation();
    }

    public void ClosePurchaseConfirmation()
    {
        purchaseConfirmationDialog.SetActive(false);
        pendingPurchaseItem = null;
    }    
    /// <summary>
    /// Attempts to purchase an item by:
    /// 1. Checking if player can afford it
    /// 2. Spending the coins
    /// 3. Applying the item's stat boost
    /// 4. Marking the item as purchased
    /// </summary>
    /// <param name="item">The item to purchase</param>
    /// <returns>True if purchase was successful, false otherwise</returns>
    private bool TryPurchaseItem(Shop_Item_Data item)
    {
        if (PlayerStats.Instance.CanAfford(item.cost))
        {
            PlayerStats.Instance.SpendCoins(item.cost);
            PlayerStats.Instance.IncreaseStat(item.type, item.value);
            PlayerStats.Instance.MarkItemAsPurchased(item.id);
            return true;
        }
        return false;
    }
}
