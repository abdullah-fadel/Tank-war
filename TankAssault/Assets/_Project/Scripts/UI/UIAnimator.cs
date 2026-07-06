using System.Collections;
using UnityEngine;

namespace TankAssault.UI
{
    /// <summary>Reusable one-off UI animation helpers: button punch-scale, currency counter roll-up, toast pop-in.</summary>
    public static class UIAnimator
    {
        public static IEnumerator PunchScale(Transform target, float scaleAmount = 0.15f, float duration = 0.18f)
        {
            Vector3 baseScale = target.localScale;
            Vector3 peakScale = baseScale * (1f + scaleAmount);
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                float curve = Mathf.Sin(t * Mathf.PI);
                target.localScale = Vector3.Lerp(baseScale, peakScale, curve);
                yield return null;
            }
            target.localScale = baseScale;
        }

        public static IEnumerator CountUpText(TMPro.TMP_Text label, int from, int to, float duration = 0.5f, string format = "{0}")
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                int value = Mathf.RoundToInt(Mathf.Lerp(from, to, elapsed / duration));
                label.text = string.Format(format, value);
                yield return null;
            }
            label.text = string.Format(format, to);
        }
    }
}
