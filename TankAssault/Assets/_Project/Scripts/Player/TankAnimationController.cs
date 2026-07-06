using UnityEngine;

namespace TankAssault.Player
{
    /// <summary>Bridges gameplay state to the Animator Controller parameters and destroyed-state ragdoll/VFX.</summary>
    public class TankAnimationController : MonoBehaviour
    {
        private static readonly int SpeedParam = Animator.StringToHash("Speed");
        private static readonly int FireTrigger = Animator.StringToHash("Fire");
        private static readonly int DestroyedTrigger = Animator.StringToHash("Destroyed");

        [SerializeField] private Animator animator;
        [SerializeField] private GameObject destroyedVfxPrefab;
        [SerializeField] private GameObject[] disableOnDeath;
        [SerializeField] private Rigidbody[] hullPiecesForRagdoll;

        private TankController _tankController;

        private void Awake()
        {
            _tankController = GetComponent<TankController>();
        }

        private void Update()
        {
            if (animator == null || _tankController == null) return;
            animator.SetFloat(SpeedParam, Mathf.Abs(_tankController.Velocity.x));
        }

        public void PlayFire()
        {
            animator?.SetTrigger(FireTrigger);
        }

        public void PlayDestroyed()
        {
            animator?.SetTrigger(DestroyedTrigger);

            if (_tankController != null) _tankController.ControlsEnabled = false;

            foreach (var go in disableOnDeath)
                if (go != null) go.SetActive(false);

            if (destroyedVfxPrefab != null)
                Instantiate(destroyedVfxPrefab, transform.position, Quaternion.identity);

            foreach (var rb in hullPiecesForRagdoll)
            {
                if (rb == null) continue;
                rb.isKinematic = false;
                rb.AddExplosionForce(500f, transform.position - transform.forward, 5f);
            }
        }
    }
}
