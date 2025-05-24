using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

[DefaultExecutionOrder(-1)] //initializes before other scripts
public class PlayerStats : MonoBehaviour
{

    public static PlayerStats Instance { get; private set; }

    [Header("Base Stats")]
    [SerializeField] private int baseMaxHP = 100;
    [SerializeField] private int baseAttack = 10;
    [SerializeField] private int startingCoins = 50;
    [SerializeField] private int startingLevel = 1;
    [SerializeField] private int startingExp = 0;
    [SerializeField] private int startingExpToLevelUp = 10;
    [SerializeField] private int maxLevel = 100;

    // Store the initial base values for linear stat growth
    private int initialBaseMaxHP;
    private int initialBaseAttack;

    [Header("Current Stats")]
    [SerializeField] private int _maxHP;
    [SerializeField] private int _currentHP;
    [SerializeField] private int _attack;
    [SerializeField] private int _coins;
    [SerializeField] private int _level;
    [SerializeField] private int _currentExp;
    [SerializeField] private int _expToLevelUp;


    public int maxHP => _maxHP;
    public int currentHP => _currentHP;
    public int attack => _attack;
    public int coins => _coins;
    public int currentExp => _currentExp;
    public int expToLevelUp => _expToLevelUp;
    public int level => _level;

   /// <summary>
   /// public slimeDamage = 25;  /// Quick and dirty slime damage because I can't be bothered making ANOTHER event LISTENER GOD UUUUUGH
   /// </summary>

    // List to store the IDs of purchased items
    private List<string> purchasedItemIDs = new List<string>();
    // Dictionary to store the count of owned items
    private Dictionary<string, int> ownedItemCounts = new Dictionary<string, int>();
    // Dictionary to store currently equipped items by their ItemType (e.g., one HP item, one Attack item)
    private Dictionary<ItemType, Shop_Item_Data> equippedItemIDs = new Dictionary<ItemType, Shop_Item_Data>();

    // Constants for PlayerPrefs Keys
    private const string COINS_KEY = "PlayerCoins";
    private const string MAX_HP_KEY = "PlayerMaxHP";
    private const string CURRENT_HP_KEY = "PlayerCurrentHP";
    private const string ATK_KEY = "PlayerAttack";
    private const string PURCHASED_ITEMS_KEY = "PurchasedItems";
    private const string OWNED_ITEM_COUNTS_KEY = "OwnedItemCounts";
    private const string LEVEL_KEY = "PlayerLevel";
    private const string EXP_KEY = "PlayerExp";
    private const string EXP_TO_LEVEL_KEY = "PlayerExpToLevelUp";
    private const string BASE_MAX_HP_KEY = "PlayerLeveledBaseMaxHP";
    private const string BASE_ATTACK_KEY = "PlayerLeveledBaseAttack";

    public event Action onCoinsChanged;
    public event Action onStatsChanged;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            initialBaseMaxHP = baseMaxHP;
            initialBaseAttack = baseAttack;
            LoadStats();
            Debug.Log("PlayerStats Initialized in MainMenu");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        TestStats();
    }

    /// Checks if the player has enough coins for a purchase
    public bool CanAfford(int cost) => _coins >= cost;

    /// Deducts coins from the player's currency
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

    /// Saves all player stats and purchased items to PlayerPrefs.
    public void SaveStats()
    {
        PlayerPrefs.SetInt(COINS_KEY, _coins);
        PlayerPrefs.SetInt(MAX_HP_KEY, _maxHP);
        PlayerPrefs.SetInt(CURRENT_HP_KEY, _currentHP);
        PlayerPrefs.SetInt(ATK_KEY, _attack);
        PlayerPrefs.SetInt(LEVEL_KEY, _level);
        PlayerPrefs.SetInt(EXP_KEY, _currentExp);
        PlayerPrefs.SetInt(EXP_TO_LEVEL_KEY, _expToLevelUp);

        // Save current base stats
        PlayerPrefs.SetInt(BASE_MAX_HP_KEY, baseMaxHP);
        PlayerPrefs.SetInt(BASE_ATTACK_KEY, baseAttack);

        string purchasedItemsString = string.Join(",", purchasedItemIDs);
        PlayerPrefs.SetString(PURCHASED_ITEMS_KEY, purchasedItemsString);

        // Save owned item counts
        string ownedItemCountsString = string.Join(";", ownedItemCounts.Select(kv => kv.Key + ":" + kv.Value));
        PlayerPrefs.SetString(OWNED_ITEM_COUNTS_KEY, ownedItemCountsString);

        PlayerPrefs.Save();

        StringBuilder tempItemsLog = new StringBuilder();
        if (ShopManager.Instance != null && ShopManager.Instance.availableItems != null)
        {
            bool firstTempItem = true;
            foreach (var entry in ownedItemCounts)
            {
                if (entry.Value > 0)
                {
                    // Find the item data to check its type
                    Shop_Item_Data itemData = ShopManager.Instance.availableItems.FirstOrDefault(item => item.id == entry.Key);
                    if (itemData != null && itemData.type == ItemType.Heal)
                    {
                        if (!firstTempItem)
                        {
                            tempItemsLog.Append(", ");
                        }
                        tempItemsLog.Append($"{itemData.itemName}: {entry.Value}");
                        firstTempItem = false;
                    }
                }
            }
        }

        string tempItemsString = tempItemsLog.Length > 0 ? tempItemsLog.ToString() : "None";

        Debug.Log($"Saved - HP: {_currentHP}/{_maxHP}, ATK: {_attack}, Coins: {_coins}, Level: {_level}, EXP: {_currentExp}/{_expToLevelUp}, Temp Items: [{tempItemsString}]");
    }

    public void LoadStats()
    {
        // Load core progression stats first
        _coins = PlayerPrefs.GetInt(COINS_KEY, startingCoins);
        _level = PlayerPrefs.GetInt(LEVEL_KEY, startingLevel);
        _currentExp = PlayerPrefs.GetInt(EXP_KEY, startingExp);
        _expToLevelUp = PlayerPrefs.GetInt(EXP_TO_LEVEL_KEY, startingExpToLevelUp);

        baseMaxHP = PlayerPrefs.GetInt(BASE_MAX_HP_KEY, baseMaxHP);
        baseAttack = PlayerPrefs.GetInt(BASE_ATTACK_KEY, baseAttack);

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

        // Load owned item counts
        string ownedItemCountsString = PlayerPrefs.GetString(OWNED_ITEM_COUNTS_KEY, "");
        ownedItemCounts = new Dictionary<string, int>();
        if (!string.IsNullOrEmpty(ownedItemCountsString))
        {
            foreach (string pair in ownedItemCountsString.Split(';'))
            {
                string[] keyValue = pair.Split(':');
                if (keyValue.Length == 2)
                {
                    ownedItemCounts[keyValue[0]] = int.Parse(keyValue[1]);
                }
            }
        }

        // Debug.Log("Player stats and purchased items loaded.");

        // Notify UI of loaded stats
        onCoinsChanged?.Invoke();
        onStatsChanged?.Invoke();
    }

    public void ResetStats()
    {
        _level = startingLevel;
        _coins = startingCoins;
        _currentExp = startingExp;
        _expToLevelUp = startingExpToLevelUp;

        // Reset base stats to their absolute L1 defaults
        baseMaxHP = 100;
        baseAttack = 10;


        // Total stats become the L1 base stats
        _maxHP = baseMaxHP;
        _currentHP = baseMaxHP;
        _attack = baseAttack;

        purchasedItemIDs.Clear(); // Clear all purchased items
        ownedItemCounts.Clear(); // Clear all owned item counts
        equippedItemIDs.Clear();

        SaveStats(); // Save the reset stats
        onCoinsChanged?.Invoke();
        onStatsChanged?.Invoke();

        Debug.Log("Player stats have been reset to default values.");
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
            // SaveStats(); // SaveStats is called in IncreaseStat or after spending coins
        }
        // Increment owned item count
        if (ownedItemCounts.ContainsKey(itemId))
        {
            ownedItemCounts[itemId]++;
        }
        else
        {
            ownedItemCounts[itemId] = 1;
        }
        SaveStats();
        onStatsChanged?.Invoke();
    }

    public int GetOwnedItemCount(string itemId)
    {
        return ownedItemCounts.TryGetValue(itemId, out int count) ? count : 0;
    }

    public List<string> GetPurchasedItemIDs()
    {
        return new List<string>(purchasedItemIDs);
    }

    public Dictionary<ItemType, Shop_Item_Data> GetEquippedItemIDs()
    {
        return equippedItemIDs;
    }

    private void RemoveFromPurchasedItems(string itemId)
    {
        if (purchasedItemIDs.Contains(itemId))
        {
            purchasedItemIDs.Remove(itemId);
        }
        SaveStats();
        onStatsChanged?.Invoke();
    }

    public void EquipItem(Shop_Item_Data item)
    {
        if (item == null)
        {
            Debug.LogWarning("Attempted to equip a null item.");
            return;
        }

        ItemType type = item.type;

        // If same type is already equipped, unequip it first
        if (equippedItemIDs.ContainsKey(type))
        {
            UnequipItem(type);
        }

        // Equip the new item
        equippedItemIDs[type] = item;

        // Apply stat bonus
        switch (type)
        {
            case ItemType.HP:
                _maxHP += item.value;
                _currentHP = Mathf.Min(_currentHP, maxHP);
                break;
            case ItemType.attack:
                _attack += item.value;
                break;
        }

        // Reduce inventory count
        if (ownedItemCounts.ContainsKey(item.id))
        {
            ownedItemCounts[item.id]--;
            if (ownedItemCounts[item.id] <= 0)
            {
                ownedItemCounts.Remove(item.id);
            }
        }

        Debug.Log($"Equipped item {item.itemName} ({type}).");

        SaveStats();
        onStatsChanged?.Invoke();
    }

    public void UnequipItem(ItemType type)
    {
        if (!equippedItemIDs.ContainsKey(type)) return;

        Shop_Item_Data item = equippedItemIDs[type];

        // Remove bonuses
        switch (type)
        {
            case ItemType.HP:
                _maxHP -= item.value;
                _currentHP = Mathf.Min(_currentHP, maxHP); // Clamp current HP
                break;
            case ItemType.attack:
                _attack -= item.value;
                break;
        }
        
        UISoundPlayer.Instance.PlayBackwardClickSound();
        // Return item to inventory
        if (ownedItemCounts.ContainsKey(item.id))
        {
            IncreaseItemCount(item.id, 1);
            ownedItemCounts[item.id]++;
        }
        else
        {
            ownedItemCounts[item.id] = 1;
        }

        equippedItemIDs.Remove(type);

        Debug.Log($"Unequipped item {item.itemName} ({type}). Returned to inventory.");

        SaveStats();
        onStatsChanged?.Invoke();
    }

    public void DecreaseItemCount(string itemId, int amount)
    {
        if (ownedItemCounts.ContainsKey(itemId))
        {
            ownedItemCounts[itemId] -= amount;
            if (ownedItemCounts[itemId] <= 0)
            {
                ownedItemCounts.Remove(itemId);
                RemoveFromPurchasedItems(itemId);
            }
            SaveStats();
            onStatsChanged?.Invoke();
        }
    }

    public void IncreaseItemCount(string itemId, int amount)
    {
        if (ownedItemCounts.ContainsKey(itemId))
        {
            ownedItemCounts[itemId] += amount;
        }
        else
        {
            ownedItemCounts[itemId] = amount;
            AddToPurchasedItems(itemId);
        }

        SaveStats();
        onStatsChanged?.Invoke();
    }

    private void AddToPurchasedItems(string itemId)
    {
        if (!purchasedItemIDs.Contains(itemId))
        {
            purchasedItemIDs.Add(itemId);
        }
    }

    /// Experience and Leveling Methods
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
        _currentExp -= _expToLevelUp;
        _level++;

        int newBaseMaxHP = initialBaseMaxHP + ((_level - 1) * 10); // +10 HP per level (after level 1)
        int newBaseAttack = initialBaseAttack + ((_level - 1) * 5); // +5 Attack per level (after level 1)

        // Calculate the change in base stats
        int deltaBaseHP = newBaseMaxHP - baseMaxHP;
        int deltaBaseAttack = newBaseAttack - baseAttack;

        // Store old total MaxHP
        float oldTotalMaxHP = _maxHP;

        // Update base stats
        baseMaxHP = newBaseMaxHP;
        baseAttack = newBaseAttack;

        // Update total stats
        _maxHP += deltaBaseHP;
        _attack += deltaBaseAttack;

        // Adjust _currentHP
        if (oldTotalMaxHP > 0)
        {
            _currentHP = Mathf.RoundToInt(((float)_currentHP / oldTotalMaxHP) * _maxHP);
        }
        else
        {
            _currentHP = _maxHP;
        }
        _currentHP = Mathf.Min(_currentHP, _maxHP);
        _currentHP = Mathf.Max(0, _currentHP);

        Debug.Log($"Leveled up! New Level: {_level}. New Base HP: {baseMaxHP}, New Base ATK: {baseAttack}. Total MaxHP: {_maxHP}, Total ATK: {_attack}. CurrentHP: {_currentHP}");

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
        Debug.Log("Current: " + _currentHP + " Heal: " + amount);
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
