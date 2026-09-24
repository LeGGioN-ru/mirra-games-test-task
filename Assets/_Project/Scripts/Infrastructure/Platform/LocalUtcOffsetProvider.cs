using System;
using ClockApp.Core.Platform;

namespace ClockApp.Infrastructure.Platform
{
    public sealed class LocalUtcOffsetProvider : IUtcOffsetProvider
    {
        private long _cachedMinute = long.MinValue;
        private TimeSpan _cachedOffset;

        public TimeSpan GetUtcOffset(DateTime utc)
        {
            var normalizedUtc = UtcDateTime.From(utc);
            var minute = normalizedUtc.Ticks / TimeSpan.TicksPerMinute;

            if (minute != _cachedMinute)
            {
                _cachedOffset = TimeZoneInfo.Local.GetUtcOffset(normalizedUtc);
                _cachedMinute = minute;
            }

            return _cachedOffset;
        }
    }
}
