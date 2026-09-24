using System;

namespace ClockApp.Core.Platform
{
    public interface IWallClock
    {
        DateTime UtcNow { get; }
    }
}
