using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

/// <summary>
/// Core player statistics manager that handles:
/// - Player's core stats (HP, Attack)
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
    [SerializeField] private int baseMaxHP = 100; // Represents current base HP, initialized to L1 default
    [SerializeField] private int baseAttack = 10;   // Represents current base Attack, initialized to L1 default
    [SerializeField] private int startingCoins = 50;
    [SerializeField] private int startingLevel = 1;
    [SerializeField] private int startingExp = 0;
    [SerializeField] private int startingExpToLevelUp = 10;
    [SerializeField] private int maxLevel = 100;

    [Header("Current Stats")]
    [SerializeField] private int _maxHP;
    [SerializeField] private int _currentHP;
    [SerializeField] private int _attack;
    [SerializeField] private int _coins;
    [SerializeField] private int _level;
    [SerializeField] private int _currentExp;
    [SerializeField] private int _expToLevelUp;


    // Public read-only properties
    public int maxHP => _maxHP;
    public int currentHP => _currentHP;
    public int attack => _attack;
    public int coins => _coins;
    public int currentExp => _currentExp;
    public int expToLevelUp => _expToLevelUp;
    public int level => _level;

    // List to store the IDs of purchased items
    private List<string> purchasedItemIDs = new List<string>();

    // Constants for PlayerPrefs Keys
    private const string COINS_KEY = "PlayerCoins";
    private const string MAX_HP_KEY = "PlayerMaxHP";
    private const string CURRENT_HP_KEY = "PlayerCurrentHP";
    private const string ATK_KEY = "PlayerAttack";
    private const string PURCHASED_ITEMS_KEY = "PurchasedItems";
    private const string LEVEL_KEY = "PlayerLevel";
    private const string EXP_KEY = "PlayerExp";
    private const string EXP_TO_LEVEL_KEY = "PlayerExpToLevelUp";
    private const string BASE_MAX_HP_KEY = "PlayerLeveledBaseMaxHP"; // New Key
    private const string BASE_ATTACK_KEY = "PlayerLeveledBaseAttack"; // New Key

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
            Debug.Log("PlayerStats Initialized in MainMenu");
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    private void Update()
    {
        TestStats();
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
    /// <param name="statType">The type of stat to increase (HP, Attack)</param>
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
        }
        SaveStats();
        onStatsChanged?.Invoke();
    }

    public bool IsItemPurchased(string itemID)
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
        PlayerPrefs.SetInt(LEVEL_KEY, _level);
        PlayerPrefs.SetInt(EXP_KEY, _currentExp);
        PlayerPrefs.SetInt(EXP_TO_LEVEL_KEY, _expToLevelUp);

        // Save current (potentially leveled-up) base stats
        PlayerPrefs.SetInt(BASE_MAX_HP_KEY, baseMaxHP);
        PlayerPrefs.SetInt(BASE_ATTACK_KEY, baseAttack);

        string purchasedItemsString = string.Join(",", purchasedItemIDs);
        PlayerPrefs.SetString(PURCHASED_ITEMS_KEY, purchasedItemsString);

        PlayerPrefs.Save();
        Debug.Log($"Saved - HP: {_currentHP}/{_maxHP}, ATK: {_attack}, Coins: {_coins}, Level: {_level}, EXP: {_currentExp}/{_expToLevelUp}");
    }

    public void LoadStats()
    {
        // Load core progression stats first
        _coins = PlayerPrefs.GetInt(COINS_KEY, startingCoins);
        _level = PlayerPrefs.GetInt(LEVEL_KEY, startingLevel);
        _currentExp = PlayerPrefs.GetInt(EXP_KEY, startingExp);
        _expToLevelUp = PlayerPrefs.GetInt(EXP_TO_LEVEL_KEY, startingExpToLevelUp);

        // Load current base stats. If keys don't exist, 'baseMaxHP' (etc.) will retain its Inspector-defined L1 default value.
        baseMaxHP = PlayerPrefs.GetInt(BASE_MAX_HP_KEY, baseMaxHP);
        baseAttack = PlayerPrefs.GetInt(BASE_ATTACK_KEY, baseAttack);

        // Load total stats, defaulting to the now loaded/initialized base stats.
        // This ensures that if total stats aren't saved, they start equal to current base stats.
        _maxHP = PlayerPrefs.GetInt(MAX_HP_KEY, baseMaxHP);
        _attack = PlayerPrefs.GetInt(ATK_KEY, baseAttack);
        _currentHP = PlayerPrefs.GetInt(CURRENT_HP_KEY, _maxHP); // Default current HP to loaded max HP


        string purchasedItemsString = PlayerPrefs.GetString(PURCHASED_ITEMS_KEY, "");
        if (!string.IsNullOrEmpty(purchasedItemsString))
        {
            purchasedItemIDs = purchasedItemsString.Split(',').ToList();
        }
        else
        {
            purchasedItemIDs = new List<string>();
        }

        // Debug.Log("Player stats and purchased items loaded.");

        // Notify UI of loaded stats
        onCoinsChanged?.Invoke();
        onStatsChanged?.Invoke();
    }

    public void ResetStats()
    {
        // Reset to initial L1 values defined by startingX fields or hardcoded L1 defaults
        _level = startingLevel;
        _coins = startingCoins;
        _currentExp = startingExp;
        _expToLevelUp = startingExpToLevelUp;

        // Reset base stats to their absolute L1 defaults
        baseMaxHP = 100; 
        baseAttack = 10;    

        // Total stats become the L1 base stats
        _maxHP = baseMaxHP;
        _currentHP = baseMaxHP; // Full health at L1
        _attack = baseAttack;
        
        purchasedItemIDs.Clear(); // Clear all purchased items on reset

        SaveStats(); // Save the reset stats to PlayerPrefs
        onCoinsChanged?.Invoke();
        onStatsChanged?.Invoke();

        Debug.Log("Player stats have been reset to default values.");
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
                }
            }
        }

        // Adjust current HP proportionally to the new max HP
        _currentHP = Mathf.RoundToInt(_maxHP * healthPercentage);
        _currentHP = Mathf.Clamp(_currentHP, 1, _maxHP); // Ensure at least 1 HP

        Debug.Log($"Stats recalculated. HP: {_currentHP}/{_maxHP}, ATK: {_attack}");
        SaveStats();
        onStatsChanged?.Invoke();
    }

    public bool HasPurchasedItem(string itemId)
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





    /// <summary>
    /// Experience and Leveling Methods, 
    /// </summary>
    public void GainExperience(int amount)
    {
        if (amount <= 0) return;
        
        // if under level cap, add exp
        if (_level < maxLevel)
        {
            _currentExp += amount;
        }

        Debug.Log($"Gained {amount} EXP. Current EXP: {_currentExp}/{_expToLevelUp}");

        // if exceed exp cap, level up
        while (_currentExp >= _expToLevelUp)
        {
            LevelUp();
        }

        SaveStats();
        onStatsChanged?.Invoke();
    }

    private void LevelUp()
    {
        // level up and remove exp
        _currentExp -= _expToLevelUp;
        _level++;

        // Store old base values for delta calculation
        int oldLeveledBaseMaxHP = baseMaxHP;
        int oldLeveledBaseAttack = baseAttack;

        // Increase current base stats by 20% and round up
        baseMaxHP = Mathf.CeilToInt(baseMaxHP * 1.20f);
        baseAttack = Mathf.CeilToInt(baseAttack * 1.20f);

        // Calculate the actual change (delta) in base stats
        int deltaBaseHP = baseMaxHP - oldLeveledBaseMaxHP;
        int deltaBaseAttack = baseAttack - oldLeveledBaseAttack;

        // Store old total MaxHP for health percentage calculation
        float oldTotalMaxHP = _maxHP;

        // Update total stats by adding the delta from base stat increase
        _maxHP += deltaBaseHP;
        _attack += deltaBaseAttack;

        // Adjust _currentHP proportionally to maintain health percentage
        if (oldTotalMaxHP > 0)
        {
            _currentHP = Mathf.RoundToInt(((float)_currentHP / oldTotalMaxHP) * _maxHP);
        }
        else
        {
            // If old max HP was 0 or less (e.g., just revived or error state), set current HP to new max HP
            _currentHP = _maxHP;
        }
        _currentHP = Mathf.Min(_currentHP, _maxHP); // Clamp current HP to not exceed new max HP
        _currentHP = Mathf.Max(0, _currentHP);     // Ensure current HP is not negative

        Debug.Log($"Leveled up! New Level: {_level}. New Base HP: {baseMaxHP}, New Base ATK: {baseAttack}. Total MaxHP: {_maxHP}, Total ATK: {_attack}. CurrentHP: {_currentHP}");
        
        // reset the exp to 0 if max level is reached
        if (_level >= maxLevel)
        {
            _currentExp = 0;
        }
        SaveStats();
        onStatsChanged?.Invoke();
    }

    public void TakeDamage(int amount)
    {
        _currentHP = Mathf.Max(0, _currentHP - amount);
        SaveStats();
        onStatsChanged?.Invoke();
    }

    public void Heal(int amount)
    {
        _currentHP = Mathf.Min(_maxHP, _currentHP + amount);
        SaveStats();
        onStatsChanged?.Invoke();
    }

    // For testing
    public void TestStats()
    {
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            Instance.TakeDamage(3);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            Instance.Heal(3);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            Instance.GainExperience(3);
        }
        else if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            Instance.ResetStats();
        }
    }
}
