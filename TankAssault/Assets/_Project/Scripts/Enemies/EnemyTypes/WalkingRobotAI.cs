using TankAssault.Combat;
using TankAssault.Core;
using TankAssault.Enemies.AIStates;
using UnityEngine;

namespace TankAssault.Enemies.EnemyTypes
{
    /// <summary>
    /// Ground melee unit: closes distance quickly, lunges into contact range, then delivers a burst
    /// melee strike before backing off briefly. BehaviourParamA = lunge speed multiplier.
    /// </summary>
    public class WalkingRobotAI : EnemyBase
    {
        private StateMachine _fsm;
        private bool _isAttacking;

        protected override void Awake()
        {
            base.Awake();
            _fsm = new StateMachine();
            var idle = new IdleState();
            var chase = new ChaseState(transform, this, enemyData.MoveSpeed, 0f);
            var attack = new AttackState(MeleeStrike, () => enemyData.AttackCooldown);

            _fsm.AddAnyTransition(idle, () => IsDead || PlayerTransform == null);
            _fsm.AddTransition(idle, chase, () => !IsDead && DistanceToPlayer() <= enemyData.DetectionRange);
            _fsm.AddTransition(chase, attack, () => DistanceToPlayer() <= enemyData.AttackRange && !_isAttacking);
            _fsm.AddTransition(attack, chase, () => DistanceToPlayer() > enemyData.AttackRange);
            _fsm.SetState(idle);
        }

        private void Update()
        {
            if (IsDead) return;
            _fsm.Tick(Time.deltaTime);
        }

        private void MeleeStrike()
        {
            if (PlayerTransform == null) return;
            var playerDamageable = PlayerTransform.GetComponentInParent<IDamageable>();
            if (playerDamageable != null && playerDamageable.IsAlive && DistanceToPlayer() <= enemyData.AttackRange)
            {
                playerDamageable.TakeDamage(new DamageInfo(enemyData.AttackDamage, PlayerTransform.position, DirectionToPlayer(), false, false, gameObject));

                if (enemyData.AttackSfx != null)
                    AudioSource.PlayClipAtPoint(enemyData.AttackSfx, transform.position);
            }
        }
    }
}
