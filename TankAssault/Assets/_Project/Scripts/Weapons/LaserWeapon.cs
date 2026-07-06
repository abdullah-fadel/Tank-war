using UnityEngine;

namespace TankAssault.Weapons
{
    /// <summary>Continuous instant-hit beam that renders a LineRenderer while the fire button is held.</summary>
    public class LaserWeapon : WeaponBase
    {
        [SerializeField] private LineRenderer beamRenderer;
        [SerializeField] private float beamDamageTickRate = 10f;

        private float _tickTimer;

        protected override void Update()
        {
            base.Update();
            if (beamRenderer != null && beamRenderer.enabled)
                beamRenderer.SetPosition(0, muzzlePoint.position);
        }

        protected override void FireInternal(Vector3 aimDirection)
        {
            _tickTimer += Time.deltaTime;
            float tickInterval = 1f / beamDamageTickRate;
            if (_tickTimer < tickInterval) return;
            _tickTimer = 0f;

            HitscanWeapon.FireRay(muzzlePoint, aimDirection, weaponData.Range, GetCurrentDamage(), targetMask, Owner, weaponData.ImpactVfxPrefab, out var hitPoint);

            if (beamRenderer != null)
            {
                beamRenderer.enabled = true;
                beamRenderer.SetPosition(0, muzzlePoint.position);
                beamRenderer.SetPosition(1, hitPoint);
            }
        }

        public void StopBeam()
        {
            if (beamRenderer != null) beamRenderer.enabled = false;
        }
    }
}
