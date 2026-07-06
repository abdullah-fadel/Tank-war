using UnityEngine;

namespace TankAssault.Data
{
    [CreateAssetMenu(menuName = "TankAssault/Achievement Data", fileName = "Achievement_")]
    public class AchievementData : ScriptableObject
    {
        public string AchievementId;
        public string DisplayName;
        [TextArea] public string Description;
        public Sprite Icon;
        public int TargetAmount = 1;
        public int RewardCoins = 200;
        public int RewardGems = 5;
    }
}
