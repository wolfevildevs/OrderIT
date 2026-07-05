using UnityEngine;

namespace RunnerGame.Shop
{
    [CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Shop Item")]
    public class ShopItemSO : ScriptableObject
    {
        [Header("Identity Configuration")]
        public string itemId;
        public string itemName;
        public Sprite itemIcon;
        public string sceneObjectName;

        [Header("Economic Settings")]
        public int cost = 100;
    }
}