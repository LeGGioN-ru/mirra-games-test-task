using System;

namespace ClockApp.Core.Timekeeping
{
    public static class SecondHandMotion
    {
        private const double DegreesPerSecond = TimeOfDayMath.DegreesPerTurn / TimeOfDayMath.SecondsPerMinute;

        public static float TickAngle(double secondsOfDay, float tickDuration, Func<float, float> ease)
        {
            if (ease == null)
            {
                throw new ArgumentNullException(nameof(ease));
            }

            var secondsOfMinute = TimeOfDayMath.Wrap(secondsOfDay, TimeOfDayMath.SecondsPerMinute);
            var wholeSeconds = Math.Floor(secondsOfMinute);
            var fraction = secondsOfMinute - wholeSeconds;

            if (tickDuration <= 0f || fraction >= tickDuration)
            {
                return (float)(wholeSeconds * DegreesPerSecond);
            }

            var progress = ease((float)(fraction / tickDuration));
            var angle = (wholeSeconds - 1d + progress) * DegreesPerSecond;

            return (float)TimeOfDayMath.Wrap(angle, TimeOfDayMath.DegreesPerTurn);
        }
    }
}
