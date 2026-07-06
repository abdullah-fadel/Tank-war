using TankAssault.Data;
using UnityEditor;
using UnityEngine;

namespace TankAssault.EditorTools
{
    /// <summary>
    /// Generates the full roster of sample ScriptableObject content: 7 weapons, 20 tanks,
    /// 10 enemy types, and 8 world configs (with 3 levels each). Stats are reasonable
    /// starting points for balancing, not final numbers — tune freely in the Inspector
    /// afterwards, nothing reads these values except through the data assets themselves.
    /// </summary>
    public static class ContentDataGenerator
    {
        public const string WeaponDataFolder = "Assets/_Project/ScriptableObjects/Weapons";
        public const string TankDataFolder = "Assets/_Project/ScriptableObjects/Tanks";
        public const string EnemyDataFolder = "Assets/_Project/ScriptableObjects/Enemies";
        public const string WorldDataFolder = "Assets/_Project/ScriptableObjects/Levels";
        public const string UpgradeDataFolder = "Assets/_Project/ScriptableObjects/Upgrades";
        public const string MissionDataFolder = "Assets/_Project/ScriptableObjects/Missions";
        public const string AchievementDataFolder = "Assets/_Project/ScriptableObjects/Achievements";

        [MenuItem("Tank Assault/2. Generate Sample Content Data")]
        public static void GenerateAll()
        {
            EditorFolderUtil.EnsureFolder(WeaponDataFolder);
            EditorFolderUtil.EnsureFolder(TankDataFolder);
            EditorFolderUtil.EnsureFolder(EnemyDataFolder);
            EditorFolderUtil.EnsureFolder(WorldDataFolder);
            EditorFolderUtil.EnsureFolder(UpgradeDataFolder);
            EditorFolderUtil.EnsureFolder(MissionDataFolder);
            EditorFolderUtil.EnsureFolder(AchievementDataFolder);
            EditorFolderUtil.RefreshAndSave();

            GenerateWeapons();
            GenerateTanks();
            GenerateEnemies();
            GenerateWorlds();
            GenerateUpgrades();
            GenerateMissions();
            GenerateAchievements();

            EditorFolderUtil.RefreshAndSave();
            Debug.Log("[ContentDataGenerator] Sample content data generated under Assets/_Project/ScriptableObjects.");
        }

        private static T CreateAsset<T>(string folder, string fileName) where T : ScriptableObject
        {
            string path = $"{folder}/{fileName}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null) return existing;

            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }

        private static GameObject LoadProjectile(bool homing) =>
            AssetDatabase.LoadAssetAtPath<GameObject>(homing ? PrefabBuilder.HomingProjectilePath : PrefabBuilder.StandardProjectilePath);

        // ---------------------------------------------------------------- Weapons

        private static void GenerateWeapons()
        {
            CreateWeapon("MachineGun", WeaponType.MachineGun, DamageDeliveryMode.Hitscan, 4f, 8f, 0.5f, 0, 18f, 0f, 0f);
            CreateWeapon("Cannon", WeaponType.Cannon, DamageDeliveryMode.Projectile, 35f, 0.8f, 1.5f, 1, 25f, 30f, 2f);
            CreateWeapon("RocketLauncher", WeaponType.RocketLauncher, DamageDeliveryMode.Projectile, 55f, 0.5f, 2.5f, 3, 22f, 22f, 4f);
            CreateWeapon("Laser", WeaponType.Laser, DamageDeliveryMode.Hitscan, 6f, 12f, 0.3f, 0, 30f, 0f, 0f);
            CreateWeapon("Plasma", WeaponType.Plasma, DamageDeliveryMode.Projectile, 20f, 3f, 0.8f, 0, 24f, 45f, 1.5f);
            CreateWeapon("HomingMissile", WeaponType.HomingMissile, DamageDeliveryMode.Projectile, 60f, 0.4f, 3f, 2, 35f, 18f, 3.5f);
            CreateWeapon("FlameThrower", WeaponType.FlameThrower, DamageDeliveryMode.Continuous, 8f, 20f, 1f, 0, 8f, 0f, 0f);
        }

        private static void CreateWeapon(string id, WeaponType type, DamageDeliveryMode mode, float damage, float fireRate,
            float reload, int magazineSize, float range, float projectileSpeed, float splashRadius)
        {
            var weapon = CreateAsset<WeaponData>(WeaponDataFolder, $"WeaponData_{id}");
            weapon.WeaponId = $"weapon_{id.ToLowerInvariant()}";
            weapon.DisplayName = System.Text.RegularExpressions.Regex.Replace(id, "(\\B[A-Z])", " $1");
            weapon.Type = type;
            weapon.DeliveryMode = mode;
            weapon.BaseDamage = damage;
            weapon.FireRate = fireRate;
            weapon.ReloadTime = reload;
            weapon.MagazineSize = magazineSize;
            weapon.Range = range;
            weapon.ProjectileSpeed = projectileSpeed;
            weapon.SplashRadius = splashRadius;
            weapon.ConeAngleDegrees = 30f;
            weapon.DamageTickRate = 10f;
            weapon.TurnRateDegPerSec = 180f;
            weapon.UnlockCostCoins = 500;

            if (mode == DamageDeliveryMode.Projectile)
                weapon.ProjectilePrefab = LoadProjectile(type == WeaponType.HomingMissile);

            weapon.UpgradeLevels = new WeaponUpgradeLevel[5];
            for (int i = 0; i < 5; i++)
            {
                weapon.UpgradeLevels[i] = new WeaponUpgradeLevel
                {
                    Level = i + 1,
                    DamageMultiplier = 1f + i * 0.15f,
                    FireRateMultiplier = 1f + i * 0.08f,
                    ReloadMultiplier = 1f - i * 0.05f,
                    UpgradeCostCoins = 100 * (i + 1) * (i + 1)
                };
            }

            EditorUtility.SetDirty(weapon);
        }

        // ---------------------------------------------------------------- Tanks

        private static readonly string[] TankNames =
        {
            "Scout Raider", "Desert Fox", "Iron Wall", "Ridgeback", "Vanguard",
            "Ember Crusher", "Frostbite", "Nightshade", "Marauder", "Juggernaut",
            "Phantom Strike", "Titan Prime", "Ashfall", "Stormbreaker", "Obsidian Fang",
            "Warhammer", "Viper Coil", "Doomtrack", "Apex Predator", "Colossus Omega"
        };

        private static void GenerateTanks()
        {
            var cannon = AssetDatabase.LoadAssetAtPath<WeaponData>($"{WeaponDataFolder}/WeaponData_Cannon.asset");
            var missile = AssetDatabase.LoadAssetAtPath<WeaponData>($"{WeaponDataFolder}/WeaponData_HomingMissile.asset");

            for (int i = 0; i < TankNames.Length; i++)
            {
                string idSuffix = i == 0 ? "scout_01" : $"{i + 1:00}";
                string fileSuffix = TankNames[i].Replace(" ", "");
                var tank = CreateAsset<TankData>(TankDataFolder, $"TankData_{fileSuffix}");

                tank.TankId = $"tank_{idSuffix}";
                tank.DisplayName = TankNames[i];
                tank.Rarity = (TankRarity)Mathf.Min(3, i / 5);
                tank.BaseHealth = 100f + i * 12f;
                tank.BaseArmor = 5f + i * 2f;
                tank.BaseSpeed = 6f + (i % 5) * 0.4f;
                tank.BaseCriticalChance = 5f + (i % 4);
                tank.TurretRotationSpeed = 120f;
                tank.DefaultPrimaryWeapon = cannon;
                tank.DefaultMissileWeapon = missile;
                tank.SkillId = $"skill_{fileSuffix.ToLowerInvariant()}";
                tank.SkillCooldownSeconds = 20f;
                tank.UnlockedByDefault = i == 0;
                tank.UnlockCostCoins = i == 0 ? 0 : 800 * (i + 1);
                tank.UnlockCostGems = tank.Rarity == TankRarity.Legendary ? 150 : 0;

                EditorUtility.SetDirty(tank);
            }
        }

        // ---------------------------------------------------------------- Enemies

        private static void GenerateEnemies()
        {
            var standardProjectile = LoadProjectile(false);

            CreateEnemy("LightTank", EnemyType.LightTank, EnemyMovementKind.Ground, 40f, 0f, 4f, 12f, 8f, 16f, 1.5f, standardProjectile, 5, 10);
            CreateEnemy("HeavyTank", EnemyType.HeavyTank, EnemyMovementKind.Ground, 140f, 8f, 2f, 25f, 10f, 20f, 3f, standardProjectile, 12, 25, behaviourA: 3f);
            CreateEnemy("SniperTank", EnemyType.SniperTank, EnemyMovementKind.Ground, 55f, 0f, 3f, 40f, 30f, 25f, 3.5f, standardProjectile, 10, 20, behaviourA: 1.2f);
            CreateEnemy("RocketTank", EnemyType.RocketTank, EnemyMovementKind.Ground, 70f, 2f, 3.5f, 20f, 18f, 22f, 2.5f, standardProjectile, 10, 22, behaviourA: 3f, behaviourB: 0.25f, behaviourC: 3f);
            CreateEnemy("Drone", EnemyType.Drone, EnemyMovementKind.Flying, 25f, 0f, 5f, 15f, 18f, 14f, 1.5f, standardProjectile, 6, 12, behaviourA: 6f, behaviourB: 4f, behaviourC: 1.5f);
            CreateEnemy("Helicopter", EnemyType.Helicopter, EnemyMovementKind.Flying, 90f, 4f, 3f, 16f, 22f, 12f, 2f, standardProjectile, 14, 28, behaviourA: 9f, behaviourB: 2f, behaviourC: 4f);
            CreateEnemy("Jet", EnemyType.Jet, EnemyMovementKind.Flying, 60f, 0f, 12f, 16f, 20f, 30f, 1f, standardProjectile, 12, 24, behaviourA: 10f, behaviourB: 12f, behaviourC: 1.5f);
            CreateEnemy("Turret", EnemyType.Turret, EnemyMovementKind.Stationary, 80f, 6f, 0f, 22f, 24f, 15f, 2f, standardProjectile, 8, 18);
            CreateEnemy("WalkingRobot", EnemyType.WalkingRobot, EnemyMovementKind.Ground, 65f, 3f, 3.5f, 2.5f, 12f, 12f, 2f, null, 9, 20);
            CreateEnemy("BossTank", EnemyType.BossTank, EnemyMovementKind.Ground, 1200f, 15f, 2f, 30f, 35f, 18f, 8f, standardProjectile, 500, 1000, isBoss: true);
        }

        private static void CreateEnemy(string id, EnemyType type, EnemyMovementKind movementKind, float health, float armor,
            float moveSpeed, float attackDamage, float attackRange, float detectionRange, float attackCooldown,
            GameObject projectilePrefab, int coinReward, int xpReward,
            float behaviourA = 0f, float behaviourB = 0f, float behaviourC = 0f, bool isBoss = false)
        {
            var enemy = CreateAsset<EnemyData>(EnemyDataFolder, $"EnemyData_{id}");
            enemy.EnemyId = $"enemy_{id.ToLowerInvariant()}";
            enemy.DisplayName = System.Text.RegularExpressions.Regex.Replace(id, "(\\B[A-Z])", " $1");
            enemy.Type = type;
            enemy.MovementKind = movementKind;
            enemy.IsBoss = isBoss;
            enemy.MaxHealth = health;
            enemy.Armor = armor;
            enemy.MoveSpeed = moveSpeed;
            enemy.ContactDamage = attackDamage * 0.5f;
            enemy.AttackDamage = attackDamage;
            enemy.AttackRange = attackRange;
            enemy.DetectionRange = detectionRange;
            enemy.AttackCooldown = attackCooldown;
            enemy.ProjectileSpeed = 20f;
            enemy.ProjectilePrefab = projectilePrefab;
            enemy.BehaviourParamA = behaviourA;
            enemy.BehaviourParamB = behaviourB;
            enemy.BehaviourParamC = behaviourC;
            enemy.CoinReward = coinReward;
            enemy.XpReward = xpReward;

            EditorUtility.SetDirty(enemy);
        }

        // ---------------------------------------------------------------- Worlds

        private static readonly WorldTheme[] Themes =
        {
            WorldTheme.Desert, WorldTheme.Forest, WorldTheme.Snow, WorldTheme.City,
            WorldTheme.MilitaryBase, WorldTheme.Volcano, WorldTheme.Night, WorldTheme.Factory
        };

        private static void GenerateWorlds()
        {
            for (int i = 0; i < Themes.Length; i++)
            {
                var world = CreateAsset<WorldConfig>(WorldDataFolder, $"World_{Themes[i]}");
                world.WorldId = $"world_{Themes[i].ToString().ToLowerInvariant()}";
                world.DisplayName = Themes[i].ToString();
                world.Theme = Themes[i];
                world.RequiredWorldIndex = i == 0 ? -1 : i - 1;
                world.UnlockCostCoins = i == 0 ? 0 : 1000 * i;
                world.FogDensity = 0.008f;

                world.Levels = new LevelInfo[3];
                for (int lvl = 0; lvl < 3; lvl++)
                {
                    world.Levels[lvl] = new LevelInfo
                    {
                        LevelId = $"{world.WorldId}_level_{lvl + 1}",
                        DisplayName = $"{Themes[i]} {lvl + 1}",
                        Mode = lvl == 2 ? LevelMode.Boss : LevelMode.Story,
                        LevelLengthMeters = 150f + lvl * 50f,
                        BossEnemyId = lvl == 2 ? "enemy_bosstank" : null,
                        RecommendedPower = 80 + i * 20 + lvl * 10,
                        CoinsOnComplete = 50 + lvl * 25,
                        XpOnComplete = 100 + lvl * 50,
                        StarThresholdScore1 = 1000,
                        StarThresholdScore2 = 2500,
                        StarThresholdScore3 = 5000,
                        HasBridges = lvl % 2 == 0,
                        HasHills = true,
                        HasMovingElevators = lvl == 1,
                        HasExplosiveBarrels = true,
                        HasBreakableWalls = lvl == 1,
                        HasHiddenPaths = lvl == 2
                    };
                }

                EditorUtility.SetDirty(world);
            }
        }

        // ---------------------------------------------------------------- Upgrades

        private static void GenerateUpgrades()
        {
            CreateUpgrade(UpgradeTrack.Engine, 0.06f, 120, 1.3f);
            CreateUpgrade(UpgradeTrack.Armor, 0.08f, 100, 1.3f);
            CreateUpgrade(UpgradeTrack.Tracks, 0.05f, 90, 1.25f);
            CreateUpgrade(UpgradeTrack.Cannon, 0.1f, 150, 1.35f);
            CreateUpgrade(UpgradeTrack.MissileLauncher, 0.1f, 180, 1.35f);
            CreateUpgrade(UpgradeTrack.FireRate, 0.05f, 130, 1.3f);
            CreateUpgrade(UpgradeTrack.ReloadSpeed, 0.05f, 110, 1.3f);
            CreateUpgrade(UpgradeTrack.Health, 0.08f, 100, 1.3f);
            CreateUpgrade(UpgradeTrack.CriticalChance, 0.02f, 200, 1.4f);
        }

        private static void CreateUpgrade(UpgradeTrack track, float valuePerLevel, int baseCost, float growth)
        {
            var upgrade = CreateAsset<UpgradeData>(UpgradeDataFolder, $"Upgrade_{track}");
            upgrade.Track = track;
            upgrade.DisplayName = track.ToString();
            upgrade.MaxLevel = 10;
            upgrade.ValuePerLevel = valuePerLevel;
            upgrade.BaseCostCoins = baseCost;
            upgrade.CostGrowth = growth;
            EditorUtility.SetDirty(upgrade);
        }

        // ---------------------------------------------------------------- Missions

        private static void GenerateMissions()
        {
            CreateMission("daily_kill_20", "Destroy 20 enemies", MissionType.KillEnemies, null, 20, true, 150, 60, 0);
            CreateMission("daily_earn_500_coins", "Earn 500 coins", MissionType.EarnCoins, null, 500, true, 100, 40, 0);
            CreateMission("daily_complete_3_levels", "Complete 3 levels", MissionType.CompleteLevels, null, 3, true, 200, 80, 5);
            CreateMission("story_kill_100_light_tanks", "Destroy 100 Light Tanks", MissionType.KillSpecificEnemyType, "enemy_lighttank", 100, false, 500, 200, 10);
            CreateMission("story_use_flamethrower_50", "Land 50 Flame Thrower hits", MissionType.UseWeapon, "weapon_flamethrower", 50, false, 300, 150, 0);
        }

        private static void CreateMission(string id, string displayName, MissionType type, string filter, int target, bool isDaily, int coins, int xp, int gems)
        {
            var mission = CreateAsset<MissionData>(MissionDataFolder, $"Mission_{id}");
            mission.MissionId = id;
            mission.DisplayName = displayName;
            mission.Type = type;
            mission.TargetFilterId = filter;
            mission.TargetAmount = target;
            mission.IsDaily = isDaily;
            mission.RewardCoins = coins;
            mission.RewardXp = xp;
            mission.RewardGems = gems;
            EditorUtility.SetDirty(mission);
        }

        // ---------------------------------------------------------------- Achievements

        private static void GenerateAchievements()
        {
            CreateAchievement("kills_100", "Rookie Gunner", "Destroy 100 enemies", 100, 200, 5);
            CreateAchievement("kills_1000", "Battlefield Veteran", "Destroy 1,000 enemies", 1000, 1000, 20);
            CreateAchievement("kills_10000", "Legendary Destroyer", "Destroy 10,000 enemies", 10000, 5000, 100);
            CreateAchievement("levels_10", "Campaign Starter", "Complete 10 levels", 10, 300, 10);
            CreateAchievement("levels_50", "Campaign Master", "Complete 50 levels", 50, 1500, 40);
        }

        private static void CreateAchievement(string id, string displayName, string description, int target, int coins, int gems)
        {
            var achievement = CreateAsset<AchievementData>(AchievementDataFolder, $"Achievement_{id}");
            achievement.AchievementId = id;
            achievement.DisplayName = displayName;
            achievement.Description = description;
            achievement.TargetAmount = target;
            achievement.RewardCoins = coins;
            achievement.RewardGems = gems;
            EditorUtility.SetDirty(achievement);
        }
    }
}
