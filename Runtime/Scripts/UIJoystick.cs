using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TouchGestures.Controls
{
    public class UIJoystick : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [System.Serializable]
        public class Vector2Event : UnityEvent<Vector2>
        { }

        #region Public Variables
        public bool activeInput = true;
        public bool invertInput = false;
        public float movementRange = 50;
        public Vector2 direction = Vector2.zero;
        public Vector2Event directionEvent = new Vector2Event();
        #endregion

        #region Private Variables
        private Vector2 deltaValue = Vector2.zero;
        #endregion

        #region Unity Methods
        public void OnBeginDrag(PointerEventData data)
        {
            deltaValue = Vector2.zero;
        }

        public void OnDrag(PointerEventData data)
        {
            deltaValue = data.position - data.pressPosition;
            deltaValue = Vector2.ClampMagnitude(deltaValue, movementRange);

            direction = new Vector2(deltaValue.x / movementRange, deltaValue.y / movementRange);

            direction.y = invertInput ? direction.y * -1 : direction.y;

            if (activeInput)
            {
                directionEvent.Invoke(direction);
            }
        }

        public void OnEndDrag(PointerEventData data)
        {
            direction = Vector2.zero;
            if (activeInput)
            {
                directionEvent.Invoke(direction);
            }
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