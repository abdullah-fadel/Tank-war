using TankAssault.Core;
using UnityEngine;

namespace TankAssault.Input
{
    /// <summary>
    /// Central hub aggregating all touch controls into simple properties consumed by
    /// gameplay systems. Keeping this as one place makes it trivial to swap virtual
    /// controls for a gamepad/keyboard in the editor without touching gameplay code.
    /// </summary>
    public class InputManager : Singleton<InputManager>
    {
        [Header("References")]
        [SerializeField] private VirtualJoystick moveJoystick;
        [SerializeField] private TouchButton fireButton;
        [SerializeField] private TouchButton aimButton;
        [SerializeField] private TouchButton missileButton;
        [SerializeField] private TouchButton skillButton;
        [SerializeField] private TouchButton boostButton;

        [Header("Sensitivity")]
        [Range(0.1f, 3f)] public float MoveSensitivity = 1f;
        [Range(0.1f, 3f)] public float AimSensitivity = 1f;

        public float MoveAxis { get; private set; }
        public bool IsFiring { get; private set; }
        public bool IsAiming { get; private set; }
        public bool FireMissileRequested { get; private set; }
        public bool SkillRequested { get; private set; }
        public bool IsBoostHeld { get; private set; }
        public Vector3 AimDirection { get; private set; } = Vector3.right;

        protected override void Awake()
        {
            base.Awake();
            var saved = SaveSystem.Instance != null ? SaveSystem.Instance.Current : null;
            if (saved != null) MoveSensitivity = saved.JoystickSensitivity;
        }

        private void Update()
        {
            MoveAxis = moveJoystick != null ? Mathf.Clamp(moveJoystick.Horizontal * MoveSensitivity, -1f, 1f) : 0f;
            IsFiring = fireButton != null && fireButton.IsHeld;
            IsAiming = aimButton != null && aimButton.IsHeld;
            IsBoostHeld = boostButton != null && boostButton.IsHeld;

            if (IsAiming && aimButton != null)
            {
                // Aim button acts as a directional pad for cannon elevation, driven by drag delta in a full
                // implementation; approximated here as forward + slight upward aim while held.
                AimDirection = new Vector3(Mathf.Sign(MoveAxis == 0 ? 1 : MoveAxis), 0.15f * AimSensitivity, 0f).normalized;
            }

            FireMissileRequested = missileButton != null && missileButton.IsHeld;
            SkillRequested = skillButton != null && skillButton.IsHeld;
        }

        public void SetMoveSensitivity(float value)
        {
            MoveSensitivity = value;
            if (SaveSystem.Instance != null)
            {
                SaveSystem.Instance.Current.JoystickSensitivity = value;
                SaveSystem.Instance.Save();
            }
        }
    }
}
