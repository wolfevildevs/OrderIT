using System.Collections.Generic;
using UnityEngine;
using RunnerGame.Player;
using RunnerGame.Audio;

namespace RunnerGame.Shop
{
    public class ShopManager : MonoBehaviour
    {
        [Header("Architecture Dependencies")]
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private List<ShopItemSO> availableShopItems; // ضع هنا كل العناصر التي صممتها

        [Header("UI Building Hierarchy")]
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private Transform cardsContainer; // الـ Content داخل الـ Scroll View

        [Header("In-Scene Actual Assets Matrix")]
        [SerializeField] private List<GameObject> actualSceneObjects; // اسحب كل مجسمات اللعبة الفعلية هنا

        private List<ShopCardUI> spawnedCards = new List<ShopCardUI>();

        public PlayerDataSO PlayerData => playerData;

        private void Start()
        {
            PopulateShopLayout();
            UpdateActualSceneObjectsPlacement();
        }

        /// <summary>
        /// Spawns database assets systematically into modern UI viewport grids.
        /// </summary>
        private void PopulateShopLayout()
        {
            foreach (Transform child in cardsContainer) Destroy(child.gameObject);
            spawnedCards.Clear();

            foreach (var item in availableShopItems)
            {
                GameObject cardInstance = Instantiate(cardPrefab, cardsContainer);
                ShopCardUI cardUI = cardInstance.GetComponent<ShopCardUI>();
                
                if (cardUI != null)
                {
                    cardUI.InitializeCard(item, this);
                    spawnedCards.Add(cardUI);
                }
            }
        }

        /// <summary>
        /// Centralized state processing workflow for buying or switching layout configurations securely.
        /// </summary>
        public void HandleCardInteraction(ShopItemSO item, ShopCardUI triggeredCard)
        {
            if (playerData.IsItemPurchased(item.itemId))
            {
                // تجهيز العنصر مباشرة لأنه مشترى بالفعل
                playerData.equippedItemId = item.itemId;
                Debug.Log($"Item equipped safely: {item.itemName}");
                AudioManager.Instance.PlaySFX("pick");
            }
            else
            {
                // عملية الشراء
                if (playerData.totalWalletCurrency >= item.cost)
                {
                    playerData.totalWalletCurrency -= item.cost; // خصم الفلوس
                    playerData.PurchaseItem(item.itemId);        // تسجيل الشراء دائمًا
                    playerData.equippedItemId = item.itemId;     // تجهيزه تلقائياً
                    AudioManager.Instance.PlaySFX("buy");
                    Debug.Log($"Purchase successful for: {item.itemName}");
                }
            }

            // تحديث واجهات كل البطاقات لتعكس التغيير الجديد والمحفظة
            RefreshAllCards();
            UpdateActualSceneObjectsPlacement();
        }

        private void RefreshAllCards()
        {
            foreach (var card in spawnedCards)
            {
                card.UpdateCardVisualState();
            }
        }

        /// <summary>
        /// Reads the equipped ID parameters, locates the matching name tag, and enforces physical toggle actions inside the scene.
        /// </summary>
        private void UpdateActualSceneObjectsPlacement()
        {
            // ابحث عن بيانات العنصر المجهز حالياً
            ShopItemSO equippedItemData = availableShopItems.Find(x => x.itemId == playerData.equippedItemId);
            
            string targetObjectName = equippedItemData != null ? equippedItemData.sceneObjectName : "";

            foreach (GameObject obj in actualSceneObjects)
            {
                if (obj == null) continue;

                // تفعيل المجسم فقط إذا كان اسمه يطابق الاسم المحدد في الـ ScriptableObject
                bool shouldBeActive = (obj.name == targetObjectName);
                obj.SetActive(shouldBeActive);
            }
        }
    }
}