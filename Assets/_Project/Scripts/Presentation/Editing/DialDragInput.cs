using System;
using ClockApp.Core.Editing;
using ClockApp.Presentation.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ClockApp.Presentation.Editing
{
    [RequireComponent(typeof(NonDrawingGraphic))]
    public sealed class DialDragInput : MonoBehaviour,
        IInitializePotentialDragHandler,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler,
        ICanvasRaycastFilter
    {
        private const int NoPointer = int.MinValue;

        [SerializeField] private NonDrawingGraphic _hitArea;

        private int _activePointerId = NoPointer;

        public event Action<DialPointer> DragStarted;

        public event Action<DialPointer> Dragged;

        public event Action DragEnded;

        public bool IsInteractable
        {
            get => _hitArea.raycastTarget;
            set
            {
                _hitArea.raycastTarget = value;

                if (!value)
                {
                    _activePointerId = NoPointer;
                }
            }
        }

        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            eventData.useDragThreshold = false;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_activePointerId != NoPointer || !TryGetPointer(eventData.position, eventData.pressEventCamera, out var pointer))
            {
                return;
            }

            _activePointerId = eventData.pointerId;
            DragStarted?.Invoke(pointer);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId == _activePointerId && TryGetPointer(eventData.position, eventData.pressEventCamera, out var pointer))
            {
                Dragged?.Invoke(pointer);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.pointerId != _activePointerId)
            {
                return;
            }

            _activePointerId = NoPointer;
            DragEnded?.Invoke();
        }

        public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            return TryGetPointer(screenPoint, eventCamera, out var pointer) && pointer.NormalizedRadius <= 1f;
        }

        private bool TryGetPointer(Vector2 screenPoint, Camera eventCamera, out DialPointer pointer)
        {
            var rectTransform = _hitArea.rectTransform;

            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out var localPoint))
            {
                pointer = default;
                return false;
            }

            var offset = localPoint - rectTransform.rect.center;
            var radius = Mathf.Min(rectTransform.rect.width, rectTransform.rect.height) * 0.5f;
            pointer = new DialPointer(DialMath.PointerAngle(offset.x, offset.y), offset.magnitude / radius);
            return true;
        }
    }
}
