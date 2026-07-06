using UnityEngine;
using UnityEngine.EventSystems;

namespace TankAssault.Input
{
    /// <summary>
    /// Left-side floating/fixed virtual joystick constrained to the horizontal axis
    /// (this is a side-scroller: only left/right movement is needed).
    /// </summary>
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField] private RectTransform background;
        [SerializeField] private RectTransform handle;
        [SerializeField] private float handleRange = 100f;
        [SerializeField] private bool horizontalOnly = true;

        public float Horizontal { get; private set; }
        public float Vertical { get; private set; }
        public bool IsPressed { get; private set; }

        private Vector2 _inputPointerStart;
        private Canvas _canvas;

        private void Awake()
        {
            _canvas = GetComponentInParent<Canvas>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            IsPressed = true;
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                background, eventData.position, eventData.pressEventCamera, out var localPoint);

            localPoint = Vector2.ClampMagnitude(localPoint, handleRange);
            handle.anchoredPosition = horizontalOnly ? new Vector2(localPoint.x, 0f) : localPoint;

            Horizontal = Mathf.Clamp(localPoint.x / handleRange, -1f, 1f);
            Vertical = horizontalOnly ? 0f : Mathf.Clamp(localPoint.y / handleRange, -1f, 1f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            IsPressed = false;
            Horizontal = 0f;
            Vertical = 0f;
            handle.anchoredPosition = Vector2.zero;
        }
    }
}
