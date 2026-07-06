using System;
using TankAssault.Core;
using UnityEngine;

namespace TankAssault.Progression
{
    /// <summary>Grants an escalating daily login reward; streak resets if a day is missed.</summary>
    public class DailyRewardManager : Singleton<DailyRewardManager>
    {
        [SerializeField] private int[] coinRewardByStreakDay = { 50, 75, 100, 150, 200, 300, 500 };

        public bool IsRewardAvailable()
        {
            var save = SaveSystem.Instance.Current;
            if (string.IsNullOrEmpty(save.LastDailyRewardUtc)) return true;

            var last = DateTime.Parse(save.LastDailyRewardUtc, null, System.Globalization.DateTimeStyles.RoundtripKind);
            return DateTime.UtcNow.Date > last.Date;
        }

        public int ClaimDailyReward()
        {
            var save = SaveSystem.Instance.Current;
            if (!IsRewardAvailable()) return 0;

            bool missedADay = false;
            if (!string.IsNullOrEmpty(save.LastDailyRewardUtc))
            {
                var last = DateTime.Parse(save.LastDailyRewardUtc, null, System.Globalization.DateTimeStyles.RoundtripKind);
                missedADay = (DateTime.UtcNow.Date - last.Date).Days > 1;
            }

            save.DailyRewardStreak = missedADay ? 1 : save.DailyRewardStreak + 1;
            save.LastDailyRewardUtc = DateTime.UtcNow.ToString("o");

            int dayIndex = Mathf.Clamp(save.DailyRewardStreak - 1, 0, coinRewardByStreakDay.Length - 1);
            int reward = coinRewardByStreakDay[dayIndex];

            Economy.CurrencyManager.Instance.AddCoins(reward);
            SaveSystem.Instance.Save();
            return reward;
        }
    }
}
