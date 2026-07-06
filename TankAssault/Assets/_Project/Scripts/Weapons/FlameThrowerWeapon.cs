using TankAssault.Combat;
using UnityEngine;

namespace TankAssault.Weapons
{
    /// <summary>Continuous cone-of-effect weapon: damages every target inside a forward cone each tick while firing.</summary>
    public class FlameThrowerWeapon : WeaponBase
    {
        [SerializeField] private ParticleSystem flameParticles;

        private float _tickTimer;

        protected override void FireInternal(Vector3 aimDirection)
        {
            if (flameParticles != null && !flameParticles.isPlaying)
                flameParticles.Play();

            _tickTimer += Time.deltaTime;
            float tickInterval = 1f / weaponData.DamageTickRate;
            if (_tickTimer < tickInterval) return;
            _tickTimer = 0f;

            var colliders = Physics.OverlapSphere(muzzlePoint.position, weaponData.Range, targetMask);
            float halfAngle = weaponData.ConeAngleDegrees * 0.5f;

            foreach (var col in colliders)
            {
                Vector3 toTarget = (col.transform.position - muzzlePoint.position).normalized;
                if (Vector3.Angle(aimDirection, toTarget) > halfAngle) continue;

                var damageable = col.GetComponentInParent<IDamageable>();
                if (damageable != null && damageable.IsAlive)
                    damageable.TakeDamage(new DamageInfo(GetCurrentDamage(), col.transform.position, toTarget, false, false, Owner));
            }
        }

        public void StopFlame()
        {
            if (flameParticles != null && flameParticles.isPlaying)
                flameParticles.Stop();
        }
    }
}
