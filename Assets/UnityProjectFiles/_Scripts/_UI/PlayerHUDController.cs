using UnityEngine;
using TMPro;

namespace RunnerGame.UI
{
    /// <summary>
    /// Monitors and updates the live active gameplay HUD metrics (Level and Currency) directly from PlayerDataSO.
    /// </summary>
    public class PlayerHUDController : MonoBehaviour
    {
        [Header("Data Link")]
        [SerializeField] private PlayerDataSO playerData;

        [Header("UI Text Mesh Pro Components")]
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI currencyText;

        private void Start()
        {
            UpdateHUDGraphics();
        }

        private void Update()
        {
            UpdateHUDGraphics();
        }

        /// <summary>
        /// Reads current scriptable values and formats them cleanly into the UI text fields.
        /// </summary>
        private void UpdateHUDGraphics()
        {
            if (playerData == null) return;

            // Display current absolute human readable level layout sequence (Index 0 translates to Level 1)
            if (levelText != null)
            {
                levelText.text = $"LEVEL {playerData.currentLevelIndex + 1}";
            }

            // Sync total progressive master wallet metrics
            if (currencyText != null)
            {
                currencyText.text = $"${playerData.totalWalletCurrency}";
            }
        }
    }
}