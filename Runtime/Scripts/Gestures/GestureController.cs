using System;
using System.Collections.Generic;
using System.Text;

using TouchGestures.Drawing;

using UnityEngine;

namespace TouchGestures.Gestures
{
    /// <summary>
    /// Controller that interprets takes pointer input from <see cref="PointerInputManager"/> and detects
    /// directional swipes and detects taps.
    /// </summary>
    [RequireComponent(typeof(PointerInputManager))]
    public class GestureController : MonoBehaviour
    {
        private PointerInputManager inputManager;

        // Maximum duration of a press before it can no longer be considered a tap.
        [Header("Settings"), SerializeField]
        private float maxTapDuration = 0.2f;

        // Maximum distance in screen units that a tap can drift from its original position before
        // it is no longer considered a tap.
        [SerializeField]
        private float maxTapDrift = 5.0f;

        // Maximum duration of a swipe before it is no longer considered to be a valid swipe.
        [SerializeField]
        private float maxSwipeDuration = 0.5f;

        // Minimum distance in screen units that a swipe must move before it is considered a swipe.
        // Note that if this is smaller or equal to maxTapDrift, then it is possible for a user action to be
        // returned as both a swipe and a tap.
        [SerializeField]
        private float minSwipeDistance = 10.0f;

        // How much a swipe should consistently be in the same direction before it is considered a swipe.
        [SerializeField]
        private float swipeDirectionSamenessThreshold = 0.6f;

        [Header("Debug"), SerializeField]
        //private Camera _camera;

        [Multiline(13)]
        private string label;

        // Mapping of input IDs to their active gesture tracking objects.
        private readonly Dictionary<int, ActiveGesture> activeGestures = new Dictionary<int, ActiveGesture>();

        /// <summary>
        /// Event fired when the user presses on the screen.
        /// </summary>
        public event Action<SwipeInput> Pressed;

        /// <summary>
        /// Event fired for every motion (possibly multiple times a frame) of a potential swipe gesture.
        /// </summary>
        public event Action<SwipeInput> PotentiallySwiped;

        /// <summary>
        /// Event fired when a user performs a swipe gesture.
        /// </summary>
        public event Action<SwipeInput> Swiped;

        /// <summary>
        /// Event fired when a user performs a tap gesture, on releasing.
        /// </summary>
        public event Action<TapInput> Tapped;

        protected virtual void Awake()
        {
            inputManager = GetComponent<PointerInputManager>();
        }

        /// <summary>
        /// This function is called when the object becomes enabled and active.
        /// </summary>
        private void OnEnable()
        {
            inputManager.Pressed += OnPressed;
            inputManager.Dragged += OnDragged;
            inputManager.Released += OnReleased;
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled or inactive.
        /// </summary>
        private void OnDisable()
        {
            inputManager.Pressed -= OnPressed;
            inputManager.Dragged -= OnDragged;
            inputManager.Released -= OnReleased;
        }

        /// <summary>
        /// Checks whether a given active gesture will be a valid swipe.
        /// </summary>
        private bool IsValidSwipe(ref ActiveGesture gesture)
        {
            return gesture.TravelDistance >= minSwipeDistance &&
                (gesture.StartTime - gesture.EndTime) <= maxSwipeDuration &&
                gesture.SwipeDirectionSameness >= swipeDirectionSamenessThreshold;
        }

        /// <summary>
        /// Checks whether a given active gesture will be a valid tap.
        /// </summary>
        private bool IsValidTap(ref ActiveGesture gesture)
        {
            return gesture.TravelDistance <= maxTapDrift &&
                (gesture.StartTime - gesture.EndTime) <= maxTapDuration;
        }

        private void OnPressed(PointerInput input, double time)
        {
            Debug.Assert(!activeGestures.ContainsKey(input.InputId));

            var newGesture = new ActiveGesture(input.InputId, input.Position, time);
            activeGestures.Add(input.InputId, newGesture);

#if UNITY_EDITOR
            DebugInfo(newGesture);
#endif

            Pressed?.Invoke(new SwipeInput(newGesture));
        }

        private void OnDragged(PointerInput input, double time)
        {
            if (!activeGestures.TryGetValue(input.InputId, out var existingGesture))
            {
                // Probably caught by UI, or the input was otherwise lost
                return;
            }

            existingGesture.SubmitPoint(input.Position, time);

            if (IsValidSwipe(ref existingGesture))
            {
                PotentiallySwiped?.Invoke(new SwipeInput(existingGesture));
            }

#if UNITY_EDITOR
            DebugInfo(existingGesture);
#endif

        }

        private void OnReleased(PointerInput input, double time)
        {
            if (!activeGestures.TryGetValue(input.InputId, out var existingGesture))
            {
                // Probably caught by UI, or the input was otherwise lost
                return;
            }

            activeGestures.Remove(input.InputId);
            existingGesture.SubmitPoint(input.Position, time);

            if (IsValidSwipe(ref existingGesture))
            {
                Swiped?.Invoke(new SwipeInput(existingGesture));
            }

            if (IsValidTap(ref existingGesture))
            {
                Tapped?.Invoke(new TapInput(existingGesture));
            }

#if UNITY_EDITOR
            DebugInfo(existingGesture);
#endif

        }
#if UNITY_EDITOR
        private void DebugInfo(ActiveGesture gesture)
        {
            if (label == null)return;

            var builder = new StringBuilder();

            builder.AppendFormat("ID: {0}", gesture.InputId);
            builder.AppendLine();
            builder.AppendFormat("Start Position: {0}", gesture.StartPosition);
            builder.AppendLine();
            builder.AppendFormat("Position: {0}", gesture.EndPosition);
            builder.AppendLine();
            builder.AppendFormat("Direction: {0}", (gesture.EndPosition - gesture.StartPosition).normalized);
            builder.AppendLine();
            builder.AppendFormat("Duration: {0}", gesture.EndTime - gesture.StartTime);
            builder.AppendLine();
            builder.AppendFormat("Sameness: {0}", gesture.SwipeDirectionSameness);
            builder.AppendLine();
            builder.AppendFormat("Travel distance: {0}", gesture.TravelDistance);
            builder.AppendLine();
            builder.AppendFormat("Samples: {0}", gesture.Samples);
            builder.AppendLine();
            builder.AppendFormat("Realtime since startup: {0}", Time.realtimeSinceStartup);
            builder.AppendLine();
            builder.AppendFormat("Starting Timestamp: {0}", gesture.StartTime);
            builder.AppendLine();
            builder.AppendFormat("Ending Timestamp: {0}", gesture.EndTime);
            builder.AppendLine();

            label = builder.ToString();
            // if (_camera)
            // {
            //     var worldStart = _camera.ScreenToWorldPoint(gesture.StartPosition);
            //     var worldEnd = _camera.ScreenToWorldPoint(gesture.EndPosition);

            //     worldStart.z += 5;
            //     worldEnd.z += 5;
            // }
        }
#endif
    }
}