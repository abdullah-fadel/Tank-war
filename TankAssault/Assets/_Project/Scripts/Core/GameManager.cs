using UnityEngine;

namespace TankAssault.Core
{
    public enum GameState
    {
        Boot,
        MainMenu,
        LevelSelect,
        Loading,
        Playing,
        Paused,
        Victory,
        Defeat
    }

    /// <summary>
    /// Top-level game state owner. UI screens and gameplay systems react to
    /// GameStateChangedEvent rather than polling this class directly.
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        public GameState State { get; private set; } = GameState.Boot;

        [Header("Session")]
        public string CurrentLevelId;
        public int CurrentScore;
        public int CurrentCombo;

        public void ChangeState(GameState newState)
        {
            if (newState == State) return;

            var previous = State;
            State = newState;

            Time.timeScale = newState == GameState.Paused ? 0f : 1f;

            EventBus.Publish(new GameStateChangedEvent(previous, newState));
        }

        public void StartLevel(string levelId)
        {
            CurrentLevelId = levelId;
            CurrentScore = 0;
            CurrentCombo = 0;
            ChangeState(GameState.Loading);
        }

        public void OnLevelPlayable()
        {
            ChangeState(GameState.Playing);
        }

        public void PauseGame()
        {
            if (State == GameState.Playing)
                ChangeState(GameState.Paused);
        }

        public void ResumeGame()
        {
            if (State == GameState.Paused)
                ChangeState(GameState.Playing);
        }

        public void CompleteLevel(int stars)
        {
            var progress = SaveSystem.Instance.Current;
            var entry = progress.LevelProgress.Find(e => e.LevelId == CurrentLevelId);
            if (entry == null)
            {
                entry = new LevelProgressEntry { LevelId = CurrentLevelId };
                progress.LevelProgress.Add(entry);
            }
            entry.Completed = true;
            entry.Stars = Mathf.Max(entry.Stars, stars);
            entry.BestScore = Mathf.Max(entry.BestScore, CurrentScore);
            SaveSystem.Instance.Save();

            EventBus.Publish(new LevelCompletedEvent(stars, CurrentScore));
            ChangeState(GameState.Victory);
        }

        public void FailLevel()
        {
            ChangeState(GameState.Defeat);
        }

        public void AddScore(int amount)
        {
            float multiplier = 1f + (CurrentCombo * 0.1f);
            CurrentScore += Mathf.RoundToInt(amount * multiplier);
        }
    }
}
