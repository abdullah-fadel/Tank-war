using TankAssault.Core;
using UnityEngine;

namespace TankAssault.Weapons
{
    /// <summary>High-velocity energy projectile with moderate splash and a distinct plasma trail VFX.</summary>
    public class PlasmaWeapon : WeaponBase
    {
        protected override void FireInternal(Vector3 aimDirection)
        {
            var instance = PoolManager.Instance.Spawn(weaponData.ProjectilePrefab, muzzlePoint.position, muzzlePoint.rotation);
            var projectile = instance.GetComponent<Projectile>();
            projectile.Launch(muzzlePoint.position, aimDirection, weaponData.ProjectileSpeed, GetCurrentDamage(), weaponData.SplashRadius, false, Owner, weaponData.ProjectilePrefab);
        }
    }
}
