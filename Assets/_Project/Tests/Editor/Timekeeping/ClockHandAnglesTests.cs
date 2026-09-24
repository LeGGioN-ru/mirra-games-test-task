using System;
using ClockApp.Core.Timekeeping;
using NUnit.Framework;

namespace ClockApp.Tests.Timekeeping
{
    public sealed class ClockHandAnglesTests
    {
        private const float Tolerance = 1e-3f;

        [TestCase(0, 0, 0, 0f, 0f, 0f)]
        [TestCase(3, 0, 0, 90f, 0f, 0f)]
        [TestCase(12, 0, 0, 0f, 0f, 0f)]
        [TestCase(15, 30, 0, 105f, 180f, 0f)]
        [TestCase(23, 59, 30, 359.75f, 357f, 180f)]
        [TestCase(6, 15, 45, 187.875f, 94.5f, 270f)]
        public void ConvertsTimeOfDayToAngles(int hours, int minutes, int seconds, float hour, float minute, float second)
        {
            var angles = ClockHandAngles.FromTimeOfDay(new TimeSpan(hours, minutes, seconds));

            Assert.AreEqual(hour, angles.Hour, Tolerance);
            Assert.AreEqual(minute, angles.Minute, Tolerance);
            Assert.AreEqual(second, angles.Second, Tolerance);
        }

        [Test]
        public void KeepsFractionalSeconds()
        {
            var angles = ClockHandAngles.FromSecondsOfDay(15.5d);

            Assert.AreEqual(93f, angles.Second, Tolerance);
            Assert.AreEqual(1.55f, angles.Minute, Tolerance);
            Assert.AreEqual(0.1291667f, angles.Hour, Tolerance);
        }

        [Test]
        public void WrapsNegativeTimeToPreviousDay()
        {
            var angles = ClockHandAngles.FromSecondsOfDay(-1d);

            Assert.AreEqual(354f, angles.Second, Tolerance);
            Assert.AreEqual(359.9f, angles.Minute, Tolerance);
            Assert.AreEqual(359.99167f, angles.Hour, Tolerance);
        }

        [Test]
        public void WrapsTimeBeyondOneDay()
        {
            var angles = ClockHandAngles.FromSecondsOfDay(86400d + 3600d);

            Assert.AreEqual(30f, angles.Hour, Tolerance);
            Assert.AreEqual(0f, angles.Minute, Tolerance);
        }
    }
}
