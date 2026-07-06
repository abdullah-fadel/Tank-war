using UnityEngine;

namespace TankAssault.CameraSystem
{
    /// <summary>
    /// Fixed side-view camera that smoothly follows the player horizontally, with
    /// look-ahead in the direction of movement and a cinematic zoom-in for boss fights.
    /// </summary>
    [RequireComponent(typeof(CameraShake))]
    public class CameraRig : MonoBehaviour
    {
        [Header("Follow")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 baseOffset = new Vector3(0f, 4f, -12f);
        [SerializeField] private float followSmoothing = 5f;
        [SerializeField] private float lookAheadDistance = 3f;
        [SerializeField] private float lookAheadSmoothing = 3f;

        [Header("Zoom")]
        [SerializeField] private float defaultFov = 50f;
        [SerializeField] private float bossFov = 38f;
        [SerializeField] private float fovTransitionSpeed = 2f;

        private UnityEngine.Camera _camera;
        private CameraShake _shake;
        private float _currentLookAhead;
        private float _targetFov;
        private float _lastTargetX;

        private void Awake()
        {
            _camera = GetComponent<UnityEngine.Camera>();
            _shake = GetComponent<CameraShake>();
            _targetFov = defaultFov;
            if (_camera != null) _camera.fieldOfView = defaultFov;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            float velocityDir = target.position.x - _lastTargetX;
            _lastTargetX = target.position.x;

            float lookAheadTarget = Mathf.Sign(velocityDir) * lookAheadDistance * Mathf.Clamp01(Mathf.Abs(velocityDir) * 50f);
            _currentLookAhead = Mathf.Lerp(_currentLookAhead, lookAheadTarget, Time.deltaTime * lookAheadSmoothing);

            Vector3 desiredPosition = target.position + baseOffset + new Vector3(_currentLookAhead, 0f, 0f);
            desiredPosition += _shake.GetOffset();

            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSmoothing);
            transform.LookAt(target.position + Vector3.up * 1.5f);

            if (_camera != null)
                _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, _targetFov, Time.deltaTime * fovTransitionSpeed);
        }

        public void SetTarget(Transform newTarget) => target = newTarget;

        public void EnterBossZoom() => _targetFov = bossFov;
        public void ExitBossZoom() => _targetFov = defaultFov;

        public void Shake(float intensity, float duration) => _shake.Shake(intensity, duration);
    }
}
