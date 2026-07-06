using UnityEngine;

namespace TankAssault.Optimization
{
    /// <summary>
    /// Ensures materials shared by repeated props (rocks, barrels, foliage, debris) have GPU
    /// Instancing enabled so Unity batches them into single draw calls automatically, which
    /// matters far more on mobile tile-based GPUs than on desktop.
    /// </summary>
    public static class GPUInstancingHelper
    {
        public static void EnableInstancingOnRenderers(Renderer[] renderers)
        {
            foreach (var renderer in renderers)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material != null && !material.enableInstancing)
                        material.enableInstancing = true;
                }
            }
        }

        public static void EnableInstancingInScene()
        {
            var allRenderers = Object.FindObjectsOfType<Renderer>();
            EnableInstancingOnRenderers(allRenderers);
        }
    }
}
