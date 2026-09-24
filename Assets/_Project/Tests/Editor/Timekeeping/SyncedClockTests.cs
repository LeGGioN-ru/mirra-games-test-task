using System;
using ClockApp.Core.Synchronization;
using ClockApp.Core.Timekeeping;
using ClockApp.Tests.Fakes;
using NUnit.Framework;

namespace ClockApp.Tests.Timekeeping
{
    public sealed class SyncedClockTests
    {
        private static readonly DateTime s_deviceTime = new DateTime(2026, 9, 24, 9, 58, 0, DateTimeKind.Utc);
        private static readonly DateTime s_serverTime = new DateTime(2026, 9, 24, 10, 0, 0, DateTimeKind.Utc);

        private FakeRealtimeClock _realtimeClock;
        private FakeWallClock _wallClock;
        private FakeUtcOffsetProvider _utcOffsetProvider;
        private SyncedClock _clock;
        private int _adjustedCount;

        [SetUp]
        public void SetUp()
        {
            _realtimeClock = new FakeRealtimeClock(50d);
            _wallClock = new FakeWallClock(s_deviceTime);
            _utcOffsetProvider = new FakeUtcOffsetProvider(TimeSpan.FromHours(3d));
            _clock = CreateClock(_utcOffsetProvider);
        }

        [Test]
        public void FollowsDeviceClockBeforeSync()
        {
            Advance(5d);

            Assert.AreEqual(s_deviceTime.AddSeconds(5d), _clock.UtcNow);
        }

        [Test]
        public void ConvertsLocalDeviceTimeToUtc()
        {
            var clock = new SyncedClock(_realtimeClock, new FakeWallClock(s_deviceTime.ToLocalTime()), _utcOffsetProvider);

            Assert.AreEqual(s_deviceTime, clock.UtcNow);
            Assert.AreEqual(DateTimeKind.Utc, clock.UtcNow.Kind);
        }

        [Test]
        public void CountsFromSyncAnchorUsingRealtime()
        {
            _clock.ApplySync(CreateSync(s_serverTime, 50d));

            Advance(5.5d);

            Assert.AreEqual(s_serverTime.AddSeconds(5.5d), _clock.UtcNow);
            Assert.AreEqual(DateTimeKind.Utc, _clock.UtcNow.Kind);
            Assert.AreEqual(1, _adjustedCount);
        }

        [Test]
        public void AccountsForTimePassedBetweenMeasurementAndApply()
        {
            Advance(2d);

            _clock.ApplySync(CreateSync(s_serverTime, 50d));

            Assert.AreEqual(s_serverTime.AddSeconds(2d), _clock.UtcNow);
        }

        [Test]
        public void AppliesUtcOffsetToLocalTime()
        {
            _clock.ApplySync(CreateSync(s_serverTime, 50d));

            Assert.AreEqual(new DateTime(2026, 9, 24, 13, 0, 0), _clock.LocalNow);
            Assert.AreEqual(DateTimeKind.Unspecified, _clock.LocalNow.Kind);
        }

        [Test]
        public void LocalTimeFollowsUtcOffsetChange()
        {
            var transitionUtc = s_serverTime.AddSeconds(30d);
            var clock = CreateClock(new FakeUtcOffsetProvider(TimeSpan.FromHours(3d), transitionUtc, TimeSpan.FromHours(2d)));
            clock.ApplySync(CreateSync(s_serverTime, 50d));

            Advance(60d);

            Assert.AreEqual(new DateTime(2026, 9, 24, 12, 1, 0), clock.LocalNow);
            Assert.AreEqual(s_serverTime.AddSeconds(60d), clock.UtcNow);
        }

        [Test]
        public void ManualTimeKeepsRunning()
        {
            _clock.ApplySync(CreateSync(s_serverTime, 50d));

            _clock.SetLocalTimeOfDay(new TimeSpan(7, 30, 0));
            Advance(90d);

            Assert.IsTrue(_clock.IsManuallyAdjusted);
            Assert.AreEqual(new TimeSpan(7, 31, 30), _clock.LocalNow.TimeOfDay);
            Assert.AreEqual(s_serverTime.AddSeconds(90d), _clock.UtcNow);
            Assert.AreEqual(2, _adjustedCount);
        }

        [Test]
        public void SecondManualTimeReplacesFirst()
        {
            _clock.ApplySync(CreateSync(s_serverTime, 50d));
            _clock.SetLocalTimeOfDay(new TimeSpan(7, 30, 0));
            Advance(10d);

            _clock.SetLocalTimeOfDay(new TimeSpan(8, 0, 0));
            Advance(5d);

            Assert.AreEqual(new TimeSpan(8, 0, 5), _clock.LocalNow.TimeOfDay);
            Assert.AreEqual(s_serverTime.AddSeconds(15d), _clock.UtcNow);
            Assert.AreEqual(3, _adjustedCount);
        }

        [Test]
        public void ResetReturnsToSyncedTime()
        {
            _clock.ApplySync(CreateSync(s_serverTime, 50d));
            _clock.SetLocalTimeOfDay(new TimeSpan(23, 15, 0));

            _clock.ResetToSyncedTime();

            Assert.IsFalse(_clock.IsManuallyAdjusted);
            Assert.AreEqual(new TimeSpan(13, 0, 0), _clock.LocalNow.TimeOfDay);
            Assert.AreEqual(3, _adjustedCount);
        }

        [Test]
        public void ResetWithoutManualTimeDoesNothing()
        {
            _clock.ApplySync(CreateSync(s_serverTime, 50d));

            _clock.ResetToSyncedTime();

            Assert.AreEqual(1, _adjustedCount);
        }

        [Test]
        public void LateSyncDiscardsManualTime()
        {
            _clock.SetLocalTimeOfDay(new TimeSpan(1, 0, 0));

            _clock.ApplySync(CreateSync(s_serverTime, 50d));

            Assert.IsFalse(_clock.IsManuallyAdjusted);
            Assert.AreEqual(new TimeSpan(13, 0, 0), _clock.LocalNow.TimeOfDay);
        }

        [TestCase(-1d)]
        [TestCase(86400d)]
        public void RejectsTimeOfDayOutsideOfDay(double seconds)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _clock.SetLocalTimeOfDay(TimeSpan.FromSeconds(seconds)));
        }

        [Test]
        public void CompensatesSuspensionWhenWallClockRanAhead()
        {
            _clock.ApplySync(CreateSync(s_serverTime, 50d));
            _realtimeClock.Advance(10d);
            _wallClock.Advance(10d + 3600d);

            var compensated = _clock.CompensateSuspension();

            Assert.IsTrue(compensated);
            Assert.AreEqual(s_serverTime.AddSeconds(3610d), _clock.UtcNow);
            Assert.AreEqual(2, _adjustedCount);
        }

        [Test]
        public void CompensationMeasuresWallClockFromSyncAnchor()
        {
            Advance(2d);
            _clock.ApplySync(CreateSync(s_serverTime, 50d));
            _realtimeClock.Advance(10d);
            _wallClock.Advance(3610d);

            var compensated = _clock.CompensateSuspension();

            Assert.IsTrue(compensated);
            Assert.AreEqual(s_serverTime.AddSeconds(3612d), _clock.UtcNow);
        }

        [Test]
        public void CompensationKeepsManualTime()
        {
            _clock.ApplySync(CreateSync(s_serverTime, 50d));
            _clock.SetLocalTimeOfDay(new TimeSpan(7, 30, 0));
            _realtimeClock.Advance(10d);
            _wallClock.Advance(3610d);

            var compensated = _clock.CompensateSuspension();

            Assert.IsTrue(compensated);
            Assert.IsTrue(_clock.IsManuallyAdjusted);
            Assert.AreEqual(new TimeSpan(8, 30, 10), _clock.LocalNow.TimeOfDay);
            Assert.AreEqual(3, _adjustedCount);
        }

        [Test]
        public void CompensationIsAppliedOnlyOnce()
        {
            _clock.ApplySync(CreateSync(s_serverTime, 50d));
            _wallClock.Advance(600d);
            _clock.CompensateSuspension();

            var compensatedAgain = _clock.CompensateSuspension();

            Assert.IsFalse(compensatedAgain);
            Assert.AreEqual(s_serverTime.AddSeconds(600d), _clock.UtcNow);
        }

        [TestCase(1d)]
        [TestCase(-3600d)]
        public void IgnoresSmallOrBackwardWallClockChanges(double wallClockShift)
        {
            _clock.ApplySync(CreateSync(s_serverTime, 50d));
            Advance(30d);
            _wallClock.Advance(wallClockShift);

            var compensated = _clock.CompensateSuspension();

            Assert.IsFalse(compensated);
            Assert.AreEqual(s_serverTime.AddSeconds(30d), _clock.UtcNow);
            Assert.AreEqual(1, _adjustedCount);
        }

        [Test]
        public void BackwardWallClockStepDoesNotHideLaterSuspension()
        {
            _clock.ApplySync(CreateSync(s_serverTime, 50d));
            Advance(100d);
            _wallClock.Advance(-3600d);
            _clock.CompensateSuspension();
            Advance(20d);
            _wallClock.Advance(1800d);

            var compensated = _clock.CompensateSuspension();

            Assert.IsTrue(compensated);
            Assert.AreEqual(s_serverTime.AddSeconds(1920d), _clock.UtcNow);
        }

        private SyncedClock CreateClock(FakeUtcOffsetProvider utcOffsetProvider)
        {
            var clock = new SyncedClock(_realtimeClock, _wallClock, utcOffsetProvider);
            _adjustedCount = 0;
            clock.Adjusted += () => _adjustedCount++;
            return clock;
        }

        private void Advance(double seconds)
        {
            _realtimeClock.Advance(seconds);
            _wallClock.Advance(seconds);
        }

        private static TimeSyncResult CreateSync(DateTime utc, double anchorRealtime)
        {
            return new TimeSyncResult(utc, anchorRealtime, "Test", true, TimeSpan.FromMilliseconds(100d), null);
        }
    }
}
