using System;

namespace ClockApp.Core.Platform
{
    public interface IUtcOffsetProvider
    {
        TimeSpan GetUtcOffset(DateTime utc);
    }
}
