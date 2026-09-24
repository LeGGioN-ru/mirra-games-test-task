using System;
using ClockApp.Core.Editing;
using ClockApp.Core.Timekeeping;
using NUnit.Framework;

namespace ClockApp.Tests.Editing
{
    public sealed class HandPickerTests
    {
        private const float HourHandReach = 0.55f;

        private static readonly ClockHandAngles s_quarterToFour = ClockHandAngles.FromTimeOfDay(new TimeSpan(3, 45, 0));

        [Test]
        public void PicksMinuteHandBeyondHourHandReach()
        {
            Assert.AreEqual(ClockHand.Minute, HandPicker.Pick(112f, 0.8f, s_quarterToFour, HourHandReach));
        }

        [Test]
        public void PicksClosestHandWithinReach()
        {
            Assert.AreEqual(ClockHand.Hour, HandPicker.Pick(110f, 0.4f, s_quarterToFour, HourHandReach));
            Assert.AreEqual(ClockHand.Minute, HandPicker.Pick(265f, 0.4f, s_quarterToFour, HourHandReach));
        }

        [Test]
        public void MeasuresDistanceAcrossTwelve()
        {
            var angles = ClockHandAngles.FromTimeOfDay(new TimeSpan(11, 55, 0));

            Assert.AreEqual(ClockHand.Hour, HandPicker.Pick(5f, 0.3f, angles, HourHandReach));
        }
    }
}
