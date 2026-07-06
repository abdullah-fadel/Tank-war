using TankAssault.Core;
using UnityEngine;

namespace TankAssault.Weapons
{
    /// <summary>Fires a slower but self-guided missile; typically bound to the dedicated Missile button.</summary>
    public class HomingMissileWeapon : WeaponBase
    {
        protected override void FireInternal(Vector3 aimDirection)
        {
            var instance = PoolManager.Instance.Spawn(weaponData.ProjectilePrefab, muzzlePoint.position, muzzlePoint.rotation);
            var projectile = instance.GetComponent<HomingProjectile>();
            projectile.Launch(muzzlePoint.position, aimDirection, weaponData.ProjectileSpeed, GetCurrentDamage(), weaponData.SplashRadius, false, Owner, weaponData.ProjectilePrefab);
        }
    }
}
