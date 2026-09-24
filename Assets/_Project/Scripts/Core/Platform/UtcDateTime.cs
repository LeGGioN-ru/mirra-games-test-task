using System;

namespace ClockApp.Core.Platform
{
    public static class UtcDateTime
    {
        public static DateTime From(DateTime value)
        {
            return value.Kind == DateTimeKind.Local
                ? value.ToUniversalTime()
                : DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }
    }
}
