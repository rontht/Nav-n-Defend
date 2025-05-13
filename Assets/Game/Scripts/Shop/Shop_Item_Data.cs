using UnityEngine;

/// <summary>
/// Types of stats that shop items can modify:
/// - maxHP: Increases maximum health capacity
/// - defense: Increases damage reduction
/// - attack: Increases damage dealing capability
/// </summary>
public enum ItemType
{
    /// <summary> 
    /// /// Maximum health capacity</summary>
    maxHP,
    /// <summary>Damage 
    /// reduction</summary>
    defense,
    /// <summary>Damage 
    /// dealing capability</summary>
    attack
}

/// <summary>
/// ScriptableObject that defines a purchasable shop item:
/// - Basic info (name, description)
/// - Shop settings (cost, icon)
/// - Stat modifications (type and value)
/// - Purchase state tracking
/// 
/// Create new items using Create > Shop > Item Data in the Project window.
/// Each item automatically generates a unique ID on creation.
/// </summary>

/// <summary>
/// Scriptable Object that defines a purchasable item in the shop.
/// Each item can modify one type of player stat (HP, Attack, or Defense).
/// Items maintain their purchase state through the PlayerStats system.
/// Create new items through Unity menu: Assets > Create > Shop > Item Data
/// </summary>
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
      [Header("Stat Modification")]
    public ItemType type;
    public int value;

    /// <summary>
    /// Checks if this item has been purchased by querying PlayerStats.
    /// Note: Requires PlayerStats.Instance to be initialized.
    /// </summary>
    public bool isPurchased => PlayerStats.Instance != null && PlayerStats.Instance.HasPurchasedItem(id);

      /// <summary>
    /// Unity callback that runs in the editor when this asset is modified.
    /// Automatically generates a unique ID for new items.
    /// This ensures that items can be properly tracked in the save system.
    /// </summary>
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
