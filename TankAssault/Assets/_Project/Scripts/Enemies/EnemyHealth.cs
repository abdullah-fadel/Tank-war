using TankAssault.Combat;
using UnityEngine;

namespace TankAssault.Enemies
{
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        private EnemyBase _enemyBase;
        public float MaxHealth { get; private set; }
        public float CurrentHealth { get; private set; }
        public float Armor { get; private set; }
        public bool IsAlive => CurrentHealth > 0f;

        private void Awake()
        {
            _enemyBase = GetComponent<EnemyBase>();
        }

        private void OnEnable()
        {
            if (_enemyBase != null && _enemyBase.Data != null)
            {
                MaxHealth = _enemyBase.Data.MaxHealth;
                Armor = _enemyBase.Data.Armor;
            }
            CurrentHealth = MaxHealth;
        }

        public void TakeDamage(DamageInfo damage)
        {
            if (!IsAlive) return;

            float mitigated = Mathf.Max(1f, damage.Amount - Armor);
            CurrentHealth = Mathf.Max(0f, CurrentHealth - mitigated);

            if (CurrentHealth <= 0f)
                _enemyBase?.OnDeath();
        }
    }
}
