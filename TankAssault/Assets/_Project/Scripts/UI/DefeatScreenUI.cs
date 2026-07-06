using TankAssault.Core;
using TankAssault.Economy;
using UnityEngine;
using UnityEngine.UI;

namespace TankAssault.UI
{
    public class DefeatScreenUI : UIScreen
    {
        [SerializeField] private Button retryButton;
        [SerializeField] private Button reviveWithAdButton;
        [SerializeField] private Button menuButton;
        [SerializeField] private int reviveGemCost = 20;

        protected override void Awake()
        {
            base.Awake();
            retryButton.onClick.AddListener(() => GameManager.Instance.StartLevel(GameManager.Instance.CurrentLevelId));
            menuButton.onClick.AddListener(() => GameManager.Instance.ChangeState(GameState.MainMenu));
            reviveWithAdButton.onClick.AddListener(OnReviveClicked);
        }

        private void OnReviveClicked()
        {
            AdsManager.ShowRewardedAdForCoins(0);
            GameManager.Instance.ChangeState(GameState.Playing);
            Hide();
        }
    }
}
