using UnityEngine;

/// Types of stats that shop items can modify.
public enum ItemType
{
    /// Maximum health capacity
    HP,
  
    ///Damage dealing capability
    attack,
    /// Temporary effects, e.g., healing potions
    Temp
}

/// ScriptableObject that defines a purchasable shop item.
[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop/Item Data")]
public class Shop_Item_Data : ScriptableObject
{
    [Header("Basic Info")]
    public string id;
    public string itemName;
    [TextArea(2, 4)]
    public string description;
    
    [Header("Shop Settings")]
    public int cost;
    public Sprite icon;
    public int maxPlayerOwns = -1; // -1 for infinite

    [Header("Stat Modification")]
    public ItemType type;
    public int value;

    /// Checks if this item has been purchased.
    public bool isPurchased => PlayerStats.Instance != null && PlayerStats.Instance.HasPurchasedItem(id);

    /// callback that runs in the editor when this asset is modified.
    private void OnValidate()
    {
        // Ensure each item has a unique ID
        if (string.IsNullOrEmpty(id))
        {
            id = System.Guid.NewGuid().ToString();
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            #endif
        }
    }
}
