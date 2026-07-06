using TankAssault.Combat;
using UnityEngine;

namespace TankAssault.Enemies
{
    /// <summary>Applies EnemyData.ContactDamage to whatever the enemy's collider physically touches (typically the player hull).</summary>
    [RequireComponent(typeof(EnemyBase))]
    public class EnemyContactDamage : MonoBehaviour
    {
        private EnemyBase _enemyBase;

        private void Awake()
        {
            _enemyBase = GetComponent<EnemyBase>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (_enemyBase.IsDead || _enemyBase.Data == null) return;

            var damageable = collision.collider.GetComponentInParent<IDamageable>();
            if (damageable == null || damageable == (IDamageable)_enemyBase.Health || !damageable.IsAlive) return;

            Vector3 direction = (collision.transform.position - transform.position).normalized;
            damageable.TakeDamage(new DamageInfo(_enemyBase.Data.ContactDamage, transform.position, direction, false, false, gameObject));
        }
    }
}
