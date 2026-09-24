using System;
using ClockApp.Core.Timekeeping;

namespace ClockApp.Core.Editing
{
    public static class HandPicker
    {
        public static ClockHand Pick(float pointerAngle, float normalizedRadius, ClockHandAngles angles, float hourHandReach)
        {
            if (normalizedRadius > hourHandReach)
            {
                return ClockHand.Minute;
            }

            var distanceToHour = Math.Abs(DialMath.DeltaAngle(pointerAngle, angles.Hour));
            var distanceToMinute = Math.Abs(DialMath.DeltaAngle(pointerAngle, angles.Minute));

            return distanceToHour <= distanceToMinute ? ClockHand.Hour : ClockHand.Minute;
        }
    }
}
