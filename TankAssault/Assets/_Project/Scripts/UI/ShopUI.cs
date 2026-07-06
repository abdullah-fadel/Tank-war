using TankAssault.Data;
using TankAssault.Economy;
using UnityEngine;
using UnityEngine.UI;

namespace TankAssault.UI
{
    /// <summary>Displays purchasable tanks/weapons for coins or gems. Uses a single row prefab instantiated per item.</summary>
    public class ShopUI : UIScreen
    {
        [SerializeField] private ShopManager shopManager;
        [SerializeField] private Transform itemListContainer;
        [SerializeField] private ShopItemRow itemRowPrefab;
        [SerializeField] private Button closeButton;

        protected override void Awake()
        {
            base.Awake();
            closeButton.onClick.AddListener(() => UIManager.Instance.ShowPage(UIScreenId.MainMenu));
        }

        public override void Show(bool instant = false)
        {
            base.Show(instant);
            Populate();
        }

        private void Populate()
        {
            foreach (Transform child in itemListContainer)
                Destroy(child.gameObject);

            foreach (var tank in shopManager.AvailableTanks)
            {
                var row = Instantiate(itemRowPrefab, itemListContainer);
                bool owned = InventoryManager.Instance.OwnsTank(tank.TankId);
                row.Setup(tank.DisplayName, tank.Icon, tank.UnlockCostCoins, tank.UnlockCostGems, owned,
                    () => shopManager.TryPurchaseTank(tank));
            }
        }
    }
}
