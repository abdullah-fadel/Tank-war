using System.Collections;
using TankAssault.CameraSystem;
using TankAssault.Core;
using TankAssault.Weapons;
using UnityEngine;

namespace TankAssault.Enemies.EnemyTypes
{
    /// <summary>
    /// Multi-phase world-ending boss. Phase 1 (100%-60% HP): slow cannon volleys while advancing.
    /// Phase 2 (60%-25% HP): adds rocket salvos and faster movement.
    /// Phase 3 (below 25%, "enrage"): rapid-fire cannon + rockets, ignores stand-off distance.
    /// Triggers the camera's cinematic boss zoom on spawn and releases it on death.
    /// </summary>
    public class BossTankAI : EnemyBase
    {
        private enum Phase { One, Two, Three }

        [SerializeField] private Transform cannonMuzzle;
        [SerializeField] private Transform rocketMuzzle;
        [SerializeField] private GameObject cannonProjectilePrefab;
        [SerializeField] private GameObject rocketProjectilePrefab;

        private Phase _currentPhase = Phase.One;
        private float _attackTimer;
        private CameraRig _cameraRig;

        protected override void OnEnable()
        {
            base.OnEnable();
            _currentPhase = Phase.One;
            _cameraRig = FindObjectOfType<CameraRig>();
            _cameraRig?.EnterBossZoom();
        }

        private void Update()
        {
            if (IsDead || PlayerTransform == null) return;

            UpdatePhase();
            MoveTowardPlayer();

            _attackTimer -= Time.deltaTime;
            if (_attackTimer <= 0f)
            {
                StartCoroutine(ExecuteAttackPattern());
            }
        }

        private void UpdatePhase()
        {
            float healthPercent = Health.CurrentHealth / Health.MaxHealth;
            if (healthPercent <= 0.25f) _currentPhase = Phase.Three;
            else if (healthPercent <= 0.6f) _currentPhase = Phase.Two;
            else _currentPhase = Phase.One;
        }

        private void MoveTowardPlayer()
        {
            float standOff = _currentPhase == Phase.Three ? enemyData.AttackRange * 0.3f : enemyData.AttackRange * 0.7f;
            if (DistanceToPlayer() <= standOff) return;

            float speedMultiplier = _currentPhase == Phase.One ? 1f : _currentPhase == Phase.Two ? 1.3f : 1.6f;
            float direction = Mathf.Sign(PlayerTransform.position.x - transform.position.x);
            var pos = transform.position;
            pos.x += direction * enemyData.MoveSpeed * speedMultiplier * Time.deltaTime;
            transform.position = pos;
        }

        private IEnumerator ExecuteAttackPattern()
        {
            switch (_currentPhase)
            {
                case Phase.One:
                    FireCannon();
                    _attackTimer = enemyData.AttackCooldown;
                    break;

                case Phase.Two:
                    FireCannon();
                    yield return new WaitForSeconds(0.4f);
                    if (!IsDead) FireRocket();
                    _attackTimer = enemyData.AttackCooldown * 0.8f;
                    break;

                case Phase.Three:
                    for (int i = 0; i < 3 && !IsDead; i++)
                    {
                        FireCannon();
                        yield return new WaitForSeconds(0.2f);
                    }
                    FireRocket();
                    _attackTimer = enemyData.AttackCooldown * 0.5f;
                    break;
            }
        }

        private void FireCannon()
        {
            if (cannonProjectilePrefab == null || cannonMuzzle == null) return;
            var direction = DirectionToPlayer();
            var instance = PoolManager.Instance.Spawn(cannonProjectilePrefab, cannonMuzzle.position, Quaternion.LookRotation(direction));
            var projectile = instance.GetComponent<Projectile>();
            projectile?.Launch(cannonMuzzle.position, direction, enemyData.ProjectileSpeed, enemyData.AttackDamage, 2f, false, gameObject, cannonProjectilePrefab);
        }

        private void FireRocket()
        {
            if (rocketProjectilePrefab == null || rocketMuzzle == null) return;
            var direction = DirectionToPlayer();
            var instance = PoolManager.Instance.Spawn(rocketProjectilePrefab, rocketMuzzle.position, Quaternion.LookRotation(direction));
            var projectile = instance.GetComponent<Projectile>();
            projectile?.Launch(rocketMuzzle.position, direction, enemyData.ProjectileSpeed * 0.7f, enemyData.AttackDamage * 1.5f, 4f, false, gameObject, rocketProjectilePrefab);
        }

        public override void OnDeath()
        {
            _cameraRig?.ExitBossZoom();
            _cameraRig?.Shake(2.5f, 1f);
            base.OnDeath();
        }
    }
}
