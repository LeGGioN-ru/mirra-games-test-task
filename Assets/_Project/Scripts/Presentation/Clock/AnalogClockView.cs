using System;
using ClockApp.Core.Timekeeping;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ClockApp.Presentation.Clock
{
    public sealed class AnalogClockView : MonoBehaviour
    {
        [SerializeField] private RectTransform _hourHand;
        [SerializeField] private RectTransform _minuteHand;
        [SerializeField] private RectTransform _secondHand;
        [SerializeField, Range(0f, 0.5f)] private float _tickDuration = 0.2f;
        [SerializeField] private Ease _tickEase = Ease.OutBack;
        [SerializeField] private float _tickOvershoot = 2.5f;
        [SerializeField] private Color _editableHandColor = new Color32(255, 200, 87, 255);
        [SerializeField] private float _highlightDuration = 0.2f;

        private Func<float, float> _tickEaseFunction;
        private Graphic _hourHandGraphic;
        private Graphic _minuteHandGraphic;
        private Color _normalHandColor;
        private long _shownWholeSecond = long.MinValue;
        private float _restingSecondAngle;
        private float _shownSecondAngle = float.NaN;

        private void Awake()
        {
            _tickEaseFunction = EvaluateTickEase;
            _hourHandGraphic = _hourHand.GetComponent<Graphic>();
            _minuteHandGraphic = _minuteHand.GetComponent<Graphic>();
            _normalHandColor = _minuteHandGraphic.color;
        }

        public void Show(double secondsOfDay, bool animateTick)
        {
            var wholeSecond = (long)Math.Floor(secondsOfDay);

            if (wholeSecond != _shownWholeSecond)
            {
                _shownWholeSecond = wholeSecond;
                var angles = ClockHandAngles.FromSecondsOfDay(wholeSecond);
                Rotate(_hourHand, angles.Hour);
                Rotate(_minuteHand, angles.Minute);
                _restingSecondAngle = angles.Second;
            }

            var secondAngle = animateTick
                ? SecondHandMotion.TickAngle(secondsOfDay, _tickDuration, _tickEaseFunction)
                : _restingSecondAngle;

            if (secondAngle != _shownSecondAngle)
            {
                _shownSecondAngle = secondAngle;
                Rotate(_secondHand, secondAngle);
            }
        }

        public void SetEditableHandsHighlighted(bool isHighlighted)
        {
            var targetColor = isHighlighted ? _editableHandColor : _normalHandColor;
            Tint(_hourHandGraphic, targetColor);
            Tint(_minuteHandGraphic, targetColor);
        }

        private void Tint(Graphic graphic, Color targetColor)
        {
            graphic.DOKill();
            graphic.DOColor(targetColor, _highlightDuration).SetUpdate(true).SetLink(graphic.gameObject);
        }

        private float EvaluateTickEase(float progress)
        {
            return DOVirtual.EasedValue(0f, 1f, progress, _tickEase, _tickOvershoot);
        }

        private static void Rotate(Transform hand, float clockwiseAngle)
        {
            hand.localRotation = Quaternion.Euler(0f, 0f, -clockwiseAngle);
        }
    }
}
