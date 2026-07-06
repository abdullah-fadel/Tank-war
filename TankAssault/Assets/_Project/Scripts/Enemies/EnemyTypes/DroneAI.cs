using TankAssault.Core;
using TankAssault.Weapons;
using UnityEngine;

namespace TankAssault.Enemies.EnemyTypes
{
    /// <summary>
    /// Flying enemy that hovers at a fixed height and strafes side-to-side while firing.
    /// BehaviourParamA = hover height, BehaviourParamB = strafe amplitude, BehaviourParamC = strafe speed.
    /// </summary>
    public class DroneAI : EnemyBase
    {
        [SerializeField] private Transform muzzlePoint;
        private float _hoverHeight;
        private float _strafeAmplitude;
        private float _strafeSpeed;
        private float _originX;
        private float _attackTimer;

        protected override void Awake()
        {
            base.Awake();
            _hoverHeight = enemyData.BehaviourParamA > 0 ? enemyData.BehaviourParamA : 6f;
            _strafeAmplitude = enemyData.BehaviourParamB > 0 ? enemyData.BehaviourParamB : 4f;
            _strafeSpeed = enemyData.BehaviourParamC > 0 ? enemyData.BehaviourParamC : 1.5f;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _originX = transform.position.x;
        }

        private void Update()
        {
            if (IsDead) return;

            if (PlayerTransform == null || DistanceToPlayer() > enemyData.DetectionRange) return;

            var pos = transform.position;
            pos.x = _originX + Mathf.Sin(Time.time * _strafeSpeed) * _strafeAmplitude;
            pos.y = Mathf.Lerp(pos.y, PlayerTransform.position.y + _hoverHeight, Time.deltaTime * 2f);
            transform.position = pos;

            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0f && DistanceToPlayer() <= enemyData.AttackRange)
            {
                FireAtPlayer();
                _attackTimer = enemyData.AttackCooldown;
            }
        }

        private void FireAtPlayer()
        {
            if (enemyData.ProjectilePrefab == null || muzzlePoint == null) return;
            var direction = DirectionToPlayer();
            var instance = PoolManager.Instance.Spawn(enemyData.ProjectilePrefab, muzzlePoint.position, Quaternion.LookRotation(direction));
            var projectile = instance.GetComponent<Projectile>();
            projectile?.Launch(muzzlePoint.position, direction, enemyData.ProjectileSpeed, enemyData.AttackDamage, 0f, false, gameObject, enemyData.ProjectilePrefab);
        }
    }
}
