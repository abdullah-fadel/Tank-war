using TankAssault.Core;
using TankAssault.Enemies.AIStates;
using TankAssault.Weapons;
using UnityEngine;

namespace TankAssault.Enemies.EnemyTypes
{
    /// <summary>Slow, tanky, hard-hitting cannon that never flees and shrugs off small-arms fire via high armor.</summary>
    public class HeavyTankAI : EnemyBase
    {
        [SerializeField] private Transform muzzlePoint;
        private StateMachine _fsm;

        protected override void Awake()
        {
            base.Awake();
            _fsm = new StateMachine();
            var idle = new IdleState();
            var chase = new ChaseState(transform, this, enemyData.MoveSpeed, enemyData.AttackRange * 0.9f);
            var attack = new AttackState(FireHeavyShell, () => enemyData.AttackCooldown);

            _fsm.AddAnyTransition(idle, () => IsDead || PlayerTransform == null);
            _fsm.AddTransition(idle, chase, () => !IsDead && DistanceToPlayer() <= enemyData.DetectionRange);
            _fsm.AddTransition(chase, attack, () => DistanceToPlayer() <= enemyData.AttackRange);
            _fsm.AddTransition(attack, chase, () => DistanceToPlayer() > enemyData.AttackRange);
            _fsm.SetState(idle);
        }

        private void Update()
        {
            if (IsDead) return;
            _fsm.Tick(Time.deltaTime);
        }

        private void FireHeavyShell()
        {
            if (enemyData.ProjectilePrefab == null || muzzlePoint == null) return;
            var direction = DirectionToPlayer();
            var instance = PoolManager.Instance.Spawn(enemyData.ProjectilePrefab, muzzlePoint.position, Quaternion.LookRotation(direction));
            var projectile = instance.GetComponent<Projectile>();
            projectile?.Launch(muzzlePoint.position, direction, enemyData.ProjectileSpeed, enemyData.AttackDamage, enemyData.BehaviourParamA, false, gameObject, enemyData.ProjectilePrefab);
        }
    }
}
