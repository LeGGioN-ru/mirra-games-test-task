using System;
using ClockApp.Core.Platform;

namespace ClockApp.Tests.Fakes
{
    internal sealed class FakeWallClock : IWallClock
    {
        public FakeWallClock(DateTime utcNow)
        {
            UtcNow = utcNow;
        }

        public DateTime UtcNow { get; set; }

        public void Advance(double seconds)
        {
            UtcNow = UtcNow.AddSeconds(seconds);
        }
    }
}
