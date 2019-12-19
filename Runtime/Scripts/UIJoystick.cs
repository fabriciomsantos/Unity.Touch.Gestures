using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
namespace TouchGestures.Controls
{
    public class UIJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [System.Serializable]
        public class Vector2Event : UnityEvent<Vector2>
        { }

        #region Public Variables
        public bool activeInput = true;
        public bool invertInput;
        public bool resetOnEnable;
        public float movementRange = 50;
        public Vector2 direction = Vector2.zero;
        public Vector2Event directionEvent = new Vector2Event();
        #endregion

        #region Private Variables
        private Vector2 deltaValue;
        private Vector2 pressPosition;
        #endregion

        #region Unity Methods

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            if (resetOnEnable)
            {
                direction = Vector2.zero;

                directionEvent.Invoke(direction);
            }
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData == null)
            {
                return;
            }

            pressPosition = eventData.pressPosition;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData == null)
            {
                return;
            }

            deltaValue = eventData.position - pressPosition;
            deltaValue = Vector2.ClampMagnitude(deltaValue, movementRange);

            direction.x = deltaValue.x / movementRange;
            direction.y = deltaValue.y / movementRange;
            direction.y = invertInput ? direction.y * -1 : direction.y;

            if (activeInput)
            {
                directionEvent.Invoke(direction);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            direction = Vector2.zero;

            directionEvent.Invoke(direction);
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