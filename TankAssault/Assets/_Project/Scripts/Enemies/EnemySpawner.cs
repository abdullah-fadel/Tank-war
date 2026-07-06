using TankAssault.Core;
using TankAssault.Data;
using UnityEngine;

namespace TankAssault.Enemies
{
    /// <summary>A single placed spawn point in a level; requests a pooled enemy instance when triggered by EnemySpawnManager.</summary>
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyData enemyToSpawn;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private bool spawnOnStart;

        private void Start()
        {
            if (spawnOnStart) Spawn();
        }

        public GameObject Spawn()
        {
            if (enemyPrefab == null) return null;
            var instance = PoolManager.Instance.Spawn(enemyPrefab, transform.position, transform.rotation);
            return instance;
        }

        public EnemyData EnemyData => enemyToSpawn;
    }
}
