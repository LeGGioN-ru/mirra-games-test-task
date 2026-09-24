namespace ClockApp.Core.Timekeeping
{
    public static class TimeOfDayMath
    {
        public const double SecondsPerMinute = 60d;
        public const double SecondsPerHour = 3600d;
        public const double SecondsPerHalfDay = 43200d;
        public const double SecondsPerDay = 86400d;
        public const double MinutesPerDay = 1440d;
        public const double DegreesPerTurn = 360d;

        public static double Wrap(double value, double period)
        {
            var remainder = value % period;

            if (remainder < 0d)
            {
                remainder += period;
            }

            return remainder >= period ? 0d : remainder;
        }
    }
}
