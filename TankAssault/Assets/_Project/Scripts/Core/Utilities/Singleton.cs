using UnityEngine;

namespace TankAssault.Core
{
    /// <summary>Generic MonoBehaviour singleton. Persists across scene loads.</summary>
    public abstract class Singleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;
        [SerializeField] private bool dontDestroyOnLoad = true;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        var go = new GameObject(typeof(T).Name);
                        _instance = go.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this as T)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this as T;
            if (dontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this as T)
                _instance = null;
        }
    }
}
