using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
namespace TouchGestures.Controls
{
    public class UIJoystick : OnScreenControl, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [System.Serializable]
        public class Vector2Event : UnityEvent<Vector2>
        { }

        #region Public Variables
        public bool activeInput = true;

        [SerializeField][InputControl(layout = "Vector2")]
        private string m_ControlPath;
        public bool invertInput;
        public float movementRange = 50;
        public Vector2 direction = Vector2.zero;
        public Vector2Event directionEvent = new Vector2Event();
        #endregion

        #region Private Variables
        private Vector2 deltaValue = Vector2.zero;

        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }
        #endregion

        #region Unity Methods
        public void OnPointerDown(PointerEventData data)
        {
            if (data == null)
            {
                return;
            }

            deltaValue = Vector2.zero;
        }

        public void OnDrag(PointerEventData data)
        {
            if (data == null)
            {
                return;
            }

            deltaValue = data.position - data.pressPosition;
            deltaValue = Vector2.ClampMagnitude(deltaValue, movementRange);

            direction.x = deltaValue.x / movementRange;
            direction.y = deltaValue.y / movementRange;
            direction.y = invertInput ? direction.y * -1 : direction.y;

            if (activeInput)
            {
                directionEvent.Invoke(direction);
                SendValueToControl(direction);
            }
        }

        public void OnPointerUp(PointerEventData data)
        {
            direction = Vector2.zero;

            directionEvent.Invoke(direction);
            SendValueToControl(direction);
        }

        #endregion
        public void EnableInput(bool active)
        {
            activeInput = active;
        }

        public void InvertInput(bool invert)
        {
            invertInput = invert;
        }
    }
}