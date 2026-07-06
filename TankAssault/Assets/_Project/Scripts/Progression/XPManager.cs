using TankAssault.Core;
using UnityEngine;

namespace TankAssault.Progression
{
    /// <summary>Tracks total XP and derives player level from a simple quadratic curve.</summary>
    public class XPManager : Singleton<XPManager>
    {
        [SerializeField] private int baseXpPerLevel = 100;
        [SerializeField] private float curveExponent = 1.35f;

        public int TotalXp => SaveSystem.Instance.Current.TotalXp;
        public int Level => SaveSystem.Instance.Current.PlayerLevel;

        private void OnEnable()
        {
            EventBus.Subscribe<EnemyKilledEvent>(OnEnemyKilled);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EnemyKilledEvent>(OnEnemyKilled);
        }

        private void OnEnemyKilled(EnemyKilledEvent evt)
        {
            AddXp(evt.XpAwarded);
        }

        public void AddXp(int amount)
        {
            if (amount <= 0) return;

            var save = SaveSystem.Instance.Current;
            save.TotalXp += amount;

            bool leveledUp = false;
            while (save.TotalXp >= XpRequiredForLevel(save.PlayerLevel + 1))
            {
                save.PlayerLevel++;
                leveledUp = true;
            }

            EventBus.Publish(new XpGainedEvent(save.TotalXp, save.PlayerLevel, leveledUp));
        }

        public int XpRequiredForLevel(int level)
        {
            return Mathf.RoundToInt(baseXpPerLevel * Mathf.Pow(level, curveExponent));
        }
    }
}
