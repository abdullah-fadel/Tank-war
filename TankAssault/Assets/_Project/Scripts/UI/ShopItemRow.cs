using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TankAssault.UI
{
    /// <summary>Single purchasable row: icon, name, price, and a buy button whose behaviour is injected by the owning screen.</summary>
    public class ShopItemRow : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private TMP_Text priceLabel;
        [SerializeField] private Button buyButton;
        [SerializeField] private GameObject ownedBadge;

        public void Setup(string displayName, Sprite iconSprite, int coinCost, int gemCost, bool owned, Func<bool> onBuy)
        {
            nameLabel.text = displayName;
            icon.sprite = iconSprite;
            priceLabel.text = gemCost > 0 ? $"{gemCost} Gems" : $"{coinCost} Coins";

            ownedBadge.SetActive(owned);
            buyButton.gameObject.SetActive(!owned);

            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() =>
            {
                if (onBuy())
                {
                    ownedBadge.SetActive(true);
                    buyButton.gameObject.SetActive(false);
                }
            });
        }
    }
}
