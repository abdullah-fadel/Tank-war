using TankAssault.Core;
using TankAssault.Data;
using UnityEngine;

namespace TankAssault.Audio
{
    /// <summary>Swaps background music per world theme and ducks it during boss cinematic zoom.</summary>
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private AudioClip mainMenuTheme;
        [SerializeField] private float bossDuckVolume = 0.4f;
        [SerializeField] private float duckTransitionSpeed = 2f;

        private void OnEnable()
        {
            EventBus.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnGameStateChanged(GameStateChangedEvent evt)
        {
            if (evt.NewState == GameState.MainMenu)
                AudioManager.Instance.PlayMusic(mainMenuTheme);
        }

        public void PlayWorldTheme(WorldConfig world)
        {
            if (world != null && world.AmbientMusic != null)
                AudioManager.Instance.PlayMusic(world.AmbientMusic);
        }
    }
}
