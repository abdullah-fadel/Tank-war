using TankAssault.Core;
using UnityEngine;
using UnityEngine.UI;

namespace TankAssault.UI
{
    public class PauseMenuUI : UIScreen
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button quitToMenuButton;
        [SerializeField] private Button settingsButton;

        protected override void Awake()
        {
            base.Awake();
            resumeButton.onClick.AddListener(() => GameManager.Instance.ResumeGame());
            restartButton.onClick.AddListener(OnRestartClicked);
            quitToMenuButton.onClick.AddListener(OnQuitToMenuClicked);
            settingsButton.onClick.AddListener(() => UIManager.Instance.ShowOverlay(UIScreenId.Settings));
        }

        private void OnRestartClicked()
        {
            var levelId = GameManager.Instance.CurrentLevelId;
            GameManager.Instance.ResumeGame();
            GameManager.Instance.StartLevel(levelId);
        }

        private void OnQuitToMenuClicked()
        {
            GameManager.Instance.ResumeGame();
            GameManager.Instance.ChangeState(GameState.MainMenu);
        }
    }
}
