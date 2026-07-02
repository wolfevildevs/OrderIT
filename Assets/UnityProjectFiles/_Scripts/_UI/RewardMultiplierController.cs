using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RunnerGame.Core;

namespace RunnerGame.UI
{
    /// <summary>
    /// Drives an oscillating UI gauge that lets the player multiply their run earnings 
    /// via simulated ads or luck mechanics.
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

        private bool isOscillating = true;
        private int baseRunCurrency = 0;
        private int finalSelectedMultiplier = 1;

        private void OnEnable()
        {
            if (playerData == null) return;
            
            baseRunCurrency = playerData.currentCurrencyEarnedInRun;
            isOscillating = true;
            finalSelectedMultiplier = 1;
            
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
            // Simple mapping: Center yields x5, edges yield x2, sweet spots yield x3
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
        /// Simulates Ad placement approval, halts oscillation, and commits multiplied credit parameters.
        /// </summary>
        public void TriggerAdDoublerSequence()
        {
            isOscillating = false;
            watchAdButton.interactable = false;

            // Calculate the bonus addition to avoid double-adding base earnings
            int bonusReward = (baseRunCurrency * finalSelectedMultiplier) - baseRunCurrency;
            
            if (playerData != null)
            {
                playerData.totalWalletCurrency += bonusReward;
                playerData.currentCurrencyEarnedInRun = baseRunCurrency * finalSelectedMultiplier;
            }

            Debug.Log($"Ad Completed! Final Reward committed with factor x{finalSelectedMultiplier}");
            CollectNormalEarningsAndClose();
        }

        public void CollectNormalEarningsAndClose()
        {
            // Transition back to progression workflow or next scene loading layout safely
            GameManager.Instance.NextLevel();
        }
    }
}