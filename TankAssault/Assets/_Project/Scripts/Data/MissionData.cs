using UnityEngine;

namespace TankAssault.Data
{
    public enum MissionType
    {
        KillEnemies,
        KillSpecificEnemyType,
        CompleteLevels,
        EarnCoins,
        DealDamage,
        UseWeapon,
        SurviveTime,
        CollectStars,
        WinWithoutDamage
    }

    [CreateAssetMenu(menuName = "TankAssault/Mission Data", fileName = "Mission_")]
    public class MissionData : ScriptableObject
    {
        public string MissionId;
        public string DisplayName;
        [TextArea] public string Description;
        public MissionType Type;
        public string TargetFilterId; // e.g. specific EnemyType/WeaponId name for filtered missions
        public int TargetAmount = 10;
        public bool IsDaily;

        [Header("Rewards")]
        public int RewardCoins = 100;
        public int RewardXp = 50;
        public int RewardGems;
    }
}
