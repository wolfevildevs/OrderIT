using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using RunnerGame.Core;

namespace RunnerGame.UI
{
    /// <summary>
    /// UI Presentation layer that handles fluid UI panels blending via CanvasGroup alpha transitions.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("UI Panels (Canvas Groups)")]
        [SerializeField] private CanvasGroup mainMenuPanel;
        [SerializeField] private CanvasGroup inGameHUDPanel;
        [SerializeField] private CanvasGroup winPanel;
        [SerializeField] private CanvasGroup losePanel;

        [Header("Transition Configuration")]
        [SerializeField] private float fadeDuration = 0.4f; 

        [Header("Buttons Configuration Matrix")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button winMainMenuButton;  
        [SerializeField] private Button loseMainMenuButton; 

        [Header("Score & Box Displays (TextMeshPro)")]
        [SerializeField] private TMPro.TextMeshProUGUI winScoreText;
        [SerializeField] private TMPro.TextMeshProUGUI winBoxesText;
        [SerializeField] private TMPro.TextMeshProUGUI winCurrencyText; 
        [SerializeField] private TMPro.TextMeshProUGUI loseScoreText;
        
        [Header("Data Architecture Link")]
        [SerializeField] private PlayerDataSO playerData; 

        private void Start()
        {
            // Map actions mechanically adhering to Single Responsibility Principles
            startButton.onClick.AddListener(() => GameManager.Instance.StartGame());
            restartButton.onClick.AddListener(() => GameManager.Instance.RestartLevel());
            nextLevelButton.onClick.AddListener(() => GameManager.Instance.NextLevel());
            
            // Link decoupled layout button paths safely to standard structural backend framework
            if (winMainMenuButton != null) winMainMenuButton.onClick.AddListener(() => GameManager.Instance.GoToMainMenu());
            if (loseMainMenuButton != null) loseMainMenuButton.onClick.AddListener(() => GameManager.Instance.GoToMainMenu());

            InitializeHUDGraphics();
        }

        private void InitializeHUDGraphics()
        {
            SetPanelAlphaImmediate(mainMenuPanel, 1f, true);
            SetPanelAlphaImmediate(inGameHUDPanel, 0f, false);
            SetPanelAlphaImmediate(winPanel, 0f, false);
            SetPanelAlphaImmediate(losePanel, 0f, false);
        }

        public void OnGameStartedTransition()
        {
            StartCoroutine(FadePanel(mainMenuPanel, 0f, false));
            StartCoroutine(FadePanel(inGameHUDPanel, 1f, true));
        }

        public void OnLevelWonTransition()
        {
            if (playerData != null)
            {
                if (winScoreText != null) winScoreText.text = $"SCORE: {playerData.currentScore}";
                if (winBoxesText != null) winBoxesText.text = $" {playerData.currentBoxCount}";
                if (winCurrencyText != null) winCurrencyText.text = $" ${playerData.currentCurrencyEarnedInRun}";
            }

            StartCoroutine(FadePanel(inGameHUDPanel, 0f, false));
            StartCoroutine(FadePanel(winPanel, 1f, true));
        }

        public void OnLevelLostTransition()
        {
            if (playerData != null && loseScoreText != null)
            {
                loseScoreText.text = $"FINAL SCORE: {playerData.currentScore}";
            }

            StartCoroutine(FadePanel(inGameHUDPanel, 0f, false));
            StartCoroutine(FadePanel(losePanel, 1f, true));
        }

        private IEnumerator FadePanel(CanvasGroup cg, float targetAlpha, bool isInteractable)
        {
            if (cg == null) yield break;

            if (isInteractable) cg.blocksRaycasts = true;

            float startAlpha = cg.alpha;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                cg.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
                yield return null;
            }

            cg.alpha = targetAlpha;
            cg.interactable = isInteractable;
            cg.blocksRaycasts = isInteractable;
        }

        private void SetPanelAlphaImmediate(CanvasGroup cg, float alpha, bool interactable)
        {
            if (cg == null) return;
            cg.alpha = alpha;
            cg.interactable = interactable;
            cg.blocksRaycasts = interactable;
        }
    }
}