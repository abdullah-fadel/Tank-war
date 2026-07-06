using TankAssault.Core;
using UnityEngine;

namespace TankAssault.Player
{
    /// <summary>Optional boost/fuel resource. Draining fuel triggers a speed boost with visual exhaust effects.</summary>
    public class FuelSystem : MonoBehaviour
    {
        [SerializeField] private float maxFuel = 100f;
        [SerializeField] private float drainPerSecond = 25f;
        [SerializeField] private float regenPerSecond = 8f;
        [SerializeField] private float boostSpeedMultiplier = 1.6f;
        [SerializeField] private ParticleSystem exhaustEffect;
        [SerializeField] private ParticleSystem boostEffect;

        public float CurrentFuel { get; private set; }
        public bool IsBoosting { get; private set; }

        private TankController _tankController;

        private void Awake()
        {
            CurrentFuel = maxFuel;
            _tankController = GetComponent<TankController>();
        }

        private void Update()
        {
            bool wantsBoost = TankAssault.Input.InputManager.Instance != null && TankAssault.Input.InputManager.Instance.IsBoostHeld;

            IsBoosting = wantsBoost && CurrentFuel > 0f && _tankController.IsMoving;

            if (IsBoosting)
            {
                CurrentFuel = Mathf.Max(0f, CurrentFuel - drainPerSecond * Time.deltaTime);
                _tankController.SetSpeedMultiplier(boostSpeedMultiplier);
            }
            else
            {
                CurrentFuel = Mathf.Min(maxFuel, CurrentFuel + regenPerSecond * Time.deltaTime);
                _tankController.SetSpeedMultiplier(1f);
            }

            SetEffectsActive(IsBoosting);
        }

        private void SetEffectsActive(bool active)
        {
            if (exhaustEffect != null)
            {
                var emission = exhaustEffect.emission;
                emission.rateOverTimeMultiplier = active ? 1f : 0.3f;
            }

            if (boostEffect != null)
            {
                if (active && !boostEffect.isPlaying) boostEffect.Play();
                else if (!active && boostEffect.isPlaying) boostEffect.Stop();
            }
        }

        public float FuelPercent => CurrentFuel / maxFuel;
    }
}
