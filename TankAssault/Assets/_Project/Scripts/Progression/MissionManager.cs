using System.Collections.Generic;
using TankAssault.Core;
using TankAssault.Data;
using UnityEngine;

namespace TankAssault.Progression
{
    /// <summary>
    /// Drives mission/quest progress by listening to gameplay events. Missions are authored as
    /// MissionData ScriptableObjects; runtime progress is persisted per-id in SaveData.
    /// </summary>
    public class MissionManager : Singleton<MissionManager>
    {
        [SerializeField] private MissionData[] allMissions;

        private void OnEnable()
        {
            EventBus.Subscribe<EnemyKilledEvent>(OnEnemyKilled);
            EventBus.Subscribe<LevelCompletedEvent>(OnLevelCompleted);
            EventBus.Subscribe<CurrencyChangedEvent>(OnCurrencyChanged);
            EventBus.Subscribe<WeaponFiredEvent>(OnWeaponFired);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EnemyKilledEvent>(OnEnemyKilled);
            EventBus.Unsubscribe<LevelCompletedEvent>(OnLevelCompleted);
            EventBus.Unsubscribe<CurrencyChangedEvent>(OnCurrencyChanged);
            EventBus.Unsubscribe<WeaponFiredEvent>(OnWeaponFired);
        }

        private MissionSaveEntry GetOrCreateEntry(string missionId)
        {
            var save = SaveSystem.Instance.Current;
            var entry = save.MissionProgress.Find(e => e.MissionId == missionId);
            if (entry == null)
            {
                entry = new MissionSaveEntry { MissionId = missionId };
                save.MissionProgress.Add(entry);
            }
            return entry;
        }

        private void AddProgress(MissionType type, string filterId, int amount)
        {
            foreach (var mission in allMissions)
            {
                if (mission.Type != type) continue;
                if (!string.IsNullOrEmpty(mission.TargetFilterId) && mission.TargetFilterId != filterId) continue;

                var entry = GetOrCreateEntry(mission.MissionId);
                if (entry.Completed) continue;

                entry.Progress = Mathf.Min(mission.TargetAmount, entry.Progress + amount);
                bool justCompleted = entry.Progress >= mission.TargetAmount;
                entry.Completed = justCompleted;

                EventBus.Publish(new MissionProgressEvent(mission.MissionId, entry.Progress, mission.TargetAmount, justCompleted));

                if (justCompleted)
                    GrantReward(mission);
            }
        }

        private void GrantReward(MissionData mission)
        {
            Economy.CurrencyManager.Instance.AddCoins(mission.RewardCoins);
            if (mission.RewardGems > 0) Economy.CurrencyManager.Instance.AddGems(mission.RewardGems);
            XPManager.Instance.AddXp(mission.RewardXp);
        }

        public void ClaimMission(string missionId)
        {
            var entry = GetOrCreateEntry(missionId);
            if (entry.Completed && !entry.Claimed)
            {
                entry.Claimed = true;
                SaveSystem.Instance.Save();
            }
        }

        private void OnEnemyKilled(EnemyKilledEvent evt)
        {
            AddProgress(MissionType.KillEnemies, null, 1);
            AddProgress(MissionType.KillSpecificEnemyType, evt.EnemyId, 1);
        }

        private void OnLevelCompleted(LevelCompletedEvent evt)
        {
            AddProgress(MissionType.CompleteLevels, null, 1);
        }

        private void OnCurrencyChanged(CurrencyChangedEvent evt)
        {
            // Simplified: mission progress for coin-earning missions is driven directly by
            // CurrencyManager.AddCoins call sites publishing deltas in a full implementation.
        }

        private void OnWeaponFired(WeaponFiredEvent evt)
        {
            AddProgress(MissionType.UseWeapon, evt.WeaponId, 1);
        }
    }
}
