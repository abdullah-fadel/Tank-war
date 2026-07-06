using UnityEngine;

namespace TankAssault.Data
{
    public enum EnemyType
    {
        LightTank,
        HeavyTank,
        SniperTank,
        RocketTank,
        Drone,
        Helicopter,
        Jet,
        Turret,
        WalkingRobot,
        BossTank
    }

    public enum EnemyMovementKind
    {
        Ground,
        Flying,
        Stationary
    }

    [CreateAssetMenu(menuName = "TankAssault/Enemy Data", fileName = "EnemyData_")]
    public class EnemyData : ScriptableObject
    {
        [Header("Identity")]
        public string EnemyId;
        public string DisplayName;
        public EnemyType Type;
        public EnemyMovementKind MovementKind;
        public GameObject ModelPrefab;
        public bool IsBoss;

        [Header("Stats")]
        public float MaxHealth = 50f;
        public float Armor = 0f;
        public float MoveSpeed = 3f;
        public float ContactDamage = 10f;

        [Header("Attack")]
        public float AttackDamage = 8f;
        public float AttackRange = 12f;
        public float DetectionRange = 20f;
        public float AttackCooldown = 2f;
        public float ProjectileSpeed = 25f;
        public GameObject ProjectilePrefab;
        public GameObject MuzzleVfxPrefab;

        [Header("AI Tuning")]
        [Tooltip("Behaviour-specific tuning, e.g. sniper charge time, drone strafe distance, helicopter hover height.")]
        public float BehaviourParamA;
        public float BehaviourParamB;
        public float BehaviourParamC;

        [Header("Rewards")]
        public int CoinReward = 5;
        public int XpReward = 10;

        [Header("Audio/VFX")]
        public GameObject DeathVfxPrefab;
        public AudioClip DeathSfx;
        public AudioClip AttackSfx;
    }
}
