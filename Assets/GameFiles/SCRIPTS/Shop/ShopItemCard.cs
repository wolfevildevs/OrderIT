using UnityEngine;
using UnityEngine.UI;

public class ShopItemCard : MonoBehaviour
{
    public string itemID;
    public GameObject itemModel;
    public Button buyButton;
    public Button equipButton;
    public int price = 10; // 👈 السعر الافتراضي

    private ShopManager manager;

    [Header("Audio")]
    public AudioSource buySound;
    public AudioSource noMoneySound;
    public AudioSource equipSound;

    void Start()
    {
        manager = FindObjectOfType<ShopManager>();

        bool isBought = PlayerPrefs.GetInt("Bought_" + itemID, 0) == 1;

        buyButton.onClick.AddListener(() => BuyItem());
        equipButton.onClick.AddListener(() => manager.EquipItem(this));

        buyButton.gameObject.SetActive(!isBought);
        equipButton.gameObject.SetActive(isBought);

        itemModel.SetActive(false);
    }

    public void BuyItem()
    {
        int currentMoney = PlayerPrefs.GetInt("Money", 0);
        if (currentMoney >= price)
        {
            PlayerPrefs.SetInt("Bought_" + itemID, 1);
            PlayerPrefs.SetInt("Money", currentMoney - price);
            PlayerPrefs.Save();

            if (buySound != null) buySound.Play();

            buyButton.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(true);

            // تحديث واجهة المال
            PlayerMovement player = FindObjectOfType<PlayerMovement>();
            if (player != null) player.UpdateMoneyUI(currentMoney - price);
        }
        else
        {
            if (noMoneySound != null) noMoneySound.Play();
        }
    }

    public void Activate()
    {
        itemModel.SetActive(true);
        if (equipSound != null) equipSound.Play();
    }

    public void Deactivate()
    {
        itemModel.SetActive(false);
    }

    public bool IsBought()
    {
        return PlayerPrefs.GetInt("Bought_" + itemID, 0) == 1;
    }
}