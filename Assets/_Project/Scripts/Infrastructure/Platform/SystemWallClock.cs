using System;
using ClockApp.Core.Platform;

namespace ClockApp.Infrastructure.Platform
{
    public sealed class SystemWallClock : IWallClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
