using UnityEngine;

namespace TankAssault.Player
{
    /// <summary>
    /// Purely visual per-wheel spring suspension: raycasts down from each wheel mount and
    /// lerps the wheel visual toward the hit point so the hull appears to react to terrain.
    /// </summary>
    public class SuspensionController : MonoBehaviour
    {
        [System.Serializable]
        public class WheelMount
        {
            public Transform MountPoint;
            public Transform WheelVisual;
            [HideInInspector] public float CurrentOffset;
        }

        [SerializeField] private WheelMount[] wheels;
        [SerializeField] private float restLength = 0.4f;
        [SerializeField] private float springTravel = 0.25f;
        [SerializeField] private float springSmoothing = 10f;
        [SerializeField] private LayerMask groundMask;

        private void Update()
        {
            if (wheels == null) return;

            foreach (var wheel in wheels)
            {
                if (wheel.MountPoint == null || wheel.WheelVisual == null) continue;

                float targetOffset = 0f;
                if (Physics.Raycast(wheel.MountPoint.position, Vector3.down, out var hit, restLength + springTravel, groundMask))
                {
                    float distance = hit.distance;
                    targetOffset = restLength - distance;
                    targetOffset = Mathf.Clamp(targetOffset, -springTravel, springTravel);
                }

                wheel.CurrentOffset = Mathf.Lerp(wheel.CurrentOffset, targetOffset, Time.deltaTime * springSmoothing);

                var localPos = wheel.WheelVisual.localPosition;
                localPos.y = wheel.CurrentOffset;
                wheel.WheelVisual.localPosition = localPos;
            }
        }
    }
}
