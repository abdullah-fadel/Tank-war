using UnityEngine;

namespace TankAssault.Core
{
    // Central catalogue of gameplay events published on the EventBus.
    // Kept as plain structs so publishing never allocates a class on the heap.

    public readonly struct PlayerDamagedEvent
    {
        public readonly float CurrentHealth;
        public readonly float MaxHealth;
        public readonly float DamageAmount;
        public readonly bool IsCritical;

        public PlayerDamagedEvent(float current, float max, float damage, bool isCritical)
        {
            CurrentHealth = current;
            MaxHealth = max;
            DamageAmount = damage;
            IsCritical = isCritical;
        }
    }

    public readonly struct PlayerDiedEvent { }

    public readonly struct EnemyKilledEvent
    {
        public readonly string EnemyId;
        public readonly Vector3 Position;
        public readonly int CoinsAwarded;
        public readonly int XpAwarded;

        public EnemyKilledEvent(string enemyId, Vector3 position, int coins, int xp)
        {
            EnemyId = enemyId;
            Position = position;
            CoinsAwarded = coins;
            XpAwarded = xp;
        }
    }

    public readonly struct WeaponFiredEvent
    {
        public readonly string WeaponId;
        public readonly Vector3 MuzzlePosition;

        public WeaponFiredEvent(string weaponId, Vector3 muzzlePosition)
        {
            WeaponId = weaponId;
            MuzzlePosition = muzzlePosition;
        }
    }

    public readonly struct ExplosionEvent
    {
        public readonly Vector3 Position;
        public readonly float Radius;
        public readonly float ShakeIntensity;

        public ExplosionEvent(Vector3 position, float radius, float shakeIntensity)
        {
            Position = position;
            Radius = radius;
            ShakeIntensity = shakeIntensity;
        }
    }

    public readonly struct CurrencyChangedEvent
    {
        public readonly int Coins;
        public readonly int Gems;

        public CurrencyChangedEvent(int coins, int gems)
        {
            Coins = coins;
            Gems = gems;
        }
    }

    public readonly struct XpGainedEvent
    {
        public readonly int TotalXp;
        public readonly int Level;
        public readonly bool LeveledUp;

        public XpGainedEvent(int totalXp, int level, bool leveledUp)
        {
            TotalXp = totalXp;
            Level = level;
            LeveledUp = leveledUp;
        }
    }

    public readonly struct ComboChangedEvent
    {
        public readonly int ComboCount;
        public readonly float ScoreMultiplier;

        public ComboChangedEvent(int comboCount, float scoreMultiplier)
        {
            ComboCount = comboCount;
            ScoreMultiplier = scoreMultiplier;
        }
    }

    public readonly struct MissionProgressEvent
    {
        public readonly string MissionId;
        public readonly int Progress;
        public readonly int Target;
        public readonly bool Completed;

        public MissionProgressEvent(string missionId, int progress, int target, bool completed)
        {
            MissionId = missionId;
            Progress = progress;
            Target = target;
            Completed = completed;
        }
    }

    public readonly struct AchievementUnlockedEvent
    {
        public readonly string AchievementId;
        public AchievementUnlockedEvent(string achievementId) => AchievementId = achievementId;
    }

    public readonly struct CheckpointReachedEvent
    {
        public readonly int CheckpointIndex;
        public CheckpointReachedEvent(int index) => CheckpointIndex = index;
    }

    public readonly struct LevelCompletedEvent
    {
        public readonly int Stars;
        public readonly int Score;
        public LevelCompletedEvent(int stars, int score)
        {
            Stars = stars;
            Score = score;
        }
    }

    public readonly struct GameStateChangedEvent
    {
        public readonly GameState PreviousState;
        public readonly GameState NewState;
        public GameStateChangedEvent(GameState previous, GameState next)
        {
            PreviousState = previous;
            NewState = next;
        }
    }
}
