using UnityEngine;

namespace TankAssault.Weapons
{
    /// <summary>Homing missile variant of Projectile: steers toward the nearest valid target after a brief activation delay.</summary>
    public class HomingProjectile : Projectile
    {
        [SerializeField] private float turnRateDegPerSec = 180f;
        [SerializeField] private float activationDelay = 0.15f;
        [SerializeField] private float acquisitionRadius = 30f;

        private Transform _target;
        private float _speed;

        public override void Launch(Vector3 position, Vector3 direction, float speed, float damage, float splashRadius, bool isCritical, GameObject owner, GameObject sourcePrefab)
        {
            base.Launch(position, direction, speed, damage, splashRadius, isCritical, owner, sourcePrefab);
            _speed = speed;
            _target = AcquireTarget();
        }

        private Transform AcquireTarget()
        {
            var colliders = Physics.OverlapSphere(transform.position, acquisitionRadius, hitMask);
            Transform closest = null;
            float closestDist = float.MaxValue;
            foreach (var col in colliders)
            {
                if (col.gameObject == Owner) continue;
                float dist = Vector3.Distance(transform.position, col.transform.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = col.transform;
                }
            }
            return closest;
        }

        protected override void Update()
        {
            base.Update();

            if (LifeTimer < activationDelay || _target == null) return;

            Vector3 desiredDirection = (_target.position - transform.position).normalized;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, desiredDirection, turnRateDegPerSec * Mathf.Deg2Rad * Time.deltaTime, 0f);
            transform.rotation = Quaternion.LookRotation(newDirection, Vector3.up);
            Rb.velocity = newDirection * _speed;
        }
    }
}
