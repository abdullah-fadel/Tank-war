using System.Collections.Generic;
using UnityEngine;

namespace TankAssault.Core
{
    /// <summary>
    /// Central registry of ObjectPool&lt;Component&gt; instances keyed by prefab.
    /// Any system (weapons, VFX, enemy spawner) asks this for pooled instances
    /// instead of calling Instantiate/Destroy directly.
    /// </summary>
    public class PoolManager : Singleton<PoolManager>
    {
        private readonly Dictionary<GameObject, ObjectPool<Transform>> _pools = new Dictionary<GameObject, ObjectPool<Transform>>();
        private Transform _root;

        protected override void Awake()
        {
            base.Awake();
            _root = new GameObject("PooledObjects").transform;
            _root.SetParent(transform);
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!_pools.TryGetValue(prefab, out var pool))
            {
                pool = new ObjectPool<Transform>(prefab.transform, 8, _root);
                _pools[prefab] = pool;
            }
            return pool.Get(position, rotation).gameObject;
        }

        public void Despawn(GameObject prefab, GameObject instance)
        {
            if (_pools.TryGetValue(prefab, out var pool))
                pool.Release(instance.transform);
            else
                Destroy(instance);
        }

        public void Prewarm(GameObject prefab, int count)
        {
            if (!_pools.ContainsKey(prefab))
                _pools[prefab] = new ObjectPool<Transform>(prefab.transform, count, _root);
        }
    }
}
