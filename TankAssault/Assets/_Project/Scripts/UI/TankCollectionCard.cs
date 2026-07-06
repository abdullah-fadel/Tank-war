using System;
using TankAssault.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TankAssault.UI
{
    /// <summary>Card representing one of the 20+ collectible tanks: portrait, rarity frame, name, and lock/equip state.</summary>
    public class TankCollectionCard : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Image rarityFrame;
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private GameObject lockedOverlay;
        [SerializeField] private GameObject equippedBadge;
        [SerializeField] private Button selectButton;

        [Header("Rarity Colors")]
        [SerializeField] private Color commonColor = Color.gray;
        [SerializeField] private Color rareColor = new Color(0.3f, 0.6f, 1f);
        [SerializeField] private Color epicColor = new Color(0.7f, 0.3f, 1f);
        [SerializeField] private Color legendaryColor = new Color(1f, 0.7f, 0.1f);

        public void Setup(TankData tank, bool owned, bool equipped, Action onSelect)
        {
            nameLabel.text = tank.DisplayName;
            icon.sprite = tank.Icon;
            lockedOverlay.SetActive(!owned);
            equippedBadge.SetActive(equipped);

            rarityFrame.color = tank.Rarity switch
            {
                TankRarity.Rare => rareColor,
                TankRarity.Epic => epicColor,
                TankRarity.Legendary => legendaryColor,
                _ => commonColor
            };

            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onSelect());
        }
    }
}
