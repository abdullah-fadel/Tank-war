using TankAssault.Core;
using UnityEngine;

namespace TankAssault.VFX
{
    /// <summary>Short-lived pooled muzzle flash (particle burst + optional flash sprite billboard).</summary>
    public class MuzzleFlashEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem flashParticles;
        [SerializeField] private float lifetime = 0.2f;
        [SerializeField] private GameObject sourcePrefab;

        private void OnEnable()
        {
            flashParticles?.Play();
            Invoke(nameof(Despawn), lifetime);
        }

        private void Despawn()
        {
            if (sourcePrefab != null && PoolManager.Instance != null)
                PoolManager.Instance.Despawn(sourcePrefab, gameObject);
            else
                gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            CancelInvoke();
        }
    }
}
