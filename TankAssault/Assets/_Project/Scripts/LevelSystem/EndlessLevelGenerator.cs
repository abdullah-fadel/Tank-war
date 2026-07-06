using System.Collections.Generic;
using UnityEngine;

namespace TankAssault.LevelSystem
{
    /// <summary>
    /// Procedurally chains pre-built terrain chunk prefabs ahead of the player and recycles
    /// ones that fall behind, giving an effectively infinite endless-mode track without
    /// needing to hand-place kilometres of geometry.
    /// </summary>
    public class EndlessLevelGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject[] chunkPrefabs;
        [SerializeField] private Transform player;
        [SerializeField] private float chunkLength = 40f;
        [SerializeField] private int chunksAheadToKeep = 4;
        [SerializeField] private int chunksBehindToKeep = 1;

        private readonly List<GameObject> _activeChunks = new List<GameObject>();
        private float _nextSpawnX;

        private void Start()
        {
            for (int i = 0; i < chunksAheadToKeep; i++)
                SpawnNextChunk();
        }

        private void Update()
        {
            if (player == null) return;

            while (_nextSpawnX - player.position.x < chunksAheadToKeep * chunkLength)
                SpawnNextChunk();

            while (_activeChunks.Count > 0 && (player.position.x - _activeChunks[0].transform.position.x) > (chunksBehindToKeep + 1) * chunkLength)
            {
                Destroy(_activeChunks[0]);
                _activeChunks.RemoveAt(0);
            }
        }

        private void SpawnNextChunk()
        {
            if (chunkPrefabs == null || chunkPrefabs.Length == 0) return;

            var prefab = chunkPrefabs[Random.Range(0, chunkPrefabs.Length)];
            var chunk = Instantiate(prefab, new Vector3(_nextSpawnX, 0f, 0f), Quaternion.identity, transform);
            _activeChunks.Add(chunk);
            _nextSpawnX += chunkLength;
        }
    }
}
