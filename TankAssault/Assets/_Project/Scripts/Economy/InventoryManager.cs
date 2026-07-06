using TankAssault.Core;

namespace TankAssault.Economy
{
    /// <summary>Tracks which tanks/weapons the player owns and which tank is currently equipped.</summary>
    public class InventoryManager : Singleton<InventoryManager>
    {
        public bool OwnsTank(string tankId) => SaveSystem.Instance.Current.UnlockedTankIds.Contains(tankId);

        public void UnlockTank(string tankId)
        {
            if (OwnsTank(tankId)) return;
            SaveSystem.Instance.Current.UnlockedTankIds.Add(tankId);
            SaveSystem.Instance.Save();
        }

        public void EquipTank(string tankId)
        {
            if (!OwnsTank(tankId)) return;
            SaveSystem.Instance.Current.EquippedTankId = tankId;
            SaveSystem.Instance.Save();
        }

        public string EquippedTankId => SaveSystem.Instance.Current.EquippedTankId;
    }
}
