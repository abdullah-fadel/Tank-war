using TankAssault.Core;
using TankAssault.Data;
using UnityEngine;

namespace TankAssault.Economy
{
    /// <summary>
    /// Applies coin-gated per-track upgrades (Engine, Armor, Tracks, Cannon, MissileLauncher,
    /// FireRate, ReloadSpeed, Health, CriticalChance) to a tank's persisted upgrade levels.
    /// </summary>
    public class UpgradeManager : Singleton<UpgradeManager>
    {
        [SerializeField] private UpgradeData[] upgradeTracks;

        public TankUpgradeEntry GetOrCreateEntry(string tankId)
        {
            var save = SaveSystem.Instance.Current;
            var entry = save.TankUpgrades.Find(e => e.TankId == tankId);
            if (entry == null)
            {
                entry = new TankUpgradeEntry { TankId = tankId };
                save.TankUpgrades.Add(entry);
            }
            return entry;
        }

        public int GetLevel(string tankId, UpgradeTrack track)
        {
            var entry = GetOrCreateEntry(tankId);
            return track switch
            {
                UpgradeTrack.Engine => entry.EngineLevel,
                UpgradeTrack.Armor => entry.ArmorLevel,
                UpgradeTrack.Tracks => entry.TracksLevel,
                UpgradeTrack.Cannon => entry.CannonLevel,
                UpgradeTrack.MissileLauncher => entry.MissileLauncherLevel,
                UpgradeTrack.FireRate => entry.FireRateLevel,
                UpgradeTrack.ReloadSpeed => entry.ReloadSpeedLevel,
                UpgradeTrack.Health => entry.HealthLevel,
                UpgradeTrack.CriticalChance => entry.CriticalChanceLevel,
                _ => 0
            };
        }

        private UpgradeData GetTrackData(UpgradeTrack track)
        {
            foreach (var data in upgradeTracks)
                if (data.Track == track) return data;
            return null;
        }

        public bool TryUpgrade(string tankId, UpgradeTrack track)
        {
            var trackData = GetTrackData(track);
            if (trackData == null) return false;

            int currentLevel = GetLevel(tankId, track);
            if (currentLevel >= trackData.MaxLevel) return false;

            int cost = trackData.GetCostForLevel(currentLevel);
            if (!CurrencyManager.Instance.TrySpendCoins(cost)) return false;

            var entry = GetOrCreateEntry(tankId);
            SetLevel(entry, track, currentLevel + 1);
            SaveSystem.Instance.Save();
            return true;
        }

        private void SetLevel(TankUpgradeEntry entry, UpgradeTrack track, int newLevel)
        {
            switch (track)
            {
                case UpgradeTrack.Engine: entry.EngineLevel = newLevel; break;
                case UpgradeTrack.Armor: entry.ArmorLevel = newLevel; break;
                case UpgradeTrack.Tracks: entry.TracksLevel = newLevel; break;
                case UpgradeTrack.Cannon: entry.CannonLevel = newLevel; break;
                case UpgradeTrack.MissileLauncher: entry.MissileLauncherLevel = newLevel; break;
                case UpgradeTrack.FireRate: entry.FireRateLevel = newLevel; break;
                case UpgradeTrack.ReloadSpeed: entry.ReloadSpeedLevel = newLevel; break;
                case UpgradeTrack.Health: entry.HealthLevel = newLevel; break;
                case UpgradeTrack.CriticalChance: entry.CriticalChanceLevel = newLevel; break;
            }
        }

        public int GetUpgradeCost(string tankId, UpgradeTrack track)
        {
            var trackData = GetTrackData(track);
            if (trackData == null) return 0;
            return trackData.GetCostForLevel(GetLevel(tankId, track));
        }
    }
}
