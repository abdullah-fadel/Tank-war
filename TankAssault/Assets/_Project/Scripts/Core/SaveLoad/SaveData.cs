using System;
using System.Collections.Generic;

namespace TankAssault.Core
{
    /// <summary>Root save document. Kept flat and JSON-friendly for easy versioning/migration.</summary>
    [Serializable]
    public class SaveData
    {
        public int SaveVersion = 1;

        // Economy
        public int Coins;
        public int Gems;

        // Progression
        public int PlayerLevel = 1;
        public int TotalXp;
        public List<string> UnlockedTankIds = new List<string> { "tank_scout_01" };
        public string EquippedTankId = "tank_scout_01";
        public List<string> UnlockedAchievementIds = new List<string>();
        public List<MissionSaveEntry> MissionProgress = new List<MissionSaveEntry>();

        // Upgrades: tankId -> upgrade track -> level
        public List<TankUpgradeEntry> TankUpgrades = new List<TankUpgradeEntry>();

        // Level progress
        public List<LevelProgressEntry> LevelProgress = new List<LevelProgressEntry>();
        public int HighestUnlockedWorldIndex;

        // Settings
        public float MusicVolume = 0.8f;
        public float SfxVolume = 1f;
        public float JoystickSensitivity = 1f;
        public bool InvertAim;
        public string GraphicsQuality = "Auto";

        // Daily reward
        public string LastDailyRewardUtc = "";
        public int DailyRewardStreak;

        public long LastSaveUnixTime;
    }

    [Serializable]
    public class MissionSaveEntry
    {
        public string MissionId;
        public int Progress;
        public bool Completed;
        public bool Claimed;
    }

    [Serializable]
    public class TankUpgradeEntry
    {
        public string TankId;
        public int EngineLevel;
        public int ArmorLevel;
        public int TracksLevel;
        public int CannonLevel;
        public int MissileLauncherLevel;
        public int FireRateLevel;
        public int ReloadSpeedLevel;
        public int HealthLevel;
        public int CriticalChanceLevel;
    }

    [Serializable]
    public class LevelProgressEntry
    {
        public string LevelId;
        public bool Completed;
        public int Stars;
        public int BestScore;
    }
}
