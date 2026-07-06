using TankAssault.Core;
using TankAssault.Data;
using TankAssault.Player;
using UnityEngine;

namespace TankAssault.LevelSystem
{
    /// <summary>
    /// Owns the current level's runtime state: active checkpoint, respawn, mode-specific win
    /// condition, and score-to-stars conversion. Story/Boss levels call CompleteLevel() from a
    /// LevelEndZone trigger; Survival levels call it from a survival timer; Endless never
    /// completes and instead ends via FailLevel() on player death.
    /// </summary>
    public class LevelManager : Singleton<LevelManager>
    {
        [SerializeField] private WorldConfig worldConfig;
        [SerializeField] private int levelIndex;
        [SerializeField] private TankController playerController;
        [SerializeField] private TankHealth playerHealth;

        private Vector3 _lastCheckpointPosition;
        private int _lastCheckpointIndex;
        public LevelInfo CurrentLevelInfo { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            if (worldConfig != null && worldConfig.Levels.Length > levelIndex)
                CurrentLevelInfo = worldConfig.Levels[levelIndex];
        }

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerDiedEvent>(OnPlayerDied);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerDiedEvent>(OnPlayerDied);
        }

        public void SetActiveCheckpoint(int index, Vector3 position)
        {
            if (index <= _lastCheckpointIndex) return;
            _lastCheckpointIndex = index;
            _lastCheckpointPosition = position;
        }

        private void OnPlayerDied(PlayerDiedEvent evt)
        {
            GameManager.Instance.FailLevel();
        }

        public void RespawnPlayerAtCheckpoint()
        {
            if (playerController == null) return;
            playerController.transform.position = _lastCheckpointPosition;
            playerController.ControlsEnabled = true;
        }

        public void CompleteLevel()
        {
            int stars = CalculateStars(GameManager.Instance.CurrentScore);
            GameManager.Instance.CompleteLevel(stars);
        }

        private int CalculateStars(int score)
        {
            if (CurrentLevelInfo == null) return 1;
            if (score >= CurrentLevelInfo.StarThresholdScore3) return 3;
            if (score >= CurrentLevelInfo.StarThresholdScore2) return 2;
            if (score >= CurrentLevelInfo.StarThresholdScore1) return 1;
            return 0;
        }
    }
}
