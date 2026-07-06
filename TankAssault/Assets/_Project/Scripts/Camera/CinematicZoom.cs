using System.Collections;
using UnityEngine;

namespace TankAssault.CameraSystem
{
    /// <summary>Trigger volume that engages the boss cinematic zoom when the player enters a boss arena.</summary>
    [RequireComponent(typeof(Collider))]
    public class CinematicZoom : MonoBehaviour
    {
        [SerializeField] private CameraRig cameraRig;
        [SerializeField] private float introHoldSeconds = 1.5f;
        [SerializeField] private string playerTag = "Player";

        private void Reset()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(playerTag)) return;
            if (cameraRig == null) cameraRig = FindObjectOfType<CameraRig>();
            cameraRig?.EnterBossZoom();
            StartCoroutine(HoldThenReleaseIfNeeded());
        }

        private IEnumerator HoldThenReleaseIfNeeded()
        {
            yield return new WaitForSeconds(introHoldSeconds);
            // Zoom persists for the boss fight duration; BossTankAI calls ExitBossZoom on death.
        }
    }
}
