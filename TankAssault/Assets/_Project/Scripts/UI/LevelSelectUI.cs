using TankAssault.Core;
using TankAssault.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TankAssault.UI
{
    /// <summary>World and level picker; shows lock state and star ratings sourced from SaveData.LevelProgress.</summary>
    public class LevelSelectUI : UIScreen
    {
        [SerializeField] private WorldConfig[] worlds;
        [SerializeField] private Transform worldListContainer;
        [SerializeField] private LevelSelectCard cardPrefab;
        [SerializeField] private Button backButton;

        protected override void Awake()
        {
            base.Awake();
            backButton.onClick.AddListener(() => UIManager.Instance.ShowPage(UIScreenId.MainMenu));
        }

        public override void Show(bool instant = false)
        {
            base.Show(instant);
            Populate();
        }

        private void Populate()
        {
            foreach (Transform child in worldListContainer)
                Destroy(child.gameObject);

            var save = SaveSystem.Instance.Current;

            for (int worldIndex = 0; worldIndex < worlds.Length; worldIndex++)
            {
                var world = worlds[worldIndex];
                bool worldUnlocked = world.RequiredWorldIndex < 0 || save.HighestUnlockedWorldIndex >= world.RequiredWorldIndex;

                foreach (var level in world.Levels)
                {
                    var card = Instantiate(cardPrefab, worldListContainer);
                    var progress = save.LevelProgress.Find(e => e.LevelId == level.LevelId);
                    int stars = progress != null ? progress.Stars : 0;

                    card.Setup(world.DisplayName, level.DisplayName, stars, worldUnlocked, () =>
                    {
                        GameManager.Instance.StartLevel(level.LevelId);
                    });
                }
            }
        }
    }
}
