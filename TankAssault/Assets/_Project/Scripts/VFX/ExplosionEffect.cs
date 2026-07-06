using System.Collections;
using TankAssault.Core;
using UnityEngine;

namespace TankAssault.VFX
{
    /// <summary>Pooled explosion VFX: particle burst + light flash that auto-returns to the pool.</summary>
    public class ExplosionEffect : MonoBehaviour
    {
        [SerializeField] private ParticleSystem[] particleSystems;
        [SerializeField] private Light flashLight;
        [SerializeField] private float flashDuration = 0.15f;
        [SerializeField] private float lifetime = 2f;
        [SerializeField] private GameObject sourcePrefab;

        private void OnEnable()
        {
            foreach (var ps in particleSystems)
                ps.Play();

            if (flashLight != null)
                StartCoroutine(FlashRoutine());

            Invoke(nameof(Despawn), lifetime);
        }

        private IEnumerator FlashRoutine()
        {
            flashLight.enabled = true;
            float startIntensity = flashLight.intensity;
            float elapsed = 0f;
            while (elapsed < flashDuration)
            {
                elapsed += Time.deltaTime;
                flashLight.intensity = Mathf.Lerp(startIntensity, 0f, elapsed / flashDuration);
                yield return null;
            }
            flashLight.enabled = false;
            flashLight.intensity = startIntensity;
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
