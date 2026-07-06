using System.Collections.Generic;
using UnityEngine;

namespace TankAssault.Core
{
    /// <summary>
    /// Generic component pool for one prefab. Avoids Instantiate/Destroy churn for
    /// projectiles, VFX, and enemies which is critical for mobile frame pacing.
    /// </summary>
    public class ObjectPool<T> where T : Component
    {
        private readonly Queue<T> _available = new Queue<T>();
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly HashSet<T> _all = new HashSet<T>();

        public ObjectPool(T prefab, int prewarmCount, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;
            for (int i = 0; i < prewarmCount; i++)
                CreateNew();
        }

        private T CreateNew()
        {
            var instance = Object.Instantiate(_prefab, _parent);
            instance.gameObject.SetActive(false);
            _available.Enqueue(instance);
            _all.Add(instance);
            return instance;
        }

        public T Get(Vector3 position, Quaternion rotation)
        {
            T instance = _available.Count > 0 ? _available.Dequeue() : CreateNew();

            var t = instance.transform;
            t.SetPositionAndRotation(position, rotation);
            instance.gameObject.SetActive(true);
            return instance;
        }

        public void Release(T instance)
        {
            if (!_all.Contains(instance)) return;
            instance.gameObject.SetActive(false);
            instance.transform.SetParent(_parent);
            _available.Enqueue(instance);
        }

        public int TotalCount => _all.Count;
        public int AvailableCount => _available.Count;
    }
}
