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
        foreach (Shop_Item_Data item in allItems)
        {
            // Check if the item is purchased
            if (PlayerStats.Instance.HasPurchasedItem(item.id))
            {
                // Instantiate the item slot
                GameObject itemSlot = Instantiate(itemSlotPrefab, inventoryContent);

                // Get references to UI components within the prefab
                Image icon = itemSlot.transform.Find("IconPanel/Icon").GetComponent<Image>();
                TextMeshProUGUI nameText = itemSlot.transform.Find("IconPanel/Name").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI bonusText = itemSlot.transform.Find("Bonus").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI countText = itemSlot.transform.Find("CountPanel/Count").GetComponent<TextMeshProUGUI>();

                // Set the item data
                icon.sprite = item.icon;
                nameText.text = item.itemName;
                bonusText.text = $"{item.type}: +{item.value}";
                
                // Get the correct count from PlayerStats
                // int itemCount = PlayerStats.Instance.GetItemQuantity(item.id);
                // countText.text = itemCount.ToString();

                // Optional: Add click handler for item use (e.g., equipping or consuming)
                Button itemButton = itemSlot.GetComponent<Button>();
                itemButton.onClick.AddListener(() => UseItem(item));
            }
        }
    }

    private void UseItem(Shop_Item_Data item)
    {
        // Apply the item's stat bonus
        PlayerStats.Instance.IncreaseStat(item.type, item.value);
        
        // Optionally reduce the item count or remove it from inventory if it is consumable
        // You can add more logic here if needed (e.g., consuming potions)

        Debug.Log($"Used {item.itemName}. Applied {item.type} +{item.value}");
        
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
