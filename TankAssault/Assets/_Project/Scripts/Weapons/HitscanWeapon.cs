using TankAssault.Combat;
using TankAssault.Core;
using UnityEngine;

namespace TankAssault.Weapons
{
    /// <summary>Shared instant-hit raycast logic reused by MachineGun and Laser weapons.</summary>
    public static class HitscanWeapon
    {
        public static void FireRay(Transform muzzle, Vector3 direction, float range, float damage, LayerMask mask, GameObject owner, GameObject impactVfxPrefab, out Vector3 hitPoint)
        {
            hitPoint = muzzle.position + direction * range;

            if (Physics.Raycast(muzzle.position, direction, out var hit, range, mask))
            {
                hitPoint = hit.point;
                var damageable = hit.collider.GetComponentInParent<IDamageable>();
                if (damageable != null && damageable.IsAlive)
                    damageable.TakeDamage(new DamageInfo(damage, hit.point, direction, false, false, owner));

                if (impactVfxPrefab != null)
                    PoolManager.Instance.Spawn(impactVfxPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
    }
}
