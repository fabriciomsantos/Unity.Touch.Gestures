using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ReplaceMe
{
    public class UISwipe : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [System.Serializable]
        public class Vector2Event : UnityEvent<Vector2> { }

        #region Public Variables

        public float swipeTime = .25f;
        public Vector2 swipeDirection = Vector2.zero;
        public Vector2Event swipeEvent = new Vector2Event();

        #endregion

        #region Private Variables
        private float timeCount;
        private Vector2 deltaValue = Vector2.zero;
        private bool finishSwipe;

        #endregion

        #region Unity Methods
        public void OnBeginDrag(PointerEventData data)
        {
            deltaValue = Vector2.zero;
        }

        public void OnDrag(PointerEventData data)
        {
            deltaValue = (data.position - data.pressPosition).normalized;
            if (data.dragging)
            {
                timeCount += Time.deltaTime;
                if (timeCount >= swipeTime && !finishSwipe)
                {
                    timeCount = 0.0f;
                    swipeDirection = deltaValue;
                    swipeDirection.x = Mathf.Round(swipeDirection.x * 10f) / 10f;
                    swipeDirection.y = Mathf.Round(swipeDirection.y * 10f) / 10f;
                    swipeEvent.Invoke(swipeDirection);
                    finishSwipe = true;
                }
            }
        }

        public void OnEndDrag(PointerEventData data)
        {
            finishSwipe = false;
        }

        #endregion
    }
}