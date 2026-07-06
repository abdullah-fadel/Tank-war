using TankAssault.Core;
using TankAssault.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TankAssault.UI
{
    /// <summary>In-gameplay heads-up display: health bar, fuel bar, score, combo, currency, and the pause button.</summary>
    public class HUDUI : UIScreen
    {
        [SerializeField] private Slider healthBar;
        [SerializeField] private Slider fuelBar;
        [SerializeField] private TMP_Text scoreLabel;
        [SerializeField] private TMP_Text comboLabel;
        [SerializeField] private TMP_Text coinsLabel;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Image damageVignette;

        protected override void Awake()
        {
            base.Awake();
            pauseButton.onClick.AddListener(() => GameManager.Instance.PauseGame());
        }

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);
            EventBus.Subscribe<ComboChangedEvent>(OnComboChanged);
            EventBus.Subscribe<CurrencyChangedEvent>(OnCurrencyChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerDamagedEvent>(OnPlayerDamaged);
            EventBus.Unsubscribe<ComboChangedEvent>(OnComboChanged);
            EventBus.Unsubscribe<CurrencyChangedEvent>(OnCurrencyChanged);
        }

        private void Update()
        {
            if (GameManager.Instance != null)
                scoreLabel.text = GameManager.Instance.CurrentScore.ToString();
        }

        private void OnPlayerDamaged(PlayerDamagedEvent evt)
        {
            healthBar.value = evt.CurrentHealth / evt.MaxHealth;
            if (damageVignette != null)
                damageVignette.color = new Color(1f, 0f, 0f, 1f - healthBar.value);
        }

        private void OnComboChanged(ComboChangedEvent evt)
        {
            comboLabel.text = evt.ComboCount > 1 ? $"x{evt.ScoreMultiplier:0.0} COMBO" : "";
        }

        private void OnCurrencyChanged(CurrencyChangedEvent evt)
        {
            coinsLabel.text = evt.Coins.ToString();
        }

        public void SetFuelPercent(float percent) => fuelBar.value = percent;
    }
}
