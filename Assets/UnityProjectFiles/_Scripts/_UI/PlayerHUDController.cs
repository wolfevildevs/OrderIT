using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace RunnerGame.UI
{
    /// <summary>
    /// Monitors and updates live gameplay HUD metrics (Level and multiple Currency texts) directly from PlayerDataSO.
    /// </summary>
    public class PlayerHUDController : MonoBehaviour
    {
        [Header("Data Link")]
        [SerializeField] private PlayerDataSO playerData;

        [Header("UI Text Mesh Pro Components")]
        [SerializeField] private TextMeshProUGUI levelText;
        
        [Header("Multiple Currency Text Displays")]
        [SerializeField] private List<TextMeshProUGUI> currencyTexts = new List<TextMeshProUGUI>();

        private void Start()
        {
            UpdateHUDGraphics();
        }

        private void Update()
        {
            UpdateHUDGraphics();
        }

        /// <summary>
        /// Reads current scriptable values and formats them cleanly into all registered UI text fields.
        /// </summary>
        private void UpdateHUDGraphics()
        {
            if (playerData == null) return;

            // Display current absolute human readable level layout sequence (Index 0 translates to Level 1)
            if (levelText != null)
            {
                levelText.text = $"LEVEL {playerData.currentLevelIndex + 1}";
            }

            // FIX: المرور على كل نصوص العملات المسجلة وتحديثها بالقيمة الحالية للمحفظة
            if (currencyTexts != null && currencyTexts.Count > 0)
            {
                for (int i = 0; i < currencyTexts.Count; i++)
                {
                    if (currencyTexts[i] != null)
                    {
                        currencyTexts[i].text = $"${playerData.totalWalletCurrency}";
                    }
                }
            }
        }
    }
}