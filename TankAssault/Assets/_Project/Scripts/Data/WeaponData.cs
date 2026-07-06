using UnityEngine;

namespace TankAssault.Data
{
    public enum WeaponType
    {
        MachineGun,
        Cannon,
        RocketLauncher,
        Laser,
        Plasma,
        HomingMissile,
        FlameThrower
    }

    public enum DamageDeliveryMode
    {
        Projectile,     // physical projectile travels through the world (Cannon, Rocket, Missile, Plasma)
        Hitscan,        // instant raycast hit (MachineGun, Laser)
        Continuous      // damage-over-time cone/stream (FlameThrower)
    }

    [CreateAssetMenu(menuName = "TankAssault/Weapon Data", fileName = "WeaponData_")]
    public class WeaponData : ScriptableObject
    {
        [Header("Identity")]
        public string WeaponId;
        public string DisplayName;
        [TextArea] public string Description;
        public Sprite Icon;
        public WeaponType Type;
        public DamageDeliveryMode DeliveryMode;

        [Header("Base Stats")]
        public float BaseDamage = 10f;
        public float FireRate = 1f;          // shots per second
        public float ReloadTime = 1.5f;      // seconds, for burst-limited weapons (rockets/missiles)
        public int MagazineSize = 1;         // shots before reload; 0 = unlimited/continuous
        public float Range = 20f;
        public float ProjectileSpeed = 40f;
        public float SplashRadius = 0f;      // 0 = no AoE
        public float CriticalChanceBonus = 0f;

        [Header("Homing (Homing Missile only)")]
        public float TurnRateDegPerSec = 180f;
        public float HomingActivationDelay = 0.15f;

        [Header("Continuous (Flame Thrower only)")]
        public float ConeAngleDegrees = 30f;
        public float DamageTickRate = 10f;

        [Header("Economy")]
        public int UnlockCostCoins;
        public int UnlockCostGems;
        public WeaponUpgradeLevel[] UpgradeLevels;

        [Header("Presentation")]
        public GameObject ProjectilePrefab;
        public GameObject MuzzleFlashPrefab;
        public GameObject ImpactVfxPrefab;
        public AudioClip FireSfx;
        public AudioClip ImpactSfx;
        public AudioClip ReloadSfx;

        public WeaponUpgradeLevel GetUpgrade(int level)
        {
            if (UpgradeLevels == null || UpgradeLevels.Length == 0) return null;
            int index = Mathf.Clamp(level - 1, 0, UpgradeLevels.Length - 1);
            return UpgradeLevels[index];
        }

        public float GetDamageForLevel(int level)
        {
            var upgrade = GetUpgrade(level);
            return upgrade != null ? BaseDamage * upgrade.DamageMultiplier : BaseDamage;
        }
    }

    [System.Serializable]
    public class WeaponUpgradeLevel
    {
        public int Level = 1;
        public float DamageMultiplier = 1f;
        public float FireRateMultiplier = 1f;
        public float ReloadMultiplier = 1f;
        public int UpgradeCostCoins;
    }
}
