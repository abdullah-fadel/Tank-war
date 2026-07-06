using TankAssault.Data;
using UnityEngine;

namespace TankAssault.Economy
{
    /// <summary>Handles purchase flow for tanks and weapons using coins or gems, delegating ownership state to InventoryManager.</summary>
    public class ShopManager : MonoBehaviour
    {
        [SerializeField] private TankData[] availableTanks;
        [SerializeField] private WeaponData[] availableWeapons;

        public TankData[] AvailableTanks => availableTanks;
        public WeaponData[] AvailableWeapons => availableWeapons;

        public bool TryPurchaseTank(TankData tank)
        {
            if (InventoryManager.Instance.OwnsTank(tank.TankId)) return false;

            if (tank.UnlockCostGems > 0)
            {
                if (!CurrencyManager.Instance.TrySpendGems(tank.UnlockCostGems)) return false;
            }
            else if (!CurrencyManager.Instance.TrySpendCoins(tank.UnlockCostCoins))
            {
                return false;
            }

            InventoryManager.Instance.UnlockTank(tank.TankId);
            return true;
        }

        public bool TryPurchaseWeapon(WeaponData weapon, System.Collections.Generic.List<string> unlockedWeaponIds)
        {
            if (unlockedWeaponIds.Contains(weapon.WeaponId)) return false;

            if (weapon.UnlockCostGems > 0)
            {
                if (!CurrencyManager.Instance.TrySpendGems(weapon.UnlockCostGems)) return false;
            }
            else if (!CurrencyManager.Instance.TrySpendCoins(weapon.UnlockCostCoins))
            {
                return false;
            }

            unlockedWeaponIds.Add(weapon.WeaponId);
            return true;
        }
    }
}
