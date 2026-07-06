using TankAssault.Core;
using TankAssault.Data;
using UnityEngine;

namespace TankAssault.Progression
{
    /// <summary>Unlocks one-time meta-achievements (e.g. "Kill 1000 enemies") independent of repeatable missions.</summary>
    public class AchievementManager : Singleton<AchievementManager>
    {
        [SerializeField] private AchievementData[] allAchievements;
        private int _totalKills;
        private int _totalLevelsCompleted;

        private void OnEnable()
        {
            EventBus.Subscribe<EnemyKilledEvent>(OnEnemyKilled);
            EventBus.Subscribe<LevelCompletedEvent>(OnLevelCompleted);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EnemyKilledEvent>(OnEnemyKilled);
            EventBus.Unsubscribe<LevelCompletedEvent>(OnLevelCompleted);
        }

        private void OnEnemyKilled(EnemyKilledEvent evt)
        {
            _totalKills++;
            CheckThresholdAchievements("kills", _totalKills);
        }

        private void OnLevelCompleted(LevelCompletedEvent evt)
        {
            _totalLevelsCompleted++;
            CheckThresholdAchievements("levels", _totalLevelsCompleted);
        }

        private void CheckThresholdAchievements(string category, int currentValue)
        {
            foreach (var achievement in allAchievements)
            {
                if (!achievement.AchievementId.StartsWith(category)) continue;
                if (currentValue < achievement.TargetAmount) continue;
                Unlock(achievement);
            }
        }

        private void Unlock(AchievementData achievement)
        {
            var save = SaveSystem.Instance.Current;
            if (save.UnlockedAchievementIds.Contains(achievement.AchievementId)) return;

            save.UnlockedAchievementIds.Add(achievement.AchievementId);
            Economy.CurrencyManager.Instance.AddCoins(achievement.RewardCoins);
            if (achievement.RewardGems > 0) Economy.CurrencyManager.Instance.AddGems(achievement.RewardGems);

            EventBus.Publish(new AchievementUnlockedEvent(achievement.AchievementId));
            SaveSystem.Instance.Save();
        }
    }
}
