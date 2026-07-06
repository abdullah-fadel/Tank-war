using TankAssault.Core;
using TankAssault.Data;
using TankAssault.Input;
using UnityEngine;

namespace TankAssault.Player
{
    /// <summary>
    /// Drives player movement on the side-scrolling X axis while following terrain height
    /// on Y via raycast (hills/bridges/platforms). Rotation/roll is purely cosmetic and does
    /// not affect gameplay logic, keeping collision simple for a 2.5D game.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class TankController : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private TankData tankData;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundRayHeight = 2f;
        [SerializeField] private float groundRayDistance = 5f;
        [SerializeField] private float bodyTiltDegrees = 8f;
        [SerializeField] private float bodyTiltSmoothing = 6f;

        [Header("Runtime (read-only)")]
        [SerializeField] private float currentSpeedMultiplier = 1f;

        private Rigidbody _rigidbody;
        private float _horizontalInput;
        private float _currentTiltVelocity;
        public bool ControlsEnabled { get; set; } = true;

        public float MoveSpeed { get; set; }
        public Vector3 Velocity => _rigidbody.velocity;
        public bool IsMoving => Mathf.Abs(_horizontalInput) > 0.05f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
            MoveSpeed = tankData != null ? tankData.BaseSpeed : 6f;
        }

        private void Update()
        {
            if (!ControlsEnabled)
            {
                _horizontalInput = 0f;
                return;
            }

            _horizontalInput = InputManager.Instance != null ? InputManager.Instance.MoveAxis : 0f;
            SnapToGround();
            ApplyBodyTilt();
        }

        private void FixedUpdate()
        {
            if (!ControlsEnabled) return;

            float targetVelocityX = _horizontalInput * MoveSpeed * currentSpeedMultiplier;
            var velocity = _rigidbody.velocity;
            velocity.x = targetVelocityX;
            _rigidbody.velocity = velocity;
        }

        private void SnapToGround()
        {
            Vector3 origin = transform.position + Vector3.up * groundRayHeight;
            if (Physics.Raycast(origin, Vector3.down, out var hit, groundRayHeight + groundRayDistance, groundMask))
            {
                var pos = transform.position;
                pos.y = Mathf.Lerp(pos.y, hit.point.y, Time.deltaTime * 10f);
                transform.position = pos;
            }
        }

        private void ApplyBodyTilt()
        {
            float targetTilt = -_horizontalInput * bodyTiltDegrees;
            float currentTilt = Mathf.SmoothDampAngle(transform.localEulerAngles.z, targetTilt, ref _currentTiltVelocity, 1f / bodyTiltSmoothing);
            var euler = transform.localEulerAngles;
            euler.z = currentTilt;
            transform.localEulerAngles = euler;
        }

        public void SetSpeedMultiplier(float multiplier) => currentSpeedMultiplier = multiplier;

        public void ApplyKnockback(Vector3 direction, float force)
        {
            _rigidbody.AddForce(new Vector3(direction.x, 0f, 0f) * force, ForceMode.Impulse);
        }
    }
}
