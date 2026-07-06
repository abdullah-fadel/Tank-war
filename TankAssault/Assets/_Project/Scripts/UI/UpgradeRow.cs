using System;
using TankAssault.Data;
using TankAssault.Economy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TankAssault.UI
{
    /// <summary>Single upgrade track row in the Garage: current level pips, cost, and an upgrade button.</summary>
    public class UpgradeRow : MonoBehaviour
    {
        [SerializeField] private UpgradeTrack track;
        [SerializeField] private TMP_Text trackNameLabel;
        [SerializeField] private TMP_Text levelLabel;
        [SerializeField] private TMP_Text costLabel;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private Slider levelBar;
        [SerializeField] private int maxLevel = 10;

        private UpgradeManager _upgradeManager;
        private Func<string> _getTankId;
        private Action<UpgradeRow> _onUpgraded;

        public void Initialize(UpgradeManager upgradeManager, Func<string> getTankId, Action<UpgradeRow> onUpgraded)
        {
            _upgradeManager = upgradeManager;
            _getTankId = getTankId;
            _onUpgraded = onUpgraded;
            trackNameLabel.text = track.ToString();
            upgradeButton.onClick.AddListener(OnUpgradeClicked);
        }

        public void Refresh()
        {
            string tankId = _getTankId();
            int level = _upgradeManager.GetLevel(tankId, track);
            int cost = _upgradeManager.GetUpgradeCost(tankId, track);

            levelLabel.text = $"Lv. {level}/{maxLevel}";
            levelBar.value = (float)level / maxLevel;
            costLabel.text = level >= maxLevel ? "MAX" : $"{cost}";
            upgradeButton.interactable = level < maxLevel && CurrencyManager.Instance.Coins >= cost;
        }

        private void OnUpgradeClicked()
        {
            string tankId = _getTankId();
            if (_upgradeManager.TryUpgrade(tankId, track))
            {
                StartCoroutine(UIAnimator.PunchScale(transform));
                _onUpgraded?.Invoke(this);
            }
        }
    }
}
