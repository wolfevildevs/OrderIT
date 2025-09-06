using UnityEngine;

public class ShopManager : MonoBehaviour
{
    public ShopItemCard[] allItems;
    private ShopItemCard currentEquipped;

    void Start()
    {
        LoadEquippedItem();
    }

    public void EquipItem(ShopItemCard selected)
    {
        if (!selected.IsBought()) return;

        foreach (var item in allItems)
            item.Deactivate();

        selected.Activate();
        currentEquipped = selected;

        PlayerPrefs.SetString("EquippedItem", selected.itemID);
        PlayerPrefs.Save();
    }

    void LoadEquippedItem()
    {
        if (!PlayerPrefs.HasKey("EquippedItem")) return;

        string id = PlayerPrefs.GetString("EquippedItem");

        foreach (var item in allItems)
        {
            if (item.itemID == id && item.IsBought())
            {
                item.Activate();
                currentEquipped = item;
                break;
            }
        }
    }
}