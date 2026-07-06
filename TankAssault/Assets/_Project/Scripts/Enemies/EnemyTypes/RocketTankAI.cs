using TankAssault.Core;
using TankAssault.Enemies.AIStates;
using TankAssault.Weapons;
using UnityEngine;

namespace TankAssault.Enemies.EnemyTypes
{
    /// <summary>Mid-range bombardier that maintains distance and lobs splash-damage rockets in salvos.
    /// BehaviourParamA = number of rockets per salvo, BehaviourParamB = delay between salvo shots.</summary>
    public class RocketTankAI : EnemyBase
    {
        [SerializeField] private Transform muzzlePoint;
        private StateMachine _fsm;
        private bool _isFiring;

        protected override void Awake()
        {
            base.Awake();
            _fsm = new StateMachine();
            var idle = new IdleState();
            var reposition = new ChaseState(transform, this, enemyData.MoveSpeed, enemyData.AttackRange * 0.7f);
            var attack = new AttackState(FireSalvo, () => enemyData.AttackCooldown);

            _fsm.AddAnyTransition(idle, () => IsDead || PlayerTransform == null);
            _fsm.AddTransition(idle, reposition, () => !IsDead && DistanceToPlayer() <= enemyData.DetectionRange);
            _fsm.AddTransition(reposition, attack, () => DistanceToPlayer() <= enemyData.AttackRange && !_isFiring);
            _fsm.AddTransition(attack, reposition, () => DistanceToPlayer() > enemyData.AttackRange && !_isFiring);
            _fsm.SetState(idle);
        }

        private void Update()
        {
            if (IsDead) return;
            _fsm.Tick(Time.deltaTime);
        }

        private void FireSalvo()
        {
            if (_isFiring) return;
            StartCoroutine(SalvoRoutine());
        }

        private System.Collections.IEnumerator SalvoRoutine()
        {
            _isFiring = true;
            int count = Mathf.Max(1, Mathf.RoundToInt(enemyData.BehaviourParamA > 0 ? enemyData.BehaviourParamA : 3));
            float delay = enemyData.BehaviourParamB > 0 ? enemyData.BehaviourParamB : 0.25f;

            for (int i = 0; i < count && !IsDead; i++)
            {
                if (enemyData.ProjectilePrefab != null && muzzlePoint != null)
                {
                    var direction = DirectionToPlayer();
                    var instance = PoolManager.Instance.Spawn(enemyData.ProjectilePrefab, muzzlePoint.position, Quaternion.LookRotation(direction));
                    var projectile = instance.GetComponent<Projectile>();
                    projectile?.Launch(muzzlePoint.position, direction, enemyData.ProjectileSpeed, enemyData.AttackDamage, enemyData.BehaviourParamC > 0 ? enemyData.BehaviourParamC : 3f, false, gameObject, enemyData.ProjectilePrefab);
                }
                yield return new WaitForSeconds(delay);
            }

            _isFiring = false;
        }
    }
}
