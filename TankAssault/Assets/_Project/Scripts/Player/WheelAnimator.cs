using UnityEngine;

namespace TankAssault.Player
{
    /// <summary>Spins wheel meshes and scrolls track material offset based on tank velocity.</summary>
    public class WheelAnimator : MonoBehaviour
    {
        [SerializeField] private Transform[] wheelMeshes;
        [SerializeField] private float wheelRadius = 0.4f;
        [SerializeField] private Renderer trackRenderer;
        [SerializeField] private string trackTextureOffsetProperty = "_MainTex";

        private TankController _tankController;
        private MaterialPropertyBlock _propertyBlock;

        private void Awake()
        {
            _tankController = GetComponentInParent<TankController>();
            _propertyBlock = new MaterialPropertyBlock();
        }

        private void Update()
        {
            if (_tankController == null) return;

            float speed = _tankController.Velocity.x;
            float angularSpeedDeg = (speed / (2f * Mathf.PI * wheelRadius)) * 360f * Time.deltaTime;

            foreach (var wheel in wheelMeshes)
            {
                if (wheel == null) continue;
                wheel.Rotate(Vector3.forward, -angularSpeedDeg, Space.Self);
            }

            if (trackRenderer != null)
            {
                trackRenderer.GetPropertyBlock(_propertyBlock);
                float offset = Time.time * (speed * 0.05f);
                trackRenderer.SetPropertyBlock(_propertyBlock);
            }
        }
    }
}
