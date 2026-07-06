using System.Collections;
using TankAssault.Core;
using UnityEngine;

namespace TankAssault.CameraSystem
{
    /// <summary>Applies decaying positional noise to simulate impacts/explosions. Listens to ExplosionEvent.</summary>
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] private float shakeFrequency = 25f;
        [SerializeField] private AnimationCurve falloff = AnimationCurve.EaseInOut(0, 1, 1, 0);

        private Vector3 _shakeOffset;
        private Coroutine _shakeRoutine;

        private void OnEnable()
        {
            EventBus.Subscribe<ExplosionEvent>(OnExplosion);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<ExplosionEvent>(OnExplosion);
        }

        private void OnExplosion(ExplosionEvent evt)
        {
            Shake(evt.ShakeIntensity, 0.35f);
        }

        public void Shake(float intensity, float duration)
        {
            if (_shakeRoutine != null) StopCoroutine(_shakeRoutine);
            _shakeRoutine = StartCoroutine(ShakeRoutine(intensity, duration));
        }

        private IEnumerator ShakeRoutine(float intensity, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float strength = intensity * falloff.Evaluate(t);

                _shakeOffset = new Vector3(
                    (Mathf.PerlinNoise(Time.time * shakeFrequency, 0f) - 0.5f) * 2f,
                    (Mathf.PerlinNoise(0f, Time.time * shakeFrequency) - 0.5f) * 2f,
                    0f) * strength;

                yield return null;
            }
            _shakeOffset = Vector3.zero;
        }

        public Vector3 GetOffset() => _shakeOffset;
    }
}
