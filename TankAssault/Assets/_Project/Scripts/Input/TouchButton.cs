using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TankAssault.Input
{
    /// <summary>Generic multi-touch capable button used for Fire/Aim/Missile/Skill controls.</summary>
    public class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public UnityEvent OnPressed;
        public UnityEvent OnReleased;

        public bool IsHeld { get; private set; }
        private int _pointerId = -2;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (IsHeld) return;
            IsHeld = true;
            _pointerId = eventData.pointerId;
            OnPressed?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId != _pointerId) return;
            IsHeld = false;
            _pointerId = -2;
            OnReleased?.Invoke();
        }
    }
}
