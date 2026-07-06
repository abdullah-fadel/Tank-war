using TankAssault.Data;
using TankAssault.Economy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TankAssault.UI
{
    /// <summary>Upgrade Garage: spend coins to level up Engine/Armor/Tracks/Cannon/MissileLauncher/FireRate/ReloadSpeed/Health/CriticalChance for the equipped tank.</summary>
    public class GarageUI : UIScreen
    {
        [SerializeField] private UpgradeManager upgradeManager;
        [SerializeField] private UpgradeRow[] upgradeRows; // one row pre-wired per UpgradeTrack in the inspector
        [SerializeField] private TMP_Text tankNameLabel;
        [SerializeField] private Button closeButton;

        protected override void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(() => UIManager.Instance.ShowPage(UIScreenId.MainMenu));

            foreach (var row in upgradeRows)
                row.Initialize(upgradeManager, () => InventoryManager.Instance.EquippedTankId, RefreshRow);
        }

        public override void Show(bool instant = false)
        {
            base.Show(instant);
            tankNameLabel.text = InventoryManager.Instance.EquippedTankId;
            foreach (var row in upgradeRows)
                RefreshRow(row);
        }

        private void RefreshRow(UpgradeRow row)
        {
            row.Refresh();
        }
    }
}
