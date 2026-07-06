using System.Collections;
using System.Collections.Generic;
using TankAssault.Core;
using UnityEngine;

namespace TankAssault.Enemies
{
    /// <summary>
    /// Drives wave-based spawning for story levels and continuous scaling spawns for endless mode.
    /// Story mode reads waves from LevelInfo.EnemyWaveIds (matched against named spawn groups in
    /// the scene); Endless mode spawns from a rotating pool with escalating difficulty over time.
    /// </summary>
    public class EnemySpawnManager : Singleton<EnemySpawnManager>
    {
        [System.Serializable]
        public class SpawnGroup
        {
            public string WaveId;
            public EnemySpawner[] Spawners;
        }

        [Header("Story Mode")]
        [SerializeField] private SpawnGroup[] storyWaves;
        [SerializeField] private float delayBetweenWaves = 3f;

        [Header("Endless Mode")]
        [SerializeField] private GameObject[] endlessEnemyPool;
        [SerializeField] private Transform[] endlessSpawnPoints;
        [SerializeField] private float endlessInitialInterval = 3f;
        [SerializeField] private float endlessMinInterval = 0.6f;
        [SerializeField] private float endlessDifficultyRampSeconds = 60f;

        private int _activeEnemyCount;

        public void BeginStoryWaves()
        {
            StartCoroutine(StoryWaveRoutine());
        }

        private IEnumerator StoryWaveRoutine()
        {
            foreach (var wave in storyWaves)
            {
                foreach (var spawner in wave.Spawners)
                {
                    spawner.Spawn();
                    _activeEnemyCount++;
                }
                yield return new WaitForSeconds(delayBetweenWaves);
            }
        }

        public void BeginEndlessMode()
        {
            StartCoroutine(EndlessSpawnRoutine());
        }

        private IEnumerator EndlessSpawnRoutine()
        {
            float elapsed = 0f;
            while (true)
            {
                elapsed += endlessInitialInterval;
                float difficultyT = Mathf.Clamp01(elapsed / endlessDifficultyRampSeconds);
                float interval = Mathf.Lerp(endlessInitialInterval, endlessMinInterval, difficultyT);

                SpawnRandomEndlessEnemy();

                yield return new WaitForSeconds(interval);
            }
        }

        private void SpawnRandomEndlessEnemy()
        {
            if (endlessEnemyPool == null || endlessEnemyPool.Length == 0) return;
            if (endlessSpawnPoints == null || endlessSpawnPoints.Length == 0) return;

            var prefab = endlessEnemyPool[Random.Range(0, endlessEnemyPool.Length)];
            var point = endlessSpawnPoints[Random.Range(0, endlessSpawnPoints.Length)];
            PoolManager.Instance.Spawn(prefab, point.position, point.rotation);
        }
    }
}
