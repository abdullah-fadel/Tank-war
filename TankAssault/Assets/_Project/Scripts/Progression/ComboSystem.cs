using TankAssault.Core;
using UnityEngine;

namespace TankAssault.Progression
{
    /// <summary>
    /// Tracks consecutive kills within a rolling time window. Each kill increases the combo
    /// count and the score multiplier; the combo resets if the player goes too long without a kill.
    /// </summary>
    public class ComboSystem : Singleton<ComboSystem>
    {
        [SerializeField] private float comboResetSeconds = 4f;
        [SerializeField] private float multiplierPerCombo = 0.1f;
        [SerializeField] private float maxMultiplier = 4f;

        private float _comboTimer;
        public int ComboCount { get; private set; }
        public float ScoreMultiplier => Mathf.Min(1f + ComboCount * multiplierPerCombo, maxMultiplier);

        private void OnEnable()
        {
            EventBus.Subscribe<EnemyKilledEvent>(OnEnemyKilled);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EnemyKilledEvent>(OnEnemyKilled);
        }

        private void Update()
        {
            if (ComboCount == 0) return;

            _comboTimer -= Time.deltaTime;
            if (_comboTimer <= 0f)
                ResetCombo();
        }

        private void OnEnemyKilled(EnemyKilledEvent evt)
        {
            ComboCount++;
            _comboTimer = comboResetSeconds;

            if (GameManager.Instance != null)
                GameManager.Instance.CurrentCombo = ComboCount;

            EventBus.Publish(new ComboChangedEvent(ComboCount, ScoreMultiplier));
        }

        private void ResetCombo()
        {
            ComboCount = 0;
            if (GameManager.Instance != null)
                GameManager.Instance.CurrentCombo = 0;

            EventBus.Publish(new ComboChangedEvent(0, 1f));
        }
    }
}
