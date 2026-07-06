using TankAssault.Core;
using TankAssault.Data;
using UnityEngine;

namespace TankAssault.Weapons
{
    /// <summary>
    /// Base class for all seven weapon types. Owns cooldown/ammo/reload bookkeeping and
    /// delegates the actual damage-delivery mechanism (projectile/hitscan/continuous) to
    /// subclasses via FireInternal, keeping WeaponData purely data-driven.
    /// </summary>
    public abstract class WeaponBase : MonoBehaviour
    {
        [SerializeField] protected WeaponData weaponData;
        [SerializeField] protected Transform muzzlePoint;
        [SerializeField] protected LayerMask targetMask;

        protected int CurrentLevel = 1;
        protected int AmmoInMagazine;
        protected float CooldownTimer;
        protected float ReloadTimer;
        protected bool IsReloading;
        protected GameObject Owner;

        public WeaponData Data => weaponData;
        public bool IsPlayerOwned { get; set; }

        protected virtual void Awake()
        {
            Owner = transform.root.gameObject;
            AmmoInMagazine = weaponData != null ? weaponData.MagazineSize : 1;
        }

        protected virtual void Update()
        {
            if (CooldownTimer > 0f) CooldownTimer -= Time.deltaTime;

            if (IsReloading)
            {
                ReloadTimer -= Time.deltaTime;
                if (ReloadTimer <= 0f)
                {
                    IsReloading = false;
                    AmmoInMagazine = weaponData.MagazineSize;
                }
            }
        }

        public void SetLevel(int level) => CurrentLevel = Mathf.Max(1, level);

        public bool TryFire(Vector3 aimDirection)
        {
            if (weaponData == null || IsReloading || CooldownTimer > 0f) return false;
            if (weaponData.MagazineSize > 0 && AmmoInMagazine <= 0)
            {
                StartReload();
                return false;
            }

            var upgrade = weaponData.GetUpgrade(CurrentLevel);
            float fireRateMultiplier = upgrade != null ? upgrade.FireRateMultiplier : 1f;
            CooldownTimer = 1f / (weaponData.FireRate * fireRateMultiplier);

            if (weaponData.MagazineSize > 0)
            {
                AmmoInMagazine--;
                if (AmmoInMagazine <= 0) StartReload();
            }

            FireInternal(aimDirection);

            if (muzzlePoint != null && weaponData.MuzzleFlashPrefab != null)
                PoolManager.Instance.Spawn(weaponData.MuzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);

            EventBus.Publish(new WeaponFiredEvent(weaponData.WeaponId, muzzlePoint != null ? muzzlePoint.position : transform.position));

            return true;
        }

        private void StartReload()
        {
            var upgrade = weaponData.GetUpgrade(CurrentLevel);
            float reloadMultiplier = upgrade != null ? upgrade.ReloadMultiplier : 1f;
            IsReloading = true;
            ReloadTimer = weaponData.ReloadTime * reloadMultiplier;
        }

        protected float GetCurrentDamage() => weaponData.GetDamageForLevel(CurrentLevel);

        protected abstract void FireInternal(Vector3 aimDirection);
    }
}
