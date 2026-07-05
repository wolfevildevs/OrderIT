using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RunnerGame.Core;

namespace RunnerGame.UI
{
    /// <summary>
    /// Drives an oscillating UI gauge that lets the player multiply their run earnings 
    /// via simulated ads or luck mechanics with a built-in transition delay.
    /// </summary>
    public class RewardMultiplierController : MonoBehaviour
    {
        [Header("Data Link")]
        [SerializeField] private PlayerDataSO playerData;

        [Header("UI Component Matrix")]
        [SerializeField] private RectTransform pointerTransform;
        [SerializeField] private Button watchAdButton;
        [SerializeField] private Button claimNormalButton;
        [SerializeField] private TextMeshProUGUI multipliedPreviewText;

        [Header("Oscillation Configuration")]
        [SerializeField] private float pingPongSpeed = 3f;
        [SerializeField] private float leftLimitX = -150f;
        [SerializeField] private float rightLimitX = 150f;

        [Header("Transition Settings")]
        [SerializeField] private float postAdDelay = 3f; // 3-second delay configuration

        private bool isOscillating = true;
        private int baseRunCurrency = 0;
        private int finalSelectedMultiplier = 1;

        private void OnEnable()
        {
            if (playerData == null) return;
            
            // Fetch the baseline currency earned during this specific run safely
            baseRunCurrency = playerData.currentCurrencyEarnedInRun;
            isOscillating = true;
            finalSelectedMultiplier = 1;
            
            // Re-enable interactions upon opening the panel layout securely
            watchAdButton.interactable = true;
            claimNormalButton.interactable = true;

            watchAdButton.onClick.RemoveAllListeners();
            claimNormalButton.onClick.RemoveAllListeners();
            
            watchAdButton.onClick.AddListener(TriggerAdDoublerSequence);
            claimNormalButton.onClick.AddListener(CollectNormalEarningsAndClose);

            UpdatePreviewText(1);
        }

        private void Update()
        {
            if (!isOscillating || pointerTransform == null) return;

            // Oscillate the UI pointer continuously back and forth using Mathf.PingPong
            float lerpFactor = Mathf.PingPong(Time.time * pingPongSpeed, 1f);
            float currentX = Mathf.Lerp(leftLimitX, rightLimitX, lerpFactor);
            pointerTransform.anchoredPosition = new Vector2(currentX, pointerTransform.anchoredPosition.y);

            // Dynamically evaluate what multiplier the pointer is currently hovering over
            EvaluateCurrentMultiplier(lerpFactor);
        }

        private void EvaluateCurrentMultiplier(float normalizedPosition)
        {
            if (normalizedPosition >= 0.4f && normalizedPosition <= 0.6f)
            {
                finalSelectedMultiplier = 5;
            }
            else if ((normalizedPosition >= 0.2f && normalizedPosition < 0.4f) || (normalizedPosition > 0.6f && normalizedPosition <= 0.8f))
            {
                finalSelectedMultiplier = 3;
            }
            else
            {
                finalSelectedMultiplier = 2;
            }

            UpdatePreviewText(finalSelectedMultiplier);
        }

        private void UpdatePreviewText(int multiplier)
        {
            if (multipliedPreviewText != null)
            {
                multipliedPreviewText.text = $"CLAIM ${baseRunCurrency * multiplier} (x{multiplier})";
            }
        }

        /// <summary>
        /// Halts oscillation, adds FULL multiplied earnings to the wallet, and triggers the 3-second delay coroutine.
        /// </summary>
        public void TriggerAdDoublerSequence()
        {
            isOscillating = false;
            watchAdButton.interactable = false;
            claimNormalButton.interactable = false;

            // Calculate full multiplied reward explicitly
            int finalMultipliedReward = baseRunCurrency * finalSelectedMultiplier;
            
            if (playerData != null)
            {
                // ADDING THE WALLET CURRENCY OFFICIALLY HERE!
                playerData.totalWalletCurrency += finalMultipliedReward;
                playerData.currentCurrencyEarnedInRun = finalMultipliedReward;
            }

            Debug.Log($"Ad Completed! Final Multiplied Reward committed: {finalMultipliedReward}. Waiting {postAdDelay} seconds...");
            
            // FIX: Start the coroutine to safely wait before transitioning to the next scene
            StartCoroutine(WaitAndLoadNextLevel());
        }

        /// <summary>
        /// Commits standard base earnings without multipliers and advances level immediately.
        /// </summary>
        public void CollectNormalEarningsAndClose()
        {
            isOscillating = false;
            watchAdButton.interactable = false;
            claimNormalButton.interactable = false;

            if (playerData != null)
            {
                // ADDING THE BASE VALUE OFFICIALLY HERE FOR NORMAL CLAIM!
                playerData.totalWalletCurrency += baseRunCurrency;
            }

            Debug.Log($"Normal Claim! Base Reward committed: {baseRunCurrency}");
            
            // No delay needed for normal claim, transitions instantly
            GameManager.Instance.NextLevel();
        }

        /// <summary>
        /// Coroutine to execute the systematic delay before loading the next level index.
        /// </summary>
        private IEnumerator WaitAndLoadNextLevel()
        {
            yield return new WaitForSeconds(postAdDelay);
            GameManager.Instance.NextLevel();
        }
    }
}