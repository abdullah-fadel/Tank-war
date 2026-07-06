using System.Collections;
using TankAssault.Core;
using TankAssault.Weapons;
using UnityEngine;

namespace TankAssault.Enemies.EnemyTypes
{
    /// <summary>
    /// High-speed strafing run enemy: flies across the screen at a fixed altitude firing continuously,
    /// then loops back off-screen and repeats. Hard to hit due to sustained high velocity.
    /// BehaviourParamA = pass altitude, BehaviourParamB = pass speed, BehaviourParamC = pause between passes.
    /// </summary>
    public class JetAI : EnemyBase
    {
        [SerializeField] private Transform muzzlePoint;
        [SerializeField] private float passHalfWidth = 25f;
        private Coroutine _passRoutine;

        protected override void OnEnable()
        {
            base.OnEnable();
            _passRoutine = StartCoroutine(PassLoop());
        }

        private void OnDisable()
        {
            if (_passRoutine != null) StopCoroutine(_passRoutine);
        }

        private IEnumerator PassLoop()
        {
            float altitude = enemyData.BehaviourParamA > 0 ? enemyData.BehaviourParamA : 10f;
            float speed = enemyData.BehaviourParamB > 0 ? enemyData.BehaviourParamB : 12f;
            float pause = enemyData.BehaviourParamC > 0 ? enemyData.BehaviourParamC : 1.5f;
            int direction = 1;

            while (!IsDead)
            {
                float startX = PlayerTransform != null ? PlayerTransform.position.x - passHalfWidth * direction : -passHalfWidth * direction;
                float endX = PlayerTransform != null ? PlayerTransform.position.x + passHalfWidth * direction : passHalfWidth * direction;

                var pos = transform.position;
                pos.x = startX;
                pos.y = altitude;
                transform.position = pos;

                float attackTimer = 0f;
                while (Mathf.Abs(transform.position.x - endX) > 0.5f && !IsDead)
                {
                    var p = transform.position;
                    p.x = Mathf.MoveTowards(p.x, endX, speed * Time.deltaTime);
                    transform.position = p;

                    attackTimer -= Time.deltaTime;
                    if (attackTimer <= 0f && DistanceToPlayer() <= enemyData.AttackRange)
                    {
                        FireForward(direction);
                        attackTimer = enemyData.AttackCooldown;
                    }
                    yield return null;
                }

                direction *= -1;
                yield return new WaitForSeconds(pause);
            }
        }

        private void FireForward(int direction)
        {
            if (enemyData.ProjectilePrefab == null || muzzlePoint == null) return;
            var dir = new Vector3(-direction, 0f, 0f);
            var instance = PoolManager.Instance.Spawn(enemyData.ProjectilePrefab, muzzlePoint.position, Quaternion.LookRotation(dir));
            var projectile = instance.GetComponent<Projectile>();
            projectile?.Launch(muzzlePoint.position, dir, enemyData.ProjectileSpeed, enemyData.AttackDamage, 0f, false, gameObject, enemyData.ProjectilePrefab);
        }
    }
}
