using System;

namespace ClockApp.Core.Timekeeping
{
    public readonly struct ClockHandAngles
    {
        public ClockHandAngles(float hour, float minute, float second)
        {
            Hour = hour;
            Minute = minute;
            Second = second;
        }

        public float Hour { get; }

        public float Minute { get; }

        public float Second { get; }

        public static ClockHandAngles FromTimeOfDay(TimeSpan timeOfDay)
        {
            return FromSecondsOfDay(timeOfDay.TotalSeconds);
        }

        public static ClockHandAngles FromSecondsOfDay(double secondsOfDay)
        {
            var seconds = TimeOfDayMath.Wrap(secondsOfDay, TimeOfDayMath.SecondsPerDay);

            return new ClockHandAngles(
                ToAngle(seconds, TimeOfDayMath.SecondsPerHalfDay),
                ToAngle(seconds, TimeOfDayMath.SecondsPerHour),
                ToAngle(seconds, TimeOfDayMath.SecondsPerMinute));
        }

        private static float ToAngle(double seconds, double secondsPerTurn)
        {
            return (float)(seconds % secondsPerTurn / secondsPerTurn * TimeOfDayMath.DegreesPerTurn);
        }
    }
}
