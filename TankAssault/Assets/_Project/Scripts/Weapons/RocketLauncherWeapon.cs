using TankAssault.Core;
using UnityEngine;

namespace TankAssault.Weapons
{
    /// <summary>Large-splash, limited-magazine explosive weapon with a long reload.</summary>
    public class RocketLauncherWeapon : WeaponBase
    {
        protected override void FireInternal(Vector3 aimDirection)
        {
            var instance = PoolManager.Instance.Spawn(weaponData.ProjectilePrefab, muzzlePoint.position, muzzlePoint.rotation);
            var projectile = instance.GetComponent<Projectile>();
            projectile.Launch(muzzlePoint.position, aimDirection, weaponData.ProjectileSpeed, GetCurrentDamage(), weaponData.SplashRadius, false, Owner, weaponData.ProjectilePrefab);
        }
    }
}
