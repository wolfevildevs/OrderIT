using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using RunnerGame.Core;

namespace RunnerGame.UI
{
    /// <summary>
    /// UI Presentation layer that handles fluid UI panels blending via CanvasGroup alpha transitions.
    /// Called cleanly by GameEventListeners deployed on the Canvas object hierarchy.
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

        [Header("Buttons")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button nextLevelButton;

        private void Start()
        {
            // Map actions mechanically adhering to SRP principles
            startButton.onClick.AddListener(() => GameManager.Instance.StartGame());
            restartButton.onClick.AddListener(() => GameManager.Instance.RestartLevel());
            nextLevelButton.onClick.AddListener(() => GameManager.Instance.NextLevel());

            InitializeHUDGraphics();
        }

        private void InitializeHUDGraphics()
        {
            SetPanelAlphaImmediate(mainMenuPanel, 1f, true);
            SetPanelAlphaImmediate(inGameHUDPanel, 0f, false);
            SetPanelAlphaImmediate(winPanel, 0f, false);
            SetPanelAlphaImmediate(losePanel, 0f, false);
        }

        // --- Public transition triggers linked natively via GameEventListeners in the Inspector ---

        public void OnGameStartedTransition()
        {
            StartCoroutine(FadePanel(mainMenuPanel, 0f, false));
            StartCoroutine(FadePanel(inGameHUDPanel, 1f, true));
        }

        public void OnLevelWonTransition()
        {
            StartCoroutine(FadePanel(inGameHUDPanel, 0f, false));
            StartCoroutine(FadePanel(winPanel, 1f, true));
        }

        public void OnLevelLostTransition()
        {
            StartCoroutine(FadePanel(inGameHUDPanel, 0f, false));
            StartCoroutine(FadePanel(losePanel, 1f, true));
        }

        /// <summary>
        /// Coroutine matrix to execute frame-rate independent alpha lerping for juicy visuals.
        /// </summary>
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