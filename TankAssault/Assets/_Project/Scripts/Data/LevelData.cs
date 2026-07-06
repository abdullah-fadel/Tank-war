using UnityEngine;

namespace TankAssault.Data
{
    public enum WorldTheme
    {
        Desert,
        Forest,
        Snow,
        City,
        MilitaryBase,
        Volcano,
        Night,
        Factory
    }

    public enum LevelMode
    {
        Story,
        Endless,
        Survival,
        Boss
    }

    [CreateAssetMenu(menuName = "TankAssault/World Config", fileName = "World_")]
    public class WorldConfig : ScriptableObject
    {
        [Header("Identity")]
        public string WorldId;
        public string DisplayName;
        public WorldTheme Theme;
        public Sprite ThumbnailImage;

        [Header("Unlock")]
        public int RequiredWorldIndex = -1; // -1 = always unlocked
        public int UnlockCostCoins;

        [Header("Environment")]
        public AudioClip AmbientMusic;
        public Color FogColor = new Color(0.6f, 0.6f, 0.6f);
        public float FogDensity = 0.01f;
        public Gradient SkyGradient;

        [Header("Levels")]
        public LevelInfo[] Levels;
    }

    [System.Serializable]
    public class LevelInfo
    {
        public string LevelId;
        public string DisplayName;
        public LevelMode Mode;
        public float LevelLengthMeters = 200f;
        public string[] EnemyWaveIds;
        public string BossEnemyId;
        public int RecommendedPower = 100;
        public int CoinsOnComplete = 50;
        public int XpOnComplete = 100;
        public int StarThresholdScore1 = 1000;
        public int StarThresholdScore2 = 2500;
        public int StarThresholdScore3 = 5000;

        [Header("Environment Elements")]
        public bool HasBridges;
        public bool HasHills;
        public bool HasMovingElevators;
        public bool HasExplosiveBarrels;
        public bool HasBreakableWalls;
        public bool HasHiddenPaths;
    }
}
