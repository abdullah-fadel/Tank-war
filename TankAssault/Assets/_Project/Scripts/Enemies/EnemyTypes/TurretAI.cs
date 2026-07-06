using TankAssault.Core;
using TankAssault.Weapons;
using UnityEngine;

namespace TankAssault.Enemies.EnemyTypes
{
    /// <summary>Stationary emplacement: does not move, rotates its barrel toward the player and fires with high accuracy once in range.</summary>
    public class TurretAI : EnemyBase
    {
        [SerializeField] private Transform barrelPivot;
        [SerializeField] private Transform muzzlePoint;
        [SerializeField] private float rotationSpeed = 90f;
        private float _attackTimer;

        private void Update()
        {
            if (IsDead || PlayerTransform == null) return;
            if (DistanceToPlayer() > enemyData.DetectionRange) return;

            if (barrelPivot != null)
            {
                float yaw = DirectionToPlayer().x >= 0f ? 90f : -90f;
                var target = Quaternion.Euler(0f, yaw, 0f);
                barrelPivot.localRotation = Quaternion.RotateTowards(barrelPivot.localRotation, target, rotationSpeed * Time.deltaTime);
            }

            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0f && DistanceToPlayer() <= enemyData.AttackRange)
            {
                Fire();
                _attackTimer = enemyData.AttackCooldown;
            }
        }

        private void Fire()
        {
            if (enemyData.ProjectilePrefab == null || muzzlePoint == null) return;
            var direction = DirectionToPlayer();
            var instance = PoolManager.Instance.Spawn(enemyData.ProjectilePrefab, muzzlePoint.position, Quaternion.LookRotation(direction));
            var projectile = instance.GetComponent<Projectile>();
            projectile?.Launch(muzzlePoint.position, direction, enemyData.ProjectileSpeed, enemyData.AttackDamage, 0f, false, gameObject, enemyData.ProjectilePrefab);
        }
    }
}
