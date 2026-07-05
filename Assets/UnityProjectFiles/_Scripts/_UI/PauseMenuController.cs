using UnityEngine;
using UnityEngine.UI;
using RunnerGame.Core;
using RunnerGame.Audio;

namespace RunnerGame.UI
{
    /// <summary>
    /// Freezes simulation loops dynamically via Time Scale manipulation and acts as HUD overlay manager.
    /// </summary>
    public class PauseMenuController : MonoBehaviour
    {
        [Header("UI Overlay Panels")]
        [SerializeField] private GameObject pauseMenuPanel;

        [Header("Control Buttons Matrix")]
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button homeButton;

        private bool isPaused = false;

        private void Start()
        {
            if (pauseButton != null) pauseButton.onClick.AddListener(PauseGame);
            if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
            if (homeButton != null) homeButton.onClick.AddListener(ReturnToMainMenu);

            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        }

        public void PauseGame()
        {
            // Disallow freezing frames unless the player is actively playing
            if (GameManager.Instance.CurrentState != GameState.Playing) return;

            isPaused = true;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(true);
            
            Time.timeScale = 0f; // Halt physics, input delta translations and internal timers instantly
                AudioManager.Instance.PlaySFX("Click");

        }

        public void ResumeGame()
        {
            isPaused = false;
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            
            Time.timeScale = 1f; // Restore game physical loops execution
                AudioManager.Instance.PlaySFX("Click");

        }

        public void ReturnToMainMenu()
        {
            Time.timeScale = 1f; // Critical: always restore time scale before unloading scene objects
                GameManager.Instance.GoToMainMenu();
                    AudioManager.Instance.PlaySFX("Click");

        }
    }
}