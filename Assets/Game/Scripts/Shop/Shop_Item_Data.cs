using UnityEngine;

[System.Serializable]
public class Shop_Item_Data
{
    public string itemName;
    public string description;
    public int cost;
    public Sprite icon;
    public ItemType type;
    public int value;

    public enum ItemType
    {
        MaxHealth,
        Damage,
        Armor
    }
}