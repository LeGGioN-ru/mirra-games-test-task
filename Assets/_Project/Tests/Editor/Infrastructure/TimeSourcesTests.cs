using System;
using System.Collections;
using System.Threading;
using ClockApp.Core.Synchronization;
using ClockApp.Infrastructure.TimeSources;
using ClockApp.Tests.Fakes;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace ClockApp.Tests.Infrastructure
{
    public sealed class TimeSourcesTests
    {
        private const string TimeApiBody =
            "{\"year\":2026,\"month\":9,\"day\":24,\"hour\":10,\"minute\":28,\"seconds\":0,\"milliSeconds\":809," +
            "\"dateTime\":\"2026-09-24T10:28:00.8091529\",\"date\":\"09/24/2026\",\"time\":\"10:28\",\"timeZone\":\"UTC\"," +
            "\"dayOfWeek\":\"Thursday\",\"dstActive\":false}";

        [UnityTest]
        public IEnumerator YandexReadsUnixMilliseconds() => UniTask.ToCoroutine(async () =>
        {
            var httpClient = FakeHttpClient.WithBody("{\"time\":1790245679070,\"clocks\":{}}");
            var source = new YandexTimeSource(httpClient);

            var utc = await source.GetUtcTimeAsync(CancellationToken.None);

            Assert.AreEqual(new DateTime(2026, 9, 24, 10, 27, 59, 70, DateTimeKind.Utc), utc);
            Assert.AreEqual(DateTimeKind.Utc, utc.Kind);
            Assert.AreEqual("GET", httpClient.LastMethod);
            Assert.AreEqual("https://yandex.com/time/sync.json", httpClient.LastUrl);
        });

        [UnityTest]
        public IEnumerator TimeApiReadsDateFields() => UniTask.ToCoroutine(async () =>
        {
            var httpClient = FakeHttpClient.WithBody(TimeApiBody);
            var source = new TimeApiTimeSource(httpClient);

            var utc = await source.GetUtcTimeAsync(CancellationToken.None);

            Assert.AreEqual(new DateTime(2026, 9, 24, 10, 28, 0, 809, DateTimeKind.Utc), utc);
            Assert.AreEqual(DateTimeKind.Utc, utc.Kind);
            StringAssert.Contains("timeZone=UTC", httpClient.LastUrl);
        });

        [UnityTest]
        public IEnumerator AkamaiReadsUnixSecondsWithMilliseconds() => UniTask.ToCoroutine(async () =>
        {
            var source = new AkamaiTimeSource(FakeHttpClient.WithBody("1790245866.977\n"));

            var utc = await source.GetUtcTimeAsync(CancellationToken.None);

            Assert.AreEqual(new DateTime(2026, 9, 24, 10, 31, 6, 977, DateTimeKind.Utc), utc);
        });

        [UnityTest]
        public IEnumerator PageDateHeaderCompensatesSecondResolution() => UniTask.ToCoroutine(async () =>
        {
            var httpClient = FakeHttpClient.WithHeader("date", "Thu, 24 Sep 2026 10:38:59 GMT");
            var source = new PageDateHeaderTimeSource(httpClient, "https://leggion-ru.github.io/mirra-games-test-task/");

            var utc = await source.GetUtcTimeAsync(CancellationToken.None);

            Assert.AreEqual(new DateTime(2026, 9, 24, 10, 38, 59, 500, DateTimeKind.Utc), utc);
            Assert.AreEqual(DateTimeKind.Utc, utc.Kind);
            Assert.AreEqual("HEAD", httpClient.LastMethod);
            StringAssert.StartsWith("https://leggion-ru.github.io/mirra-games-test-task/?nocache=", httpClient.LastUrl);
            Assert.AreEqual("leggion-ru.github.io", source.Name);
        });

        [UnityTest]
        public IEnumerator YandexRejectsUnexpectedBody() => UniTask.ToCoroutine(async () =>
        {
            await AssertFormatExceptionAsync(new YandexTimeSource(FakeHttpClient.WithBody("{\"clocks\":{}}")));
        });

        [UnityTest]
        public IEnumerator TimeApiRejectsUnexpectedBody() => UniTask.ToCoroutine(async () =>
        {
            await AssertFormatExceptionAsync(new TimeApiTimeSource(FakeHttpClient.WithBody("{}")));
        });

        [UnityTest]
        public IEnumerator AkamaiRejectsUnexpectedBody() => UniTask.ToCoroutine(async () =>
        {
            await AssertFormatExceptionAsync(new AkamaiTimeSource(FakeHttpClient.WithBody("<html>")));
        });

        [UnityTest]
        public IEnumerator PageDateHeaderRejectsMissingHeader() => UniTask.ToCoroutine(async () =>
        {
            await AssertFormatExceptionAsync(new PageDateHeaderTimeSource(FakeHttpClient.WithBody(string.Empty), "https://example.com/"));
        });

        [UnityTest]
        public IEnumerator PageDateHeaderRejectsMalformedHeader() => UniTask.ToCoroutine(async () =>
        {
            await AssertFormatExceptionAsync(new PageDateHeaderTimeSource(FakeHttpClient.WithHeader("Date", "yesterday"), "https://example.com/"));
        });

        [Test]
        public void PageDateHeaderRequiresAbsoluteUrl()
        {
            Assert.Throws<ArgumentException>(() => new PageDateHeaderTimeSource(FakeHttpClient.WithBody(string.Empty), string.Empty));
        }

        [UnityTest]
        public IEnumerator DeviceSourceReturnsWallClockAndIsNotTrusted() => UniTask.ToCoroutine(async () =>
        {
            var deviceTime = new DateTime(2026, 9, 24, 9, 58, 0, DateTimeKind.Utc);
            var source = new DeviceTimeSource(new FakeWallClock(deviceTime));

            var utc = await source.GetUtcTimeAsync(CancellationToken.None);

            Assert.AreEqual(deviceTime, utc);
            Assert.IsFalse(source.IsTrusted);
        });

        [UnityTest]
        public IEnumerator HttpErrorsPropagateToCaller() => UniTask.ToCoroutine(async () =>
        {
            var source = new TimeApiTimeSource(FakeHttpClient.WithError(new InvalidOperationException("Cannot connect")));

            try
            {
                await source.GetUtcTimeAsync(CancellationToken.None);
                Assert.Fail("Expected InvalidOperationException.");
            }
            catch (InvalidOperationException exception)
            {
                Assert.AreEqual("Cannot connect", exception.Message);
            }
        });

        [Test]
        public void BrowserChainAvoidsYandexAndEndsWithDeviceClock()
        {
            var chain = TimeSourceChain.Create(FakeHttpClient.WithBody(string.Empty), new FakeWallClock(DateTime.UtcNow), true, "https://example.com/");

            Assert.AreEqual(4, chain.Count);
            Assert.IsInstanceOf<TimeApiTimeSource>(chain[0]);
            Assert.IsInstanceOf<AkamaiTimeSource>(chain[1]);
            Assert.IsInstanceOf<PageDateHeaderTimeSource>(chain[2]);
            Assert.IsInstanceOf<DeviceTimeSource>(chain[3]);
        }

        [Test]
        public void NativeChainStartsWithYandexAndEndsWithDeviceClock()
        {
            var chain = TimeSourceChain.Create(FakeHttpClient.WithBody(string.Empty), new FakeWallClock(DateTime.UtcNow), false, string.Empty);

            Assert.AreEqual(4, chain.Count);
            Assert.IsInstanceOf<YandexTimeSource>(chain[0]);
            Assert.IsInstanceOf<TimeApiTimeSource>(chain[1]);
            Assert.IsInstanceOf<AkamaiTimeSource>(chain[2]);
            Assert.IsInstanceOf<DeviceTimeSource>(chain[3]);
        }

        private static async UniTask AssertFormatExceptionAsync(ITimeSource source)
        {
            try
            {
                await source.GetUtcTimeAsync(CancellationToken.None);
                Assert.Fail("Expected FormatException.");
            }
            catch (FormatException)
            {
            }
        }
    }
}
