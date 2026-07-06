using UnityEngine;

namespace TankAssault.Data
{
    public enum UpgradeTrack
    {
        Engine,
        Armor,
        Tracks,
        Cannon,
        MissileLauncher,
        FireRate,
        ReloadSpeed,
        Health,
        CriticalChance
    }

    [CreateAssetMenu(menuName = "TankAssault/Upgrade Track Data", fileName = "Upgrade_")]
    public class UpgradeData : ScriptableObject
    {
        public UpgradeTrack Track;
        public string DisplayName;
        public Sprite Icon;
        public int MaxLevel = 10;

        [Tooltip("Effect value added/multiplied per level, interpreted per-track by UpgradeManager.")]
        public float ValuePerLevel = 0.05f;

        [Tooltip("Coin cost for level N is BaseCost * CostGrowth^(N-1).")]
        public int BaseCostCoins = 100;
        public float CostGrowth = 1.35f;

        public int GetCostForLevel(int currentLevel)
        {
            return Mathf.RoundToInt(BaseCostCoins * Mathf.Pow(CostGrowth, currentLevel));
        }
    }
}
