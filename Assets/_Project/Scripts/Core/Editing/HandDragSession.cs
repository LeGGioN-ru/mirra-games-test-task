using System;
using ClockApp.Core.Timekeeping;

namespace ClockApp.Core.Editing
{
    public sealed class HandDragSession
    {
        private const double MinutesPerMinuteHandTurn = 60d;
        private const double MinutesPerHourHandTurn = 720d;

        private readonly double _minutesPerDegree;
        private double _minutes;
        private float _lastPointerAngle;

        public HandDragSession(ClockHand hand, TimeSpan startTimeOfDay, float startPointerAngle)
        {
            _minutesPerDegree = (hand == ClockHand.Hour ? MinutesPerHourHandTurn : MinutesPerMinuteHandTurn) / TimeOfDayMath.DegreesPerTurn;
            _minutes = TimeOfDayMath.Wrap(Math.Floor(startTimeOfDay.TotalMinutes), TimeOfDayMath.MinutesPerDay);
            _lastPointerAngle = startPointerAngle;
            TimeOfDay = Snap(_minutes);
        }

        public TimeSpan TimeOfDay { get; private set; }

        public bool MoveTo(float pointerAngle)
        {
            var delta = DialMath.DeltaAngle(_lastPointerAngle, pointerAngle);
            _lastPointerAngle = pointerAngle;
            _minutes = TimeOfDayMath.Wrap(_minutes + delta * _minutesPerDegree, TimeOfDayMath.MinutesPerDay);

            var snapped = Snap(_minutes);

            if (snapped == TimeOfDay)
            {
                return false;
            }

            TimeOfDay = snapped;
            return true;
        }

        public void Rebase(float pointerAngle)
        {
            _lastPointerAngle = pointerAngle;
        }

        private static TimeSpan Snap(double minutes)
        {
            var wholeMinutes = Math.Round(minutes, MidpointRounding.AwayFromZero);
            return TimeSpan.FromMinutes(TimeOfDayMath.Wrap(wholeMinutes, TimeOfDayMath.MinutesPerDay));
        }
    }
}
