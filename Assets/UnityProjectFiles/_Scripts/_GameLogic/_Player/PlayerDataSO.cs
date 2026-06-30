using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Game/Player Data")]
public class PlayerDataSO : ScriptableObject
{
    [Header("Movement Settings")]
    public float forwardSpeed = 12f;
    public float sideMovementSpeed = 5f;
    public float roadLimitX = 3f;

    [Header("Current Run State")]
    public int currentScore = 0;
    public bool isDead = false;
    public bool isMoving = false;

    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public float customGravity = -25f; // Custom gravity for a snappy, non-floaty jump
    public float swipeThresholdY = 50f; // Minimum vertical pixel delta to trigger action

    [Header("Stacking Settings")]
    public int currentBoxCount = 0;

    public int currentLevelIndex = 0; // 0 means Level 1, 1 means Level 2, etc.

    public void ResetData()
    {
        currentScore = 0;
        isDead = false;
        isMoving = false;
    }
}