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

    [Header("Equipment Panels")]
    public GameObject hpEquipmentPanel;
    public GameObject attackEquipmentPanel;

    private void Start()
    {
        PopulateInventory();
        PopulateEquipment();
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
        if (PlayerStats.Instance.GetOwnedItemCount(item.id) <= 0)
        {
            Debug.LogWarning($"Cannot use {item.itemName}. Player does not own any more of this item.");
            RefreshInventoryUI();
            return;
        }

        if (item.type == ItemType.Heal)
        {
            // One-time healing effect
            PlayerStats.Instance.Heal(item.value);
            PlayerStats.Instance.DecreaseItemCount(item.id, 1);
            UISoundPlayer.Instance.PlayCashSound();
        }
        else
        {
            // Equip the item (e.g., HP or attack boost)
            PlayerStats.Instance.EquipItem(item);
            PlayerStats.Instance.DecreaseItemCount(item.id, 1);
            UISoundPlayer.Instance.PlayCashSound();
            PopulateEquipment();
        }

        // Refresh inventory UI to reflect the change
        RefreshInventoryUI();
    }

    private void ClearInventoryUI()
    {
        foreach (Transform child in inventoryContent)
        {
            Destroy(child.gameObject);
        }
    }

    private void RefreshInventoryUI()
    {
        ClearInventoryUI();
        PopulateInventory();
    }

    public void PopulateEquipment()
    {
        Dictionary<ItemType, Shop_Item_Data> equippedItems = PlayerStats.Instance.GetEquippedItemIDs();
        Debug.Log("Equipped Items Count: " + equippedItems.Count);
        // Handle HP Equipment
        if (equippedItems.TryGetValue(ItemType.HP, out Shop_Item_Data hpItem))
        {
            UpdateEquipmentPanel(hpEquipmentPanel, hpItem);
        }
        else
        {
            ClearEquipmentPanel(hpEquipmentPanel);
        }

        // Handle Attack Equipment
        if (equippedItems.TryGetValue(ItemType.attack, out Shop_Item_Data atkItem))
        {
            UpdateEquipmentPanel(attackEquipmentPanel, atkItem);
        }
        else
        {
            ClearEquipmentPanel(attackEquipmentPanel);
        }
    }

    private void UpdateEquipmentPanel(GameObject panel, Shop_Item_Data item)
    {
        panel.SetActive(true);
        // Get references inside the panel
        TextMeshProUGUI nameText = panel.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI bonusText = panel.transform.Find("Bonus").GetComponent<TextMeshProUGUI>();
        Button unequipButton = panel.transform.Find("UnequipButton").GetComponent<Button>();
        Image icon = panel.transform.Find("IconPanel/Icon").GetComponent<Image>();

        // Set values
        nameText.text = item.itemName;
        bonusText.text = $"{item.type}: +{item.value}";
        icon.sprite = item.icon;

        // Assign Unequip button action
        unequipButton.onClick.RemoveAllListeners();
        unequipButton.onClick.AddListener(() =>
        {
            PlayerStats.Instance.UnequipItem(item.type);
            PopulateEquipment();
            RefreshInventoryUI();
        });
    }

    private void ClearEquipmentPanel(GameObject panel)
    {
        panel.SetActive(false);
        // panel.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = "-";
        // panel.transform.Find("Bonus").GetComponent<TextMeshProUGUI>().text = "";
        // panel.transform.Find("IconPanel/Icon").GetComponent<Image>().sprite = null;

        // Button unequipButton = panel.transform.Find("UnequipButton").GetComponent<Button>();
        // unequipButton.onClick.RemoveAllListeners();
    }

}
