using TankAssault.Core;
using UnityEngine;

namespace TankAssault.Weapons
{
    /// <summary>Slow-firing high-damage shell with modest splash radius. The main-battery weapon.</summary>
    public class CannonWeapon : WeaponBase
    {
        protected override void FireInternal(Vector3 aimDirection)
        {
            var instance = PoolManager.Instance.Spawn(weaponData.ProjectilePrefab, muzzlePoint.position, muzzlePoint.rotation);
            var projectile = instance.GetComponent<Projectile>();
            projectile.Launch(muzzlePoint.position, aimDirection, weaponData.ProjectileSpeed, GetCurrentDamage(), weaponData.SplashRadius, false, Owner, weaponData.ProjectilePrefab);
        }
    }
}
