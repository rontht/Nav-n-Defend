using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUIManager : MonoBehaviour
{
    [Header("UI Prefabs")]
    public Transform inventoryContent;
    public Transform contentPanel;
    public GameObject itemSlotPrefab;

    [Header("Item Data")]
    public List<Shop_Item_Data> allItems;

    private void Start()
    {
        PopulateInventory();
    }

    private void PopulateInventory()
    {
        // Get the list of purchased item IDs
        List<string> purchasedItemIDs = PlayerStats.Instance.GetPurchasedItemIDs();

        foreach (string itemId in purchasedItemIDs)
        {
            // Get item data by ID
            Shop_Item_Data item = allItems.Find(i => i.id == itemId);
            if (item == null)
            {
                Debug.LogWarning($"Item data not found for ID: {itemId}");
                continue;
            }

            // Instantiate the item slot
            GameObject itemSlot = Instantiate(itemSlotPrefab, inventoryContent);

            // Get references to UI components within the prefab
            Image icon = itemSlot.transform.Find("IconPanel/Icon").GetComponent<Image>();
            TextMeshProUGUI nameText = itemSlot.transform.Find("Name").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI bonusText = itemSlot.transform.Find("Bonus").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI countText = itemSlot.transform.Find("CountPanel/Count").GetComponent<TextMeshProUGUI>();

            // Set the item data
            icon.sprite = item.icon;
            nameText.text = item.itemName;
            bonusText.text = $"{item.type}: +{item.value}";

            int itemCount = PlayerStats.Instance.GetOwnedItemCount(itemId);
            countText.text = $"x{itemCount}";

            Button useButton = itemSlot.transform.Find("UseButton").GetComponent<Button>();
            useButton.onClick.AddListener(() => UseItem(item));
        }
    }

    private void UseItem(Shop_Item_Data item)
    {
        if (item == null)
        {
            Debug.LogError("Attempted to use a null item! This should not happen if PopulateInventory is working correctly.");
            return;
        }

        Debug.Log($"Used {item.itemName}. Applying {item.type} +{item.value} bonus.");
        // Shop_Item_Data item
        // Apply the item's stat bonus
        PlayerStats.Instance.IncreaseStat(item.type, item.value);
        UISoundPlayer.Instance.PlayCashSound();
        // Optionally reduce the item count or remove it from inventory if it is consumable
        // You can add more logic here if needed (e.g., consuming potions)

        // Refresh inventory UI to reflect the change
        ClearInventoryUI();
        PopulateInventory();
    }

    private void ClearInventoryUI()
    {
        foreach (Transform child in inventoryContent)
        {
            Destroy(child.gameObject);
        }
    }
}
