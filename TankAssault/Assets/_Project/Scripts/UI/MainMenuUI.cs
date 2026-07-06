using TankAssault.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TankAssault.UI
{
    public class MainMenuUI : UIScreen
    {
        [SerializeField] private Button playButton;
        [SerializeField] private Button shopButton;
        [SerializeField] private Button garageButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button dailyRewardButton;
        [SerializeField] private GameObject dailyRewardBadge;

        protected override void Awake()
        {
            base.Awake();
            playButton.onClick.AddListener(() => GameManager.Instance.ChangeState(GameState.LevelSelect));
            shopButton.onClick.AddListener(() => UIManager.Instance.ShowPage(UIScreenId.Shop));
            garageButton.onClick.AddListener(() => UIManager.Instance.ShowPage(UIScreenId.Garage));
            settingsButton.onClick.AddListener(() => UIManager.Instance.ShowOverlay(UIScreenId.Settings));
            dailyRewardButton.onClick.AddListener(OnDailyRewardClicked);
        }

        private void OnEnable()
        {
            if (dailyRewardBadge != null && Progression.DailyRewardManager.Instance != null)
                dailyRewardBadge.SetActive(Progression.DailyRewardManager.Instance.IsRewardAvailable());
        }

        private void OnDailyRewardClicked()
        {
            int reward = Progression.DailyRewardManager.Instance.ClaimDailyReward();
            if (dailyRewardBadge != null) dailyRewardBadge.SetActive(false);
            Debug.Log($"[MainMenuUI] Claimed daily reward: {reward} coins");
        }
    }
}
