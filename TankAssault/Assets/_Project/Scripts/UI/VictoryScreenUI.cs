using TankAssault.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TankAssault.UI
{
    public class VictoryScreenUI : UIScreen
    {
        [SerializeField] private TMP_Text scoreLabel;
        [SerializeField] private GameObject[] starIcons;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private Button replayButton;
        [SerializeField] private Button menuButton;

        private void OnEnable()
        {
            EventBus.Subscribe<LevelCompletedEvent>(OnLevelCompleted);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<LevelCompletedEvent>(OnLevelCompleted);
        }

        protected override void Awake()
        {
            base.Awake();
            nextLevelButton.onClick.AddListener(() => GameManager.Instance.ChangeState(GameState.LevelSelect));
            replayButton.onClick.AddListener(() => GameManager.Instance.StartLevel(GameManager.Instance.CurrentLevelId));
            menuButton.onClick.AddListener(() => GameManager.Instance.ChangeState(GameState.MainMenu));
        }

        private void OnLevelCompleted(LevelCompletedEvent evt)
        {
            scoreLabel.text = $"Score: {evt.Score}";
            for (int i = 0; i < starIcons.Length; i++)
                starIcons[i].SetActive(i < evt.Stars);
        }
    }
}
