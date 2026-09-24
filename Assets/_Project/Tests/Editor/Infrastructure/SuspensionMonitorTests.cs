using System;
using ClockApp.Core.Timekeeping;
using ClockApp.Infrastructure.Platform;
using ClockApp.Tests.Fakes;
using NUnit.Framework;

namespace ClockApp.Tests.Infrastructure
{
    public sealed class SuspensionMonitorTests
    {
        private static readonly DateTime s_startTime = new DateTime(2026, 9, 24, 10, 0, 0, DateTimeKind.Utc);

        private FakeRealtimeClock _realtimeClock;
        private FakeWallClock _wallClock;
        private SyncedClock _clock;
        private SuspensionMonitor _monitor;

        [SetUp]
        public void SetUp()
        {
            _realtimeClock = new FakeRealtimeClock(10d);
            _wallClock = new FakeWallClock(s_startTime);
            _clock = new SyncedClock(_realtimeClock, _wallClock, new FakeUtcOffsetProvider(TimeSpan.Zero));
            _monitor = new SuspensionMonitor(_clock, _realtimeClock);
        }

        [Test]
        public void CompensatesSleepOnNextCheck()
        {
            _monitor.Tick();
            _realtimeClock.Advance(1d);
            _wallClock.Advance(1801d);

            _monitor.Tick();

            Assert.AreEqual(s_startTime.AddSeconds(1801d), _clock.UtcNow);
        }

        [Test]
        public void ChecksAtMostOncePerSecond()
        {
            _monitor.Tick();
            _realtimeClock.Advance(0.5d);
            _wallClock.Advance(600d);

            _monitor.Tick();

            Assert.AreEqual(s_startTime.AddSeconds(0.5d), _clock.UtcNow);

            _realtimeClock.Advance(0.5d);
            _monitor.Tick();

            Assert.AreEqual(s_startTime.AddSeconds(600d), _clock.UtcNow);
        }
    }
}
