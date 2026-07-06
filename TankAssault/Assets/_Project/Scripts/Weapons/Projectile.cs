using TankAssault.Combat;
using TankAssault.Core;
using UnityEngine;

namespace TankAssault.Weapons
{
    /// <summary>Pooled physical projectile: travels in a straight line, applies damage and optional splash on hit.</summary>
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] protected GameObject impactVfxPrefab;
        [SerializeField] protected LayerMask hitMask;
        [SerializeField] protected float lifeTime = 5f;

        protected Rigidbody Rb;
        protected float Damage;
        protected float SplashRadius;
        protected bool IsCritical;
        protected GameObject SourcePrefab;
        protected GameObject Owner;
        protected float LifeTimer;

        protected virtual void Awake()
        {
            Rb = GetComponent<Rigidbody>();
        }

        public virtual void Launch(Vector3 position, Vector3 direction, float speed, float damage, float splashRadius, bool isCritical, GameObject owner, GameObject sourcePrefab)
        {
            transform.position = position;
            transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            Rb.velocity = direction.normalized * speed;
            Damage = damage;
            SplashRadius = splashRadius;
            IsCritical = isCritical;
            Owner = owner;
            SourcePrefab = sourcePrefab;
            LifeTimer = 0f;
        }

        protected virtual void Update()
        {
            LifeTimer += Time.deltaTime;
            if (LifeTimer >= lifeTime)
                Despawn();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & hitMask) == 0) return;
            if (other.gameObject == Owner) return;

            ApplyDamage(other, transform.position, transform.forward);
            SpawnImpactVfx();
            Despawn();
        }

        protected void ApplyDamage(Collider hitCollider, Vector3 hitPoint, Vector3 hitDirection)
        {
            if (SplashRadius > 0f)
            {
                var colliders = Physics.OverlapSphere(hitPoint, SplashRadius, hitMask);
                foreach (var col in colliders)
                {
                    var damageable = col.GetComponentInParent<IDamageable>();
                    if (damageable == null || !damageable.IsAlive) continue;

                    float falloff = 1f - Mathf.Clamp01(Vector3.Distance(hitPoint, col.transform.position) / SplashRadius);
                    damageable.TakeDamage(new DamageInfo(Damage * falloff, hitPoint, hitDirection, IsCritical, true, Owner));
                }
            }
            else
            {
                var damageable = hitCollider.GetComponentInParent<IDamageable>();
                if (damageable != null && damageable.IsAlive)
                    damageable.TakeDamage(new DamageInfo(Damage, hitPoint, hitDirection, IsCritical, false, Owner));
            }

            if (SplashRadius > 0f)
                EventBus.Publish(new ExplosionEvent(hitPoint, SplashRadius, Mathf.Clamp(SplashRadius * 0.3f, 0.2f, 2f)));
        }

        protected void SpawnImpactVfx()
        {
            if (impactVfxPrefab != null)
                PoolManager.Instance.Spawn(impactVfxPrefab, transform.position, Quaternion.identity);
        }

        protected void Despawn()
        {
            if (SourcePrefab != null)
                PoolManager.Instance.Despawn(SourcePrefab, gameObject);
            else
                gameObject.SetActive(false);
        }
    }
}
