using TankAssault.Input;
using UnityEngine;

namespace TankAssault.Player
{
    /// <summary>Rotates the turret pivot independently of the hull to aim at the current aim direction/target.</summary>
    public class TurretController : MonoBehaviour
    {
        [SerializeField] private Transform turretPivot;
        [SerializeField] private Transform cannonPivot;
        [SerializeField] private float turretRotationSpeed = 120f;
        [SerializeField] private float cannonMinPitch = -5f;
        [SerializeField] private float cannonMaxPitch = 35f;

        private Transform _autoAimTarget;
        public Vector3 AimDirection { get; private set; } = Vector3.right;

        private void Update()
        {
            UpdateAimDirection();
            RotateTurret();
        }

        private void UpdateAimDirection()
        {
            if (_autoAimTarget != null)
            {
                AimDirection = (_autoAimTarget.position - turretPivot.position).normalized;
                return;
            }

            if (InputManager.Instance != null && InputManager.Instance.IsAiming)
            {
                AimDirection = InputManager.Instance.AimDirection;
            }
        }

        private void RotateTurret()
        {
            if (turretPivot == null) return;

            // Yaw the turret to face left/right on the side-view plane.
            float yaw = AimDirection.x >= 0f ? 90f : -90f;
            var targetHullRotation = Quaternion.Euler(0f, yaw, 0f);
            turretPivot.localRotation = Quaternion.RotateTowards(turretPivot.localRotation, targetHullRotation, turretRotationSpeed * Time.deltaTime);

            if (cannonPivot == null) return;

            float pitch = Mathf.Clamp(-Mathf.Asin(Mathf.Clamp(AimDirection.y, -1f, 1f)) * Mathf.Rad2Deg, cannonMinPitch, cannonMaxPitch);
            var targetCannonRotation = Quaternion.Euler(pitch, 0f, 0f);
            cannonPivot.localRotation = Quaternion.RotateTowards(cannonPivot.localRotation, targetCannonRotation, turretRotationSpeed * Time.deltaTime);
        }

        public void SetAutoAimTarget(Transform target) => _autoAimTarget = target;

        public Vector3 GetMuzzleWorldDirection() => AimDirection;
    }
}
