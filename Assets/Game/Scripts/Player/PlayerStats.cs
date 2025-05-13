using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// Core player statistics manager that handles:
/// - Player's core stats (HP, Attack, Defense)
/// - Currency system (coins)
/// - Purchased items tracking
/// - Stat persistence between sessions
/// - Events for UI updates
/// 
/// This is a singleton that persists between scenes and initializes before other scripts
/// to ensure stats are available when needed.
/// </summary>
[DefaultExecutionOrder(-1)] // Ensure this initializes before other scripts
public class PlayerStats : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the PlayerStats manager.
    /// Access player stats through this property (e.g., PlayerStats.Instance.currentHP)
    /// </summary>
    public static PlayerStats Instance { get; private set; }

    [Header("Base Stats")]
    [SerializeField] private int baseMaxHP = 100;
    [SerializeField] private int baseAttack = 10;
    [SerializeField] private int baseDefense = 5;
    [SerializeField] private int startingCoins = 50;

    [Header("Current Stats")]
    [SerializeField] private int _maxHP;
    [SerializeField] private int _currentHP;
    [SerializeField] private int _attack;
    [SerializeField] private int _defense;
    [SerializeField] private int _coins;

    // Public read-only properties
    public int maxHP => _maxHP;
    public int currentHP => _currentHP;
    public int attack => _attack;
    public int defense => _defense;
    public int coins => _coins;

    // List to store the IDs of purchased items
    private List<string> purchasedItemIDs = new List<string>();

    // Constants for PlayerPrefs Keys
    private const string COINS_KEY = "PlayerCoins";
    private const string MAX_HP_KEY = "PlayerMaxHP";
    private const string CURRENT_HP_KEY = "PlayerCurrentHP";
    private const string ATK_KEY = "PlayerAttack";
    private const string DEF_KEY = "PlayerDefense";
    private const string PURCHASED_ITEMS_KEY = "PurchasedItems";

    // Events for UI updates
    public event Action onCoinsChanged;
    public event Action onStatsChanged;    
    /// <summary>
    /// Initializes the singleton instance and loads saved stats.
    /// Ensures only one PlayerStats exists in the game.
    /// </summary>
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Make this object persistent across scenes
            LoadStats();
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }    

    /// <summary>
    /// Checks if the player has enough coins for a purchase
    /// </summary>
    /// <param name="cost">The cost to check against current coins</param>
    /// <returns>True if player has enough coins, false otherwise</returns>
    public bool CanAfford(int cost) => _coins >= cost;

    /// <summary>
    /// Deducts coins from the player's currency
    /// </summary>
    /// <param name="amount">Number of coins to spend</param>
    public void SpendCoins(int amount)
    {
        if (!CanAfford(amount))
        {
            Debug.LogWarning($"Not enough coins to spend {amount}. Current coins: {_coins}");
            return;
        }

        _coins -= amount;
        Debug.Log($"Spent {amount} coins. Remaining: {_coins}");
        SaveStats();
        onCoinsChanged?.Invoke();
    }

    public void AddCoins(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Cannot add negative coins. Use SpendCoins instead.");
            return;
        }

        _coins += amount;
        Debug.Log($"Added {amount} coins. Total: {_coins}");
        SaveStats();
        onCoinsChanged?.Invoke();
    }    
    /// <summary>
    /// Increases a specific stat by the given amount and updates the player state
    /// For HP increases, maintains the same health percentage after the increase
    /// </summary>
    /// <param name="statType">The type of stat to increase (HP, Attack, or Defense)</param>
    /// <param name="amount">The amount to increase the stat by</param>
    public void IncreaseStat(ItemType statType, int amount)
    {
        switch (statType)
        {
            case ItemType.maxHP:
                _maxHP += amount;
                // Also increase current HP proportionally when max HP increases
                float healthPercentage = _currentHP / (float)_maxHP;
                _currentHP = Mathf.RoundToInt(_maxHP * healthPercentage);
                _currentHP = Mathf.Min(_currentHP, _maxHP);
                Debug.Log($"MaxHP increased by {amount}. New MaxHP: {_maxHP}, CurrentHP: {_currentHP}");
                break;
            case ItemType.attack:
                _attack += amount;
                Debug.Log($"Attack increased by {amount}. New Attack: {_attack}");
                break;
            case ItemType.defense:
                _defense += amount;
                Debug.Log($"Defense increased by {amount}. New Defense: {_defense}");
                break;
        }
        SaveStats();
        onStatsChanged?.Invoke();
    }    public bool IsItemPurchased(string itemID)
    {
        return purchasedItemIDs.Contains(itemID);
    }    
    /// <summary>
    /// Saves all player stats and purchased items to PlayerPrefs for persistence between sessions.
    /// Called automatically after any stat changes or purchases.
    /// </summary>
    public void SaveStats()
    {
        PlayerPrefs.SetInt(COINS_KEY, _coins);
        PlayerPrefs.SetInt(MAX_HP_KEY, _maxHP);
        PlayerPrefs.SetInt(CURRENT_HP_KEY, _currentHP);
        PlayerPrefs.SetInt(ATK_KEY, _attack);
        PlayerPrefs.SetInt(DEF_KEY, _defense);

        string purchasedItemsString = string.Join(",", purchasedItemIDs);
        PlayerPrefs.SetString(PURCHASED_ITEMS_KEY, purchasedItemsString);

        PlayerPrefs.Save();
        Debug.Log($"Saved - HP: {_currentHP}/{_maxHP}, ATK: {_attack}, DEF: {_defense}, Coins: {_coins}");
    }    public void LoadStats()
    {
        // Load with default values if keys don't exist
        _coins = PlayerPrefs.GetInt(COINS_KEY, startingCoins);
        _maxHP = PlayerPrefs.GetInt(MAX_HP_KEY, baseMaxHP);
        _currentHP = PlayerPrefs.GetInt(CURRENT_HP_KEY, _maxHP);
        _attack = PlayerPrefs.GetInt(ATK_KEY, baseAttack);
        _defense = PlayerPrefs.GetInt(DEF_KEY, baseDefense);

        string purchasedItemsString = PlayerPrefs.GetString(PURCHASED_ITEMS_KEY, "");
        if (!string.IsNullOrEmpty(purchasedItemsString))
        {
            purchasedItemIDs = purchasedItemsString.Split(',').ToList();
        }
        else
        {
            purchasedItemIDs = new List<string>();
        }

        Debug.Log("Player stats and purchased items loaded.");

        // Notify UI of loaded stats
        onCoinsChanged?.Invoke();
        onStatsChanged?.Invoke();
    }    
    /// <summary>
    /// Recalculates all player stats based on base values and purchased items.
    /// Maintains the current health percentage when recalculating max HP.
    /// Use this when loading a game or when the effects of items need to be reapplied.
    /// </summary>
    /// <param name="allShopItems">List of all available shop items to check against purchased items</param>
    public void RecalculateStatsFromItems(List<Shop_Item_Data> allShopItems)
    {
        // Reset stats to base values
        _maxHP = baseMaxHP;
        _attack = baseAttack;
        _defense = baseDefense;

        // Store current health percentage for proportional adjustment
        float healthPercentage = _currentHP / (float)_maxHP;

        foreach (string itemID in purchasedItemIDs)
        {
            Shop_Item_Data item = allShopItems.FirstOrDefault(i => i.id == itemID);
            if (item != null)
            {
                switch (item.type)
                {
                    case ItemType.maxHP:
                        _maxHP += item.value;
                        break;
                    case ItemType.attack:
                        _attack += item.value;
                        break;
                    case ItemType.defense:
                        _defense += item.value;
                        break;
                }
            }
        }

        // Adjust current HP proportionally to the new max HP
        _currentHP = Mathf.RoundToInt(_maxHP * healthPercentage);
        _currentHP = Mathf.Clamp(_currentHP, 1, _maxHP); // Ensure at least 1 HP

        Debug.Log($"Stats recalculated. HP: {_currentHP}/{_maxHP}, ATK: {_attack}, DEF: {_defense}");
        SaveStats();
        onStatsChanged?.Invoke();
    }    public bool HasPurchasedItem(string itemId)
    {
        return purchasedItemIDs.Contains(itemId);
    }

    public void MarkItemAsPurchased(string itemId)
    {
        if (!purchasedItemIDs.Contains(itemId))
        {
            purchasedItemIDs.Add(itemId);
            Debug.Log($"Item {itemId} marked as purchased.");
            SaveStats();
            onStatsChanged?.Invoke();
        }
    }

    private void SavePurchasedItems()
    {
        string purchasedItemsStr = string.Join(",", purchasedItemIDs);
        PlayerPrefs.SetString(PURCHASED_ITEMS_KEY, purchasedItemsStr);
        PlayerPrefs.Save();
    }

    private void LoadPurchasedItems()
    {
        string purchasedItemsStr = PlayerPrefs.GetString(PURCHASED_ITEMS_KEY, "");
        purchasedItemIDs = string.IsNullOrEmpty(purchasedItemsStr) 
            ? new List<string>() 
            : purchasedItemsStr.Split(',').ToList();
    }
}
