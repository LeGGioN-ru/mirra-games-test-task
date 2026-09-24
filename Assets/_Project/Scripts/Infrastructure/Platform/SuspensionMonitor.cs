using System;
using ClockApp.Core.Platform;
using ClockApp.Core.Timekeeping;
using Zenject;

namespace ClockApp.Infrastructure.Platform
{
    public sealed class SuspensionMonitor : ITickable
    {
        private const double CheckIntervalSeconds = 1d;

        private readonly IClock _clock;
        private readonly IRealtimeClock _realtimeClock;
        private double _nextCheckRealtime = double.MinValue;

        public SuspensionMonitor(IClock clock, IRealtimeClock realtimeClock)
        {
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _realtimeClock = realtimeClock ?? throw new ArgumentNullException(nameof(realtimeClock));
        }

        public void Tick()
        {
            var realtime = _realtimeClock.SecondsSinceStartup;

            if (realtime < _nextCheckRealtime)
            {
                return;
            }

            _nextCheckRealtime = realtime + CheckIntervalSeconds;
            _clock.CompensateSuspension();
        }
    }
}
