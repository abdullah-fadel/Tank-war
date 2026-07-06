using UnityEngine;

namespace TankAssault.Data
{
    public enum TankRarity
    {
        Common,
        Rare,
        Epic,
        Legendary
    }

    [CreateAssetMenu(menuName = "TankAssault/Tank Data", fileName = "TankData_")]
    public class TankData : ScriptableObject
    {
        [Header("Identity")]
        public string TankId;
        public string DisplayName;
        [TextArea] public string Description;
        public Sprite Icon;
        public GameObject ModelPrefab;
        public TankRarity Rarity;

        [Header("Base Stats")]
        public float BaseHealth = 100f;
        public float BaseArmor = 10f;
        public float BaseSpeed = 6f;
        public float BaseCriticalChance = 5f;
        public float TurretRotationSpeed = 120f;

        [Header("Loadout")]
        public WeaponData DefaultPrimaryWeapon;
        public WeaponData DefaultMissileWeapon;
        public string SkillId;
        public float SkillCooldownSeconds = 20f;

        [Header("Economy")]
        public int UnlockCostCoins;
        public int UnlockCostGems;
        public bool UnlockedByDefault;

        [Header("Audio")]
        public AudioClip EngineIdleLoop;
        public AudioClip EngineMovingLoop;
        public AudioClip DestroyedSfx;
    }
}
