using System.Collections;
using UnityEngine;

namespace TankAssault.UI
{
    /// <summary>Base class for every full-screen UI panel; provides a consistent fade+scale show/hide transition.</summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class UIScreen : MonoBehaviour
    {
        [SerializeField] protected float transitionDuration = 0.25f;
        [SerializeField] protected AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        protected CanvasGroup CanvasGroup;
        private Coroutine _transitionRoutine;

        protected virtual void Awake()
        {
            CanvasGroup = GetComponent<CanvasGroup>();
        }

        public virtual void Show(bool instant = false)
        {
            gameObject.SetActive(true);
            RunTransition(1f, instant);
        }

        public virtual void Hide(bool instant = false)
        {
            RunTransition(0f, instant, () => gameObject.SetActive(false));
        }

        private void RunTransition(float targetAlpha, bool instant, System.Action onComplete = null)
        {
            if (_transitionRoutine != null) StopCoroutine(_transitionRoutine);

            if (instant || !gameObject.activeInHierarchy)
            {
                CanvasGroup.alpha = targetAlpha;
                CanvasGroup.interactable = targetAlpha > 0.5f;
                CanvasGroup.blocksRaycasts = targetAlpha > 0.5f;
                transform.localScale = Vector3.one;
                onComplete?.Invoke();
                return;
            }

            _transitionRoutine = StartCoroutine(TransitionRoutine(targetAlpha, onComplete));
        }

        private IEnumerator TransitionRoutine(float targetAlpha, System.Action onComplete)
        {
            float startAlpha = CanvasGroup.alpha;
            float startScale = targetAlpha > 0.5f ? 0.9f : 1f;
            float endScale = targetAlpha > 0.5f ? 1f : 0.95f;
            float elapsed = 0f;

            CanvasGroup.blocksRaycasts = true;

            while (elapsed < transitionDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = transitionCurve.Evaluate(Mathf.Clamp01(elapsed / transitionDuration));
                CanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, t);
                yield return null;
            }

            CanvasGroup.alpha = targetAlpha;
            transform.localScale = Vector3.one;
            CanvasGroup.interactable = targetAlpha > 0.5f;
            CanvasGroup.blocksRaycasts = targetAlpha > 0.5f;

            onComplete?.Invoke();
        }
    }
}
