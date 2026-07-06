using TankAssault.Data;
using TankAssault.Economy;
using UnityEngine;
using UnityEngine.UI;

namespace TankAssault.UI
{
    /// <summary>Shows all owned tanks and lets the player pick their equipped tank before entering a level.</summary>
    public class InventoryUI : UIScreen
    {
        [SerializeField] private TankData[] allTanks;
        [SerializeField] private Transform tankListContainer;
        [SerializeField] private TankCollectionCard cardPrefab;
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
            foreach (Transform child in tankListContainer)
                Destroy(child.gameObject);

            foreach (var tank in allTanks)
            {
                var card = Instantiate(cardPrefab, tankListContainer);
                bool owned = InventoryManager.Instance.OwnsTank(tank.TankId);
                bool equipped = InventoryManager.Instance.EquippedTankId == tank.TankId;
                card.Setup(tank, owned, equipped, () =>
                {
                    if (owned)
                    {
                        InventoryManager.Instance.EquipTank(tank.TankId);
                        Populate();
                    }
                });
            }
        }
    }
}
