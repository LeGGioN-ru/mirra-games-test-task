using System;
using ClockApp.Core.Timekeeping;
using DG.Tweening;
using UnityEngine;

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

        private Func<float, float> _tickEaseFunction;
        private long _shownWholeSecond = long.MinValue;
        private float _shownSecondAngle = float.NaN;

        private void Awake()
        {
            _tickEaseFunction = EvaluateTickEase;
        }

        public void Show(double secondsOfDay)
        {
            var wholeSecond = (long)Math.Floor(secondsOfDay);

            if (wholeSecond != _shownWholeSecond)
            {
                _shownWholeSecond = wholeSecond;
                var angles = ClockHandAngles.FromSecondsOfDay(wholeSecond);
                Rotate(_hourHand, angles.Hour);
                Rotate(_minuteHand, angles.Minute);
            }

            var secondAngle = SecondHandMotion.TickAngle(secondsOfDay, _tickDuration, _tickEaseFunction);

            if (secondAngle != _shownSecondAngle)
            {
                _shownSecondAngle = secondAngle;
                Rotate(_secondHand, secondAngle);
            }
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
