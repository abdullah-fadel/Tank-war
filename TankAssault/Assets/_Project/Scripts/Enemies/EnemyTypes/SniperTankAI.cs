using System.Collections;
using TankAssault.Core;
using TankAssault.Enemies.AIStates;
using TankAssault.Weapons;
using UnityEngine;

namespace TankAssault.Enemies.EnemyTypes
{
    /// <summary>
    /// Keeps maximum distance, telegraphs a charge-up laser sight before firing a high-damage
    /// long-range shot, and repositions away if the player closes the gap.
    /// BehaviourParamA = charge time seconds.
    /// </summary>
    public class SniperTankAI : EnemyBase
    {
        [SerializeField] private Transform muzzlePoint;
        [SerializeField] private GameObject chargeTelegraphVfx;
        private StateMachine _fsm;
        private bool _isCharging;

        protected override void Awake()
        {
            base.Awake();
            _fsm = new StateMachine();
            var idle = new IdleState();
            var reposition = new ChaseState(transform, this, enemyData.MoveSpeed, enemyData.AttackRange * 0.95f);
            var flee = new FleeState(transform, this, enemyData.MoveSpeed * 1.2f);
            var attack = new AttackState(BeginChargeAndFire, () => enemyData.AttackCooldown);

            _fsm.AddAnyTransition(idle, () => IsDead || PlayerTransform == null);
            _fsm.AddTransition(idle, reposition, () => !IsDead && DistanceToPlayer() <= enemyData.DetectionRange);
            _fsm.AddTransition(reposition, flee, () => DistanceToPlayer() < enemyData.AttackRange * 0.4f);
            _fsm.AddTransition(flee, reposition, () => DistanceToPlayer() >= enemyData.AttackRange * 0.6f);
            _fsm.AddTransition(reposition, attack, () => DistanceToPlayer() <= enemyData.AttackRange && !_isCharging);
            _fsm.AddTransition(attack, reposition, () => DistanceToPlayer() > enemyData.AttackRange && !_isCharging);
            _fsm.SetState(idle);
        }

        private void Update()
        {
            if (IsDead) return;
            _fsm.Tick(Time.deltaTime);
        }

        private void BeginChargeAndFire()
        {
            if (_isCharging) return;
            StartCoroutine(ChargeRoutine());
        }

        private IEnumerator ChargeRoutine()
        {
            _isCharging = true;
            float chargeTime = enemyData.BehaviourParamA > 0f ? enemyData.BehaviourParamA : 1f;

            if (chargeTelegraphVfx != null)
                chargeTelegraphVfx.SetActive(true);

            yield return new WaitForSeconds(chargeTime);

            if (chargeTelegraphVfx != null)
                chargeTelegraphVfx.SetActive(false);

            if (!IsDead && enemyData.ProjectilePrefab != null && muzzlePoint != null)
            {
                var direction = DirectionToPlayer();
                var instance = PoolManager.Instance.Spawn(enemyData.ProjectilePrefab, muzzlePoint.position, Quaternion.LookRotation(direction));
                var projectile = instance.GetComponent<Projectile>();
                projectile?.Launch(muzzlePoint.position, direction, enemyData.ProjectileSpeed, enemyData.AttackDamage, 0f, true, gameObject, enemyData.ProjectilePrefab);
            }

            _isCharging = false;
        }
    }
}
