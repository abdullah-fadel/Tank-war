using UnityEngine;

namespace TankAssault.Core
{
    public static class MathUtils
    {
        /// <summary>Remaps a value from one range to another, clamped.</summary>
        public static float Remap(float value, float inMin, float inMax, float outMin, float outMax)
        {
            float t = Mathf.InverseLerp(inMin, inMax, value);
            return Mathf.Lerp(outMin, outMax, t);
        }

        /// <summary>Rolls a percentage chance in the range [0,100].</summary>
        public static bool RollChance(float percentChance)
        {
            return Random.Range(0f, 100f) < percentChance;
        }

        /// <summary>Signed angle in degrees between forward direction and target on the XY (side-scroll) plane.</summary>
        public static float SignedAngle2D(Vector2 from, Vector2 to)
        {
            return Vector2.SignedAngle(from, to);
        }
    }
}
