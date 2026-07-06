using UnityEngine;

namespace TankAssault.Weapons
{
    /// <summary>Fast, low-damage hitscan weapon with high fire rate and small spread.</summary>
    public class MachineGunWeapon : WeaponBase
    {
        [SerializeField] private float spreadDegrees = 2f;

        protected override void FireInternal(Vector3 aimDirection)
        {
            Vector3 spreadDirection = Quaternion.Euler(0f, 0f, Random.Range(-spreadDegrees, spreadDegrees)) * aimDirection;
            HitscanWeapon.FireRay(muzzlePoint, spreadDirection, weaponData.Range, GetCurrentDamage(), targetMask, Owner, weaponData.ImpactVfxPrefab, out _);
        }
    }
}
