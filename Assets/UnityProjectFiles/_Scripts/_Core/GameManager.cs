using UnityEngine;
using UnityEngine.SceneManagement;
using RunnerGame.Player;
using RunnerGame.Level;

namespace RunnerGame.Core
{
    public enum GameState { WaitingToStart, Playing, Won, Lost }

    /// <summary>
    /// Core architecture manager that drives the level workflow states using ScriptableObject Events.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Data References")]
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private GameLevelsDatabaseSO levelsDatabase;

        [Header("Architecture Events (Scriptable Objects)")]
        [SerializeField] private GameEventSO onLevelStartedEvent;
        [SerializeField] private GameEventSO onLevelWonEvent;
        [SerializeField] private GameEventSO onLevelLostEvent;

        private GameState currentState = GameState.WaitingToStart;
        public GameState CurrentState => currentState;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            currentState = GameState.WaitingToStart;
            
            if (playerData != null)
            {
                playerData.ResetData();
                playerData.isMoving = false; 
            }
        }

        /// <summary>
        /// Executed by the UI Start Button to transition into core gameplay loops.
        /// </summary>
        public void StartGame()
        {
            if (currentState != GameState.WaitingToStart) return;

            currentState = GameState.Playing;
            if (playerData != null) playerData.isMoving = true; 

            if (onLevelStartedEvent != null) onLevelStartedEvent.Raise();
        }

        /// <summary>
        /// Finalizes the level victory state, aggregates rewards into the master wallet, and dispatches architecture events.
        /// </summary>
        public void CompleteLevel(int totalCalculatedScore, int totalCalculatedCurrency)
        {
            if (currentState != GameState.Playing) return;

            currentState = GameState.Won;
            
            if (playerData != null)
            {
                playerData.isMoving = false;
                playerData.currentScore = totalCalculatedScore;
                
                // Track run specific earnings and commit them permanently to the lifetime wallet
                playerData.currentCurrencyEarnedInRun = totalCalculatedCurrency;
                playerData.totalWalletCurrency += totalCalculatedCurrency; 
                
                Debug.Log($"Level Completed! Earned: {totalCalculatedCurrency}. Total Wallet: {playerData.totalWalletCurrency}");
            }

            if (onLevelWonEvent != null) onLevelWonEvent.Raise();
        }

        public void LevelFailed()
        {
            if (currentState != GameState.Playing) return;

            currentState = GameState.Lost;
            if (playerData != null) playerData.isMoving = false;

            if (onLevelLostEvent != null) onLevelLostEvent.Raise();
        }

        private void Update()
        {
            if (currentState == GameState.Playing && playerData != null && playerData.isDead)
            {
                LevelFailed();
            }
        }

        public void RestartLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        
        /// <summary>
        /// Validates next level existence via database bounds check before incrementing indices to prevent empty scene errors.
        /// </summary>
        public void NextLevel()
        {
            if (playerData == null || levelsDatabase == null) return;

            int nextLevelIndex = playerData.currentLevelIndex + 1;

            // Safe progressive check: only increment index if a valid LevelDataSO asset is found ahead
            if (levelsDatabase.HasLevel(nextLevelIndex))
            {
                playerData.currentLevelIndex++;
            }
            else
            {
                Debug.LogWarning("No more unique levels inside GameLevelsDatabaseSO! Restarting from Level 1 or clamping.");
                playerData.currentLevelIndex = levelsDatabase.TotalLevels - 1;
            }
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        /// <summary>
        /// Reloads the active scene without altering progression state to fallback into the main menu layout seamlessly.
        /// </summary>
        public void GoToMainMenu()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}