using TankAssault.Core;
using UnityEngine;

namespace TankAssault.VFX
{
    /// <summary>Thin convenience wrapper so gameplay code can spawn common VFX by category without holding prefab references everywhere.</summary>
    public class VFXPoolManager : Singleton<VFXPoolManager>
    {
        [SerializeField] private GameObject defaultExplosionPrefab;
        [SerializeField] private GameObject defaultMuzzleFlashPrefab;
        [SerializeField] private GameObject defaultImpactPrefab;

        public void SpawnExplosion(Vector3 position) =>
            PoolManager.Instance.Spawn(defaultExplosionPrefab, position, Quaternion.identity);

        public void SpawnMuzzleFlash(Vector3 position, Quaternion rotation) =>
            PoolManager.Instance.Spawn(defaultMuzzleFlashPrefab, position, rotation);

        public void SpawnImpact(Vector3 position, Quaternion rotation) =>
            PoolManager.Instance.Spawn(defaultImpactPrefab, position, rotation);
    }
}
