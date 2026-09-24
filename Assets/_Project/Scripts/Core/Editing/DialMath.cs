using System;
using ClockApp.Core.Timekeeping;

namespace ClockApp.Core.Editing
{
    public static class DialMath
    {
        private const double HalfTurn = TimeOfDayMath.DegreesPerTurn / 2d;
        private const double DegreesPerRadian = 180d / Math.PI;

        public static float PointerAngle(float x, float y)
        {
            var angle = Math.Atan2(x, y) * DegreesPerRadian;
            return (float)TimeOfDayMath.Wrap(angle, TimeOfDayMath.DegreesPerTurn);
        }

        public static float DeltaAngle(float from, float to)
        {
            var delta = TimeOfDayMath.Wrap(to - (double)from, TimeOfDayMath.DegreesPerTurn);

            if (delta > HalfTurn)
            {
                delta -= TimeOfDayMath.DegreesPerTurn;
            }

            return (float)delta;
        }
    }
}
