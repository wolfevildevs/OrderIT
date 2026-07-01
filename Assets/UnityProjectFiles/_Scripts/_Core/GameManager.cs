using UnityEngine;
using UnityEngine.SceneManagement;
using RunnerGame.Player;

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
            
            // Forcefully lock player movement at level initialization
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

        public void CompleteLevel()
        {
            if (currentState != GameState.Playing) return;

            currentState = GameState.Won;
            if (playerData != null) playerData.isMoving = false;

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
            // Monitor player health/bounds metrics from the persistent scriptable layer
            if (currentState == GameState.Playing && playerData != null && playerData.isDead)
            {
                LevelFailed();
            }
        }

        public void RestartLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        public void NextLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}