using TankAssault.Core;
using UnityEngine;

namespace TankAssault.VFX
{
    /// <summary>Pooled surface-impact VFX (sparks/dust/debris) spawned oriented to the hit normal.</summary>
    public class ImpactEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] impactParticles;
        [SerializeField] private float lifetime = 1.5f;
        [SerializeField] private GameObject sourcePrefab;

        private void OnEnable()
        {
            foreach (var ps in impactParticles)
                ps.Play();

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
