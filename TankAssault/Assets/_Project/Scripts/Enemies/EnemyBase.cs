using TankAssault.Core;
using TankAssault.Data;
using UnityEngine;

namespace TankAssault.Enemies
{
    /// <summary>
    /// Shared enemy plumbing: player reference, rewards, death handling, and pooled despawn.
    /// Concrete behaviour (movement + attack pattern) lives in the per-type AI scripts under
    /// Enemies/EnemyTypes, which drive a TankAssault.Core.StateMachine composed from the
    /// reusable states in Enemies/AIStates.
    /// </summary>
    [RequireComponent(typeof(EnemyHealth))]
    public class EnemyBase : MonoBehaviour
    {
        [SerializeField] protected EnemyData enemyData;
        [SerializeField] protected GameObject sourcePrefab;

        public EnemyData Data => enemyData;
        public Transform PlayerTransform { get; private set; }
        public EnemyHealth Health { get; private set; }
        public bool IsDead { get; private set; }

        protected virtual void Awake()
        {
            Health = GetComponent<EnemyHealth>();
        }

        protected virtual void OnEnable()
        {
            IsDead = false;
            var playerGo = GameObject.FindGameObjectWithTag("Player");
            PlayerTransform = playerGo != null ? playerGo.transform : null;
        }

        public float DistanceToPlayer()
        {
            if (PlayerTransform == null) return float.MaxValue;
            return Vector3.Distance(transform.position, PlayerTransform.position);
        }

        public Vector3 DirectionToPlayer()
        {
            if (PlayerTransform == null) return transform.forward;
            return (PlayerTransform.position - transform.position).normalized;
        }

        /// <summary>Called by EnemyHealth once HP reaches zero.</summary>
        public virtual void OnDeath()
        {
            if (IsDead) return;
            IsDead = true;

            EventBus.Publish(new EnemyKilledEvent(enemyData.EnemyId, transform.position, enemyData.CoinReward, enemyData.XpReward));

            if (enemyData.DeathVfxPrefab != null)
                PoolManager.Instance.Spawn(enemyData.DeathVfxPrefab, transform.position, Quaternion.identity);

            if (enemyData.DeathSfx != null)
                AudioSource.PlayClipAtPoint(enemyData.DeathSfx, transform.position);

            Invoke(nameof(Despawn), 0.15f);
        }

        private void Despawn()
        {
            if (sourcePrefab != null && PoolManager.Instance != null)
                PoolManager.Instance.Despawn(sourcePrefab, gameObject);
            else
                gameObject.SetActive(false);
        }
    }
}
