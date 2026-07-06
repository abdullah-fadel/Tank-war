using TankAssault.Core;
using TankAssault.Enemies.AIStates;
using TankAssault.Weapons;
using UnityEngine;

namespace TankAssault.Enemies.EnemyTypes
{
    /// <summary>Cheap, fast, low-health filler enemy: chases into cannon range and fires single shots.</summary>
    public class LightTankAI : EnemyBase
    {
        [SerializeField] private Transform muzzlePoint;
        private StateMachine _fsm;
        private ChaseState _chase;
        private AttackState _attack;

        protected override void Awake()
        {
            base.Awake();
            _fsm = new StateMachine();
            var idle = new IdleState();
            _chase = new ChaseState(transform, this, enemyData.MoveSpeed, enemyData.AttackRange * 0.8f);
            _attack = new AttackState(FireShot, () => enemyData.AttackCooldown);

            _fsm.AddAnyTransition(idle, () => IsDead || PlayerTransform == null);
            _fsm.AddTransition(idle, _chase, () => !IsDead && DistanceToPlayer() <= enemyData.DetectionRange);
            _fsm.AddTransition(_chase, _attack, () => DistanceToPlayer() <= enemyData.AttackRange);
            _fsm.AddTransition(_attack, _chase, () => DistanceToPlayer() > enemyData.AttackRange);
            _fsm.SetState(idle);
        }

        private void Update()
        {
            if (IsDead) return;
            _fsm.Tick(Time.deltaTime);
        }

        private void FireShot()
        {
            if (enemyData.ProjectilePrefab == null || muzzlePoint == null) return;
            var direction = DirectionToPlayer();
            var instance = PoolManager.Instance.Spawn(enemyData.ProjectilePrefab, muzzlePoint.position, Quaternion.LookRotation(direction));
            var projectile = instance.GetComponent<Projectile>();
            projectile?.Launch(muzzlePoint.position, direction, enemyData.ProjectileSpeed, enemyData.AttackDamage, 0f, false, gameObject, enemyData.ProjectilePrefab);
        }
    }
}
