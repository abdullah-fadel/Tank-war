using UnityEngine;

namespace TankAssault.Optimization
{
    /// <summary>
    /// Runtime helper that builds a LODGroup from a set of pre-authored mesh variants when one
    /// hasn't been baked into the prefab by an artist. Keeps enemy/tank prefabs cheap to render
    /// at a distance on mid-range Android GPUs.
    /// </summary>
    [RequireComponent(typeof(LODGroup))]
    public class LODSetup : MonoBehaviour
    {
        [System.Serializable]
        public class LODLevel
        {
            public Renderer[] Renderers;
            [Range(0f, 1f)] public float ScreenRelativeHeight = 0.3f;
        }

        [SerializeField] private LODLevel[] levels;

        private void Awake()
        {
            var group = GetComponent<LODGroup>();
            var lods = new LOD[levels.Length];

            for (int i = 0; i < levels.Length; i++)
                lods[i] = new LOD(levels[i].ScreenRelativeHeight, levels[i].Renderers);

            group.SetLODs(lods);
            group.RecalculateBounds();
        }
    }
}
