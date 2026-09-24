using System;
using ClockApp.Infrastructure.Platform;
using NUnit.Framework;

namespace ClockApp.Tests.Infrastructure
{
    public sealed class LocalUtcOffsetProviderTests
    {
        [TestCase(2026, 1, 15)]
        [TestCase(2026, 7, 15)]
        public void MatchesSystemTimeZoneInEditor(int year, int month, int day)
        {
            var provider = new LocalUtcOffsetProvider();
            var utc = new DateTime(year, month, day, 12, 0, 0, DateTimeKind.Utc);

            Assert.AreEqual(TimeZoneInfo.Local.GetUtcOffset(utc), provider.GetUtcOffset(utc));
        }

        [Test]
        public void TreatsLocalKindAsTheSameInstant()
        {
            var provider = new LocalUtcOffsetProvider();
            var utc = new DateTime(2026, 7, 15, 12, 0, 0, DateTimeKind.Utc);

            Assert.AreEqual(TimeZoneInfo.Local.GetUtcOffset(utc), provider.GetUtcOffset(utc.ToLocalTime()));
        }
    }
}
