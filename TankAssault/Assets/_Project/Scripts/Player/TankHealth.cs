using TankAssault.Combat;
using TankAssault.Core;
using TankAssault.Data;
using UnityEngine;

namespace TankAssault.Player
{
    public class TankHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private TankData tankData;
        [SerializeField] private float criticalDamageThresholdPercent = 25f;
        [SerializeField] private float criticalDamageMultiplier = 1.5f;

        public float MaxHealth { get; private set; }
        public float CurrentHealth { get; private set; }
        public float Armor { get; set; }
        public bool IsAlive => CurrentHealth > 0f;
        public bool IsCriticalState => IsAlive && (CurrentHealth / MaxHealth) * 100f <= criticalDamageThresholdPercent;

        private void Awake()
        {
            MaxHealth = tankData != null ? tankData.BaseHealth : 100f;
            Armor = tankData != null ? tankData.BaseArmor : 0f;
            CurrentHealth = MaxHealth;
        }

        public void SetMaxHealth(float newMax, bool healToFull = true)
        {
            MaxHealth = newMax;
            if (healToFull) CurrentHealth = MaxHealth;
            else CurrentHealth = Mathf.Min(CurrentHealth, MaxHealth);
        }

        public void TakeDamage(DamageInfo damage)
        {
            if (!IsAlive) return;

            float mitigated = Mathf.Max(1f, damage.Amount - Armor);
            if (IsCriticalState) mitigated *= criticalDamageMultiplier;

            CurrentHealth = Mathf.Max(0f, CurrentHealth - mitigated);

            EventBus.Publish(new PlayerDamagedEvent(CurrentHealth, MaxHealth, mitigated, damage.IsCritical));

            if (CurrentHealth <= 0f)
                Die();
        }

        public void Heal(float amount)
        {
            if (!IsAlive) return;
            CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
        }

        private void Die()
        {
            EventBus.Publish(new PlayerDiedEvent());
            var anim = GetComponent<TankAnimationController>();
            anim?.PlayDestroyed();
        }
    }
}
