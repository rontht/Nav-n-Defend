using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using TMPro;

/// Manages the in-game shop system.
public class ShopManager : MonoBehaviour
{
    /// Singleton instance of the shop manager.
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
    /// Displays the purchase confirmation dialog for an item.
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

    private bool TryPurchaseItem(Shop_Item_Data item)
    {
        if (PlayerStats.Instance.CanAfford(item.cost))
        {
            // Check max player owns limit
            if (item.maxPlayerOwns != -1 && PlayerStats.Instance.GetOwnedItemCount(item.id) >= item.maxPlayerOwns)
            {
                Debug.Log($"Cannot purchase {item.itemName}. Max owned limit reached.");
                return false;
            }

            PlayerStats.Instance.SpendCoins(item.cost);
            // PlayerStats.Instance.IncreaseStat(item.type, item.value);

            // Only mark as purchased if it's not a consumable or if it's within limits
            if (item.type != ItemType.Heal) // Temp items are consumable and not marked as purchased
            {
                 PlayerStats.Instance.MarkItemAsPurchased(item.id);
            }
            else
            {
                 PlayerStats.Instance.MarkItemAsPurchased(item.id); // Will increment the count
            }
            return true;
        }
        return false;
    }
}
