using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Game/Player Data")]
public class PlayerDataSO : ScriptableObject
{
    [Header("Movement Settings")]
    public float forwardSpeed = 12f;
    public float sideMovementSpeed = 5f;
    public float roadLimitX = 3f;

    [Header("Current Run State (Resets Every Level)")]
    public int currentScore = 0;
    public int currentCurrencyEarnedInRun = 0; 
    public bool isDead = false;
    public bool isMoving = false;
    public int currentBoxCount = 0;

    [Header("Persistent Progressive State (Never Resets Automatically)")]
    public int totalWalletCurrency = 0; 
    public int currentLevelIndex = 0; 

    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public float customGravity = -25f; 
    public float swipeThresholdY = 50f;

    [Header("Shop Save Data Matrix")]
    [SerializeField] private string _equippedItemId = "Default";

    // PlayerPrefs Save Keys
    private const string LEVEL_INDEX_KEY = "Saved_CurrentLevelIndex";
    private const string WALLET_CURRENCY_KEY = "Saved_TotalWalletCurrency";

    /// <summary>
    /// Loads persistent player progress (level index and wallet currency) from PlayerPrefs.
    /// Call this method on game startup or when initializing the level.
    /// </summary>
    public void LoadPersistentData()
    {
        currentLevelIndex = PlayerPrefs.GetInt(LEVEL_INDEX_KEY, 0);
        totalWalletCurrency = PlayerPrefs.GetInt(WALLET_CURRENCY_KEY, 0);
    }

    /// <summary>
    /// Saves the persistent progression data (level index and wallet currency) to PlayerPrefs.
    /// </summary>
    public void SavePersistentData()
    {
        PlayerPrefs.SetInt(LEVEL_INDEX_KEY, currentLevelIndex);
        PlayerPrefs.SetInt(WALLET_CURRENCY_KEY, totalWalletCurrency);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Advances player to the next level and saves progress persistently.
    /// </summary>
    public void CompleteCurrentLevel()
    {
        currentLevelIndex++;
        SavePersistentData();
    }

    /// <summary>
    /// Adds earned currency to the total wallet and saves progress persistently.
    /// </summary>
    public void AddCurrencyToWallet(int amount)
    {
        totalWalletCurrency += amount;
        SavePersistentData();
    }

    /// <summary>
    /// Resets only the immediate live run metrics, preserving the continuous wallet and level progression.
    /// </summary>
    public void ResetData()
    {
        currentScore = 0;
        currentCurrencyEarnedInRun = 0; 
        currentBoxCount = 0; 
        isDead = false;
        isMoving = false;
    }

    /// <summary>
    /// Absolute hard reset helper for debugging, testing, or profile wipes.
    /// Clears saved PlayerPrefs and resets variables.
    /// </summary>
    public void FullWipeData()
    {
        ResetData();
        totalWalletCurrency = 0;
        currentLevelIndex = 0;
        
        PlayerPrefs.DeleteKey(LEVEL_INDEX_KEY);
        PlayerPrefs.DeleteKey(WALLET_CURRENCY_KEY);
        PlayerPrefs.Save();
    }

    public string equippedItemId
    {
        get
        {
            return PlayerPrefs.GetString("EquippedItemId", "Default");
        }
        set
        {
            _equippedItemId = value;
            PlayerPrefs.SetString("EquippedItemId", _equippedItemId);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Checks if a specific item has been purchased and saved to PlayerPrefs.
    /// </summary>
    public bool IsItemPurchased(string itemId)
    {
        if (itemId == "Default") return true; // Default item is always unlocked
        return PlayerPrefs.GetInt("ShopItem_" + itemId, 0) == 1;
    }

    /// <summary>
    /// Unlocks an item permanently and registers it into PlayerPrefs storage layout safely.
    /// </summary>
    public void PurchaseItem(string itemId)
    {
        PlayerPrefs.SetInt("ShopItem_" + itemId, 1);
        PlayerPrefs.Save();
    }
}