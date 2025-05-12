using UnityEngine;

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop/Item Data")]
public class Shop_Item_Data : ScriptableObject
{
    public string id;
    public string itemName;
    [TextArea(2, 4)]
    public string description;
    public int cost;
    public Sprite icon;
    public ItemType type;
    public int value;
    public bool isPurchased;

    public enum ItemType
    {
        maxHP,
        defense,
        attack

    }

    public Shop_Item_Data()
    {
        id = System.Guid.NewGuid().ToString();
        isPurchased = false;
    }
}
