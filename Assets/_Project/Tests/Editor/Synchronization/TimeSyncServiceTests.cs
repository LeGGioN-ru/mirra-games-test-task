using System;
using System.Collections;
using System.Threading;
using ClockApp.Core.Synchronization;
using ClockApp.Tests.Fakes;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace ClockApp.Tests.Synchronization
{
    public sealed class TimeSyncServiceTests
    {
        private static readonly DateTime s_serverTime = new DateTime(2026, 9, 24, 10, 0, 0, DateTimeKind.Utc);

        private FakeRealtimeClock _realtimeClock;

        [SetUp]
        public void SetUp()
        {
            _realtimeClock = new FakeRealtimeClock(100d);
        }

        [UnityTest]
        public IEnumerator AnchorsServerTimeAtResponseMoment() => UniTask.ToCoroutine(async () =>
        {
            var source = new FakeTimeSource("Primary", true, _realtimeClock, FakeTimeResponse.Success(s_serverTime, 0.4d));
            var service = CreateService(source);

            var result = await service.SynchronizeAsync(CancellationToken.None);

            Assert.AreEqual(1, source.CallCount);
            Assert.AreEqual("Primary", result.SourceName);
            Assert.IsTrue(result.IsTrusted);
            Assert.AreEqual(TimeSpan.FromSeconds(0.4d), result.RoundTrip);
            Assert.AreEqual(100.4d, result.AnchorRealtime, 1e-9d);
            Assert.AreEqual(s_serverTime, result.UtcAtAnchor);
            Assert.AreEqual(DateTimeKind.Utc, result.UtcAtAnchor.Kind);
            Assert.IsEmpty(result.Failures);
        });

        [UnityTest]
        public IEnumerator ConvertsLocalServerTimeToUtc() => UniTask.ToCoroutine(async () =>
        {
            var source = new FakeTimeSource("Primary", true, _realtimeClock, FakeTimeResponse.Success(s_serverTime.ToLocalTime(), 0.2d));
            var service = CreateService(source);

            var result = await service.SynchronizeAsync(CancellationToken.None);

            Assert.AreEqual(s_serverTime, result.UtcAtAnchor);
            Assert.AreEqual(DateTimeKind.Utc, result.UtcAtAnchor.Kind);
        });

        [UnityTest]
        public IEnumerator FallsBackToNextSourceWhenSourceFails() => UniTask.ToCoroutine(async () =>
        {
            var failing = new FakeTimeSource("Failing", true, _realtimeClock, FakeTimeResponse.Failure(new InvalidOperationException("CORS")));
            var backup = new FakeTimeSource("Backup", true, _realtimeClock, FakeTimeResponse.Success(s_serverTime, 0.2d));
            var service = CreateService(failing, backup);

            var result = await service.SynchronizeAsync(CancellationToken.None);

            Assert.AreEqual(1, failing.CallCount);
            Assert.AreEqual(1, backup.CallCount);
            Assert.AreEqual("Backup", result.SourceName);
            Assert.AreEqual(1, result.Failures.Count);
            Assert.AreEqual("Failing: CORS", result.Failures[0].ToString());
        });

        [UnityTest]
        public IEnumerator ReportsUntrustedSource() => UniTask.ToCoroutine(async () =>
        {
            var device = new FakeTimeSource("Device", false, _realtimeClock, FakeTimeResponse.Success(s_serverTime, 0d));
            var service = CreateService(device);

            var result = await service.SynchronizeAsync(CancellationToken.None);

            Assert.IsFalse(result.IsTrusted);
            Assert.AreEqual(s_serverTime, result.UtcAtAnchor);
        });

        [UnityTest]
        public IEnumerator ThrowsWhenEverySourceFails() => UniTask.ToCoroutine(async () =>
        {
            var first = new FakeTimeSource("First", true, _realtimeClock, FakeTimeResponse.Failure(new InvalidOperationException("A")));
            var second = new FakeTimeSource("Second", true, _realtimeClock, FakeTimeResponse.Failure(new InvalidOperationException("B")));
            var service = CreateService(first, second);

            try
            {
                await service.SynchronizeAsync(CancellationToken.None);
                Assert.Fail("Expected TimeSyncException.");
            }
            catch (TimeSyncException exception)
            {
                Assert.AreEqual(2, exception.Failures.Count);
                Assert.AreEqual("First: A", exception.Failures[0].ToString());
                Assert.AreEqual("Second: B", exception.Failures[1].ToString());
                Assert.AreEqual("No time source responded: First: A; Second: B", exception.Message);
            }
        });

        [UnityTest]
        public IEnumerator PropagatesCallerCancellation() => UniTask.ToCoroutine(async () =>
        {
            var source = new FakeTimeSource("Primary", true, _realtimeClock, FakeTimeResponse.Success(s_serverTime, 0.1d));
            var service = CreateService(source);
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            try
            {
                await service.SynchronizeAsync(cancellation.Token);
                Assert.Fail("Expected OperationCanceledException.");
            }
            catch (OperationCanceledException)
            {
                Assert.AreEqual(0, source.CallCount);
            }
        });

        [UnityTest]
        public IEnumerator PropagatesCancellationReportedAsSourceError() => UniTask.ToCoroutine(async () =>
        {
            using var cancellation = new CancellationTokenSource();
            var aborted = new FakeTimeSource("Primary", true, _realtimeClock,
                FakeTimeResponse.FailureAfterCallerCancel(cancellation, new InvalidOperationException("Request aborted")));
            var backup = new FakeTimeSource("Backup", true, _realtimeClock, FakeTimeResponse.Success(s_serverTime, 0.2d));
            var service = CreateService(aborted, backup);

            try
            {
                await service.SynchronizeAsync(cancellation.Token);
                Assert.Fail("Expected OperationCanceledException.");
            }
            catch (OperationCanceledException)
            {
                Assert.AreEqual(0, backup.CallCount);
            }
        });

        [UnityTest]
        public IEnumerator TreatsSourceTimeoutAsFailure() => UniTask.ToCoroutine(async () =>
        {
            var timingOut = new FakeTimeSource("Slow", true, _realtimeClock, FakeTimeResponse.Failure(new OperationCanceledException(), 5d));
            var backup = new FakeTimeSource("Backup", true, _realtimeClock, FakeTimeResponse.Success(s_serverTime, 0.2d));
            var service = CreateService(timingOut, backup);

            var result = await service.SynchronizeAsync(CancellationToken.None);

            Assert.AreEqual(1, timingOut.CallCount);
            Assert.AreEqual("Backup", result.SourceName);
            Assert.AreEqual("Slow: Timed out", result.Failures[0].ToString());
            Assert.AreEqual(105.2d, result.AnchorRealtime, 1e-9d);
        });

        [Test]
        public void RequiresAtLeastOneSource()
        {
            Assert.Throws<ArgumentException>(() => new TimeSyncService(Array.Empty<ITimeSource>(), _realtimeClock));
        }

        private TimeSyncService CreateService(params ITimeSource[] sources)
        {
            return new TimeSyncService(sources, _realtimeClock);
        }
    }
}
