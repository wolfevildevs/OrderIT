using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RunnerGame.Shop
{
    public class ShopCardUI : MonoBehaviour
    {
        [Header("UI Fields Layout")]
        [SerializeField] private Image itemIconImage;
        [SerializeField] private TextMeshProUGUI itemNameText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Button actionButton;
        [SerializeField] private TextMeshProUGUI buttonText;

        private ShopItemSO currentItemData;
        private ShopManager shopManager;

        /// <summary>
        /// Dynamically injects item properties into the UI Card layout architecture.
        /// </summary>
        public void InitializeCard(ShopItemSO itemData, ShopManager manager)
        {
            currentItemData = itemData;
            shopManager = manager;

            itemNameText.text = currentItemData.itemName;
            itemIconImage.sprite = currentItemData.itemIcon;

            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(OnCardButtonClicked);

            UpdateCardVisualState();
        }

        /// <summary>
        /// Refreshes the structural button feedback based on continuous PlayerData wallet states.
        /// </summary>
        public void UpdateCardVisualState()
        {
            PlayerDataSO playerData = shopManager.PlayerData;

            if (playerData.equippedItemId == currentItemData.itemId)
            {
                // العنصر مجهز حالياً
                actionButton.interactable = false;
                costText.gameObject.SetActive(false);
                buttonText.text = "EQUIPPED";
            }
            else if (playerData.IsItemPurchased(currentItemData.itemId))
            {
                // العنصر مشتري ولكن غير مجهز
                actionButton.interactable = true;
                costText.gameObject.SetActive(false);
                buttonText.text = "EQUIP";
            }
            else
            {
                // العنصر مغلق ويحتاج شراء
                actionButton.interactable = playerData.totalWalletCurrency >= currentItemData.cost;
                costText.gameObject.SetActive(true);
                costText.text = $"${currentItemData.cost}";
                buttonText.text = "BUY";
            }
        }

        private void OnCardButtonClicked()
        {
            shopManager.HandleCardInteraction(currentItemData, this);
        }
    }
}