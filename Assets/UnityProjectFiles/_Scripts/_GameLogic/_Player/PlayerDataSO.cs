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
    /// </summary>
    public void FullWipeData()
    {
        ResetData();
        totalWalletCurrency = 0;
        //currentLevelIndex = 0;
    }
}