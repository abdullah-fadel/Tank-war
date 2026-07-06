using UnityEngine;

namespace TankAssault.Optimization
{
    /// <summary>
    /// Documents/enforces the occlusion culling setup expected for each level scene. Unity's
    /// occlusion data is baked per-scene via Window > Rendering > Occlusion Culling and cannot
    /// be generated from a script; this component validates at edit-time that static occluders
    /// (terrain, buildings, hills) are correctly flagged so the bake produces useful cell data.
    /// </summary>
    public class OcclusionCullingSetup : MonoBehaviour
    {
        [SerializeField] private GameObject[] staticOccluders;
        [SerializeField] private GameObject[] staticOccludees;

#if UNITY_EDITOR
        [ContextMenu("Flag Static Occluders/Occludees")]
        private void FlagStaticFlags()
        {
            foreach (var go in staticOccluders)
                if (go != null) UnityEditor.GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.OccluderStatic);

            foreach (var go in staticOccludees)
                if (go != null) UnityEditor.GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.OccludeeStatic);
        }
#endif
    }
}
