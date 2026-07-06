using TankAssault.Input;
using TankAssault.Weapons;
using UnityEngine;

namespace TankAssault.Player
{
    /// <summary>Routes fire/missile/skill input to the equipped WeaponBase instances mounted on the turret.</summary>
    public class PlayerWeaponManager : MonoBehaviour
    {
        [SerializeField] private WeaponBase primaryWeapon;
        [SerializeField] private WeaponBase missileWeapon;
        [SerializeField] private TurretController turretController;
        [SerializeField] private TankAnimationController animationController;

        private void Awake()
        {
            if (primaryWeapon != null) primaryWeapon.IsPlayerOwned = true;
            if (missileWeapon != null) missileWeapon.IsPlayerOwned = true;
        }

        private void Update()
        {
            if (InputManager.Instance == null) return;

            Vector3 aimDirection = turretController != null ? turretController.AimDirection : Vector3.right;

            if (InputManager.Instance.IsFiring && primaryWeapon != null)
            {
                if (primaryWeapon.TryFire(aimDirection))
                    animationController?.PlayFire();
            }
            else if (primaryWeapon is FlameThrowerWeapon flame)
            {
                flame.StopFlame();
            }
            else if (primaryWeapon is LaserWeapon laser)
            {
                laser.StopBeam();
            }

            if (InputManager.Instance.FireMissileRequested && missileWeapon != null)
                missileWeapon.TryFire(aimDirection);
        }

        public void EquipPrimary(WeaponBase weapon)
        {
            primaryWeapon = weapon;
            if (weapon != null) weapon.IsPlayerOwned = true;
        }

        public void EquipMissile(WeaponBase weapon)
        {
            missileWeapon = weapon;
            if (weapon != null) weapon.IsPlayerOwned = true;
        }
    }
}
