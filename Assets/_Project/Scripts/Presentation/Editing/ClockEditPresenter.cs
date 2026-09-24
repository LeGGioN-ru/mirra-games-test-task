using System;
using ClockApp.Core.Editing;
using ClockApp.Core.Timekeeping;
using ClockApp.Presentation.Clock;
using UnityEngine;
using Zenject;

namespace ClockApp.Presentation.Editing
{
    public sealed class ClockEditPresenter : IInitializable, IDisposable, ITickable
    {
        private const float HourHandReach = 0.55f;
        private const float DeadZoneRadius = 0.15f;

        private readonly IClock _clock;
        private readonly ClockEditModel _editModel;
        private readonly EditPanelView _panelView;
        private readonly DialDragInput _dragInput;
        private readonly AnalogClockView _analogView;
        private readonly DigitalClockView _digitalView;

        private HandDragSession _dragSession;
        private bool _isPointerInDeadZone;
        private bool _isTimeTextValid;

        public ClockEditPresenter(
            IClock clock,
            ClockEditModel editModel,
            EditPanelView panelView,
            DialDragInput dragInput,
            AnalogClockView analogView,
            DigitalClockView digitalView)
        {
            _clock = clock;
            _editModel = editModel;
            _panelView = panelView;
            _dragInput = dragInput;
            _analogView = analogView;
            _digitalView = digitalView;
        }

        public void Initialize()
        {
            _panelView.EditRequested += BeginEditing;
            _panelView.SaveRequested += SaveEditing;
            _panelView.CancelRequested += CancelEditing;
            _panelView.ResetRequested += ResetToSyncedTime;
            _panelView.TimeTextChanged += OnTimeTextChanged;
            _dragInput.DragStarted += OnDragStarted;
            _dragInput.Dragged += OnDragged;
            _dragInput.DragEnded += OnDragEnded;

            _dragInput.IsInteractable = false;
            _panelView.ShowViewing(_clock.IsManuallyAdjusted);
        }

        public void Dispose()
        {
            _panelView.EditRequested -= BeginEditing;
            _panelView.SaveRequested -= SaveEditing;
            _panelView.CancelRequested -= CancelEditing;
            _panelView.ResetRequested -= ResetToSyncedTime;
            _panelView.TimeTextChanged -= OnTimeTextChanged;
            _dragInput.DragStarted -= OnDragStarted;
            _dragInput.Dragged -= OnDragged;
            _dragInput.DragEnded -= OnDragEnded;
        }

        public void Tick()
        {
            if (_editModel.IsEditing && Input.GetKeyDown(KeyCode.Escape))
            {
                CancelEditing();
            }
        }

        private void BeginEditing()
        {
            if (_editModel.IsEditing)
            {
                return;
            }

            var draft = TimeSpan.FromSeconds(Math.Floor(_clock.LocalNow.TimeOfDay.TotalSeconds));
            _editModel.Begin(draft);
            _isTimeTextValid = true;
            _dragInput.IsInteractable = true;
            _digitalView.SetVisible(false);
            _analogView.SetEditableHandsHighlighted(true);
            _panelView.ShowEditing(TimeInputParser.Format(draft));
        }

        private void SaveEditing()
        {
            if (!_editModel.IsEditing || !_isTimeTextValid)
            {
                return;
            }

            _clock.SetLocalTimeOfDay(_editModel.Draft);
            EndEditing();
        }

        private void CancelEditing()
        {
            if (_editModel.IsEditing)
            {
                EndEditing();
            }
        }

        private void EndEditing()
        {
            _dragSession = null;
            _editModel.End();
            _dragInput.IsInteractable = false;
            _digitalView.SetVisible(true);
            _analogView.SetEditableHandsHighlighted(false);
            _panelView.ShowViewing(_clock.IsManuallyAdjusted);
        }

        private void ResetToSyncedTime()
        {
            _clock.ResetToSyncedTime();
            _panelView.ShowViewing(_clock.IsManuallyAdjusted);
        }

        private void OnTimeTextChanged(string text)
        {
            _isTimeTextValid = TimeInputParser.TryParse(text, out var timeOfDay);
            _panelView.SetTimeTextValid(_isTimeTextValid);

            if (_isTimeTextValid)
            {
                _editModel.SetDraft(timeOfDay);
            }
        }

        private void OnDragStarted(DialPointer pointer)
        {
            var angles = ClockHandAngles.FromTimeOfDay(_editModel.Draft);
            var hand = HandPicker.Pick(pointer.Angle, pointer.NormalizedRadius, angles, HourHandReach);

            _dragSession = new HandDragSession(hand, _editModel.Draft, pointer.Angle);
            _isPointerInDeadZone = pointer.NormalizedRadius < DeadZoneRadius;
        }

        private void OnDragged(DialPointer pointer)
        {
            if (_dragSession == null)
            {
                return;
            }

            if (pointer.NormalizedRadius < DeadZoneRadius)
            {
                _isPointerInDeadZone = true;
                return;
            }

            if (_isPointerInDeadZone)
            {
                _isPointerInDeadZone = false;
                _dragSession.Rebase(pointer.Angle);
                return;
            }

            if (_dragSession.MoveTo(pointer.Angle))
            {
                ApplyDraggedTime(_dragSession.TimeOfDay);
            }
        }

        private void OnDragEnded()
        {
            _dragSession = null;
        }

        private void ApplyDraggedTime(TimeSpan timeOfDay)
        {
            _editModel.SetDraft(timeOfDay);
            _isTimeTextValid = true;
            _panelView.SetTimeText(TimeInputParser.Format(timeOfDay));
            _panelView.SetTimeTextValid(true);
        }
    }
}
