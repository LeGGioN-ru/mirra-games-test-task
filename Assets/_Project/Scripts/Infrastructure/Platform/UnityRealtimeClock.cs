using ClockApp.Core.Platform;
using UnityEngine;

namespace ClockApp.Infrastructure.Platform
{
    public sealed class UnityRealtimeClock : IRealtimeClock
    {
        public double SecondsSinceStartup => Time.realtimeSinceStartupAsDouble;
    }
}
