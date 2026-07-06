using TankAssault.Combat;
using TankAssault.Core;
using UnityEngine;

namespace TankAssault.LevelSystem
{
    /// <summary>Destructible environmental hazard: explodes on death, damaging nearby destructibles/enemies/player.</summary>
    public class ExplosiveBarrel : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 20f;
        [SerializeField] private float explosionDamage = 40f;
        [SerializeField] private float explosionRadius = 5f;
        [SerializeField] private GameObject explosionVfxPrefab;
        [SerializeField] private AudioClip explosionSfx;
        [SerializeField] private LayerMask damageMask;

        private float _currentHealth;
        public bool IsAlive => _currentHealth > 0f;

        private void Awake()
        {
            _currentHealth = maxHealth;
        }

        public void TakeDamage(DamageInfo damage)
        {
            if (!IsAlive) return;
            _currentHealth -= damage.Amount;
            if (_currentHealth <= 0f) Explode();
        }

        private void Explode()
        {
            var colliders = Physics.OverlapSphere(transform.position, explosionRadius, damageMask);
            foreach (var col in colliders)
            {
                var damageable = col.GetComponentInParent<IDamageable>();
                if (damageable == null || damageable == (IDamageable)this || !damageable.IsAlive) continue;

                float falloff = 1f - Mathf.Clamp01(Vector3.Distance(transform.position, col.transform.position) / explosionRadius);
                Vector3 dir = (col.transform.position - transform.position).normalized;
                damageable.TakeDamage(new DamageInfo(explosionDamage * falloff, transform.position, dir, false, true, gameObject));
            }

            if (explosionVfxPrefab != null)
                PoolManager.Instance.Spawn(explosionVfxPrefab, transform.position, Quaternion.identity);

            if (explosionSfx != null)
                AudioSource.PlayClipAtPoint(explosionSfx, transform.position);

            EventBus.Publish(new ExplosionEvent(transform.position, explosionRadius, 1.2f));

            gameObject.SetActive(false);
        }
    }
}
