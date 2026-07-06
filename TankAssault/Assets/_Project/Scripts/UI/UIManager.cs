using TankAssault.Core;
using UnityEngine;

namespace TankAssault.UI
{
    public enum UIScreenId
    {
        MainMenu,
        Settings,
        Shop,
        Inventory,
        Garage,
        LevelSelect,
        Pause,
        Victory,
        Defeat,
        Hud
    }

    /// <summary>
    /// Central UI router: exactly one "page" screen (menu/shop/garage/level-select) is visible
    /// at a time, while HUD and overlay screens (pause/victory/defeat) can stack on top of it
    /// during gameplay.
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        [System.Serializable]
        public class ScreenEntry
        {
            public UIScreenId Id;
            public UIScreen Screen;
        }

        [SerializeField] private ScreenEntry[] screens;
        private UIScreenId _currentPageScreen;

        protected override void Awake()
        {
            base.Awake();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private UIScreen Find(UIScreenId id)
        {
            foreach (var entry in screens)
                if (entry.Id == id) return entry.Screen;
            return null;
        }

        public void ShowPage(UIScreenId id)
        {
            Find(_currentPageScreen)?.Hide();
            _currentPageScreen = id;
            Find(id)?.Show();
        }

        public void ShowOverlay(UIScreenId id) => Find(id)?.Show();
        public void HideOverlay(UIScreenId id) => Find(id)?.Hide();

        private void OnGameStateChanged(GameStateChangedEvent evt)
        {
            switch (evt.NewState)
            {
                case GameState.MainMenu:
                    ShowPage(UIScreenId.MainMenu);
                    HideOverlay(UIScreenId.Hud);
                    break;
                case GameState.LevelSelect:
                    ShowPage(UIScreenId.LevelSelect);
                    break;
                case GameState.Playing:
                    ShowOverlay(UIScreenId.Hud);
                    HideOverlay(UIScreenId.Pause);
                    break;
                case GameState.Paused:
                    ShowOverlay(UIScreenId.Pause);
                    break;
                case GameState.Victory:
                    ShowOverlay(UIScreenId.Victory);
                    break;
                case GameState.Defeat:
                    ShowOverlay(UIScreenId.Defeat);
                    break;
            }
        }
    }
}
