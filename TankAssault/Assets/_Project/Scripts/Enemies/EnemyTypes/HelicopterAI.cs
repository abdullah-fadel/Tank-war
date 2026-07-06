using TankAssault.Core;
using TankAssault.Weapons;
using UnityEngine;

namespace TankAssault.Enemies.EnemyTypes
{
    /// <summary>
    /// Hovers high above the arena and drops bombs straight down when roughly over the player.
    /// BehaviourParamA = hover height, BehaviourParamB = horizontal tracking speed.
    /// </summary>
    public class HelicopterAI : EnemyBase
    {
        [SerializeField] private Transform bombDropPoint;
        private float _hoverHeight;
        private float _trackingSpeed;
        private float _attackTimer;

        protected override void Awake()
        {
            base.Awake();
            _hoverHeight = enemyData.BehaviourParamA > 0 ? enemyData.BehaviourParamA : 9f;
            _trackingSpeed = enemyData.BehaviourParamB > 0 ? enemyData.BehaviourParamB : 2f;
        }

        private void Update()
        {
            if (IsDead || PlayerTransform == null) return;
            if (DistanceToPlayer() > enemyData.DetectionRange) return;

            var pos = transform.position;
            pos.x = Mathf.MoveTowards(pos.x, PlayerTransform.position.x, _trackingSpeed * Time.deltaTime);
            pos.y = Mathf.Lerp(pos.y, PlayerTransform.position.y + _hoverHeight, Time.deltaTime * 1.5f);
            transform.position = pos;

            _attackTimer -= Time.deltaTime;
            bool overPlayer = Mathf.Abs(pos.x - PlayerTransform.position.x) < 1.5f;
            if (_attackTimer <= 0f && overPlayer)
            {
                DropBomb();
                _attackTimer = enemyData.AttackCooldown;
            }
        }

        private void DropBomb()
        {
            if (enemyData.ProjectilePrefab == null || bombDropPoint == null) return;
            var instance = PoolManager.Instance.Spawn(enemyData.ProjectilePrefab, bombDropPoint.position, Quaternion.identity);
            var projectile = instance.GetComponent<Projectile>();
            projectile?.Launch(bombDropPoint.position, Vector3.down, enemyData.ProjectileSpeed, enemyData.AttackDamage, enemyData.BehaviourParamC > 0 ? enemyData.BehaviourParamC : 4f, false, gameObject, enemyData.ProjectilePrefab);
        }
    }
}
