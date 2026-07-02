using UnityEngine;
using UnityEngine.UI;
using RunnerGame.Core;
using RunnerGame.Level;

namespace RunnerGame.UI
{
    /// <summary>
    /// Calculates the player's real-time distance progression between the start line and the finish line 
    /// and updates a UI Slider visually.
    /// </summary>
    public class LevelProgressBarController : MonoBehaviour
    {
        [Header("Data & Core Links")]
        [SerializeField] private PlayerDataSO playerData;

        [Header("UI Components")]
        [Tooltip("The Slider component acting as the visual progress fill bar")]
        [SerializeField] private Slider progressSlider;

        private Transform playerTransform;
        private float startZPosition;
        private float finishZPosition;
        private float totalLevelDistance;
        private bool isSystemInitialized = false;

        private void Start()
        {
            // The progression bar should only monitor actively if the system configuration is established
            InitializeProgressionBounds();
        }

        /// <summary>
        /// Dynamically locates the player and calculates the total Z-axis distance boundaries of the generated level.
        /// </summary>
        public void InitializeProgressionBounds()
        {
            // Find the player object dynamically in the scene hierarchy
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj == null) return;

            playerTransform = playerObj.transform;
            startZPosition = playerTransform.position.z;

            // Find the FinishLineTrigger dynamically in the active generated scene
            FinishLineTrigger finishLine = FindFirstObjectByType<FinishLineTrigger>();
            if (finishLine == null)
            {
                Debug.LogWarning("LevelProgressBarController: FinishLineTrigger not found in the scene! Progress bar disabled.");
                return;
            }

            finishZPosition = finishLine.transform.position.z;

            // Calculate the absolute total distance span of this specific level road layout
            totalLevelDistance = finishZPosition - startZPosition;

            if (totalLevelDistance <= 0f)
            {
                Debug.LogWarning("LevelProgressBarController: Total level distance is zero or negative! Check object placement.");
                return;
            }

            // Configure slider baseline values securely
            if (progressSlider != null)
            {
                progressSlider.minValue = 0f;
                progressSlider.maxValue = 1f;
                progressSlider.value = 0f;
            }

            isSystemInitialized = true;
        }

        private void Update()
        {
            if (!isSystemInitialized || playerTransform == null || progressSlider == null) return;

            // Only compute tracker metrics while the player is actively playing to save cycles
            if (GameManager.Instance.CurrentState == GameState.Playing)
            {
                UpdateProgressGraphics();
            }
        }

        /// <summary>
        /// Measures the active clamping ratio of the player's current coordinate position relative to the level scale bounds.
        /// </summary>
        private void UpdateProgressGraphics()
        {
            float currentDistanceTraveled = playerTransform.position.z - startZPosition;

            // Calculate progress ratio normalized perfectly between 0.0f and 1.0f
            float progressNormalized = currentDistanceTraveled / totalLevelDistance;

            // Clamp values safely so unintended physical overrides don't break UI layout limits
            progressSlider.value = Mathf.Clamp01(progressNormalized);
        }
    }
}