using System;
using ClockApp.Core.Editing;
using NUnit.Framework;

namespace ClockApp.Tests.Editing
{
    public sealed class HandDragSessionTests
    {
        [Test]
        public void MinuteHandCrossingTwelveForwardAdvancesHour()
        {
            var session = new HandDragSession(ClockHand.Minute, new TimeSpan(10, 55, 0), 330f);

            MoveThrough(session, 340f, 350f, 0f, 10f, 20f, 30f);

            Assert.AreEqual(new TimeSpan(11, 5, 0), session.TimeOfDay);
        }

        [Test]
        public void MinuteHandCrossingTwelveBackwardRewindsHour()
        {
            var session = new HandDragSession(ClockHand.Minute, new TimeSpan(11, 5, 0), 30f);

            MoveThrough(session, 0f, 330f);

            Assert.AreEqual(new TimeSpan(10, 55, 0), session.TimeOfDay);
        }

        [Test]
        public void HourHandFullTurnSwitchesHalfOfDay()
        {
            var session = new HandDragSession(ClockHand.Hour, new TimeSpan(9, 0, 0), 270f);

            MoveThrough(session, 0f, 90f, 180f, 270f);

            Assert.AreEqual(new TimeSpan(21, 0, 0), session.TimeOfDay);
        }

        [Test]
        public void HourHandMovesByTwoMinutesPerDegree()
        {
            var session = new HandDragSession(ClockHand.Hour, new TimeSpan(3, 0, 0), 90f);

            session.MoveTo(105f);

            Assert.AreEqual(new TimeSpan(3, 30, 0), session.TimeOfDay);
        }

        [Test]
        public void WrapsAroundMidnight()
        {
            var session = new HandDragSession(ClockHand.Minute, new TimeSpan(23, 50, 0), 300f);

            MoveThrough(session, 330f, 0f, 30f);

            Assert.AreEqual(new TimeSpan(0, 5, 0), session.TimeOfDay);
        }

        [Test]
        public void WrapsBackwardAcrossMidnight()
        {
            var session = new HandDragSession(ClockHand.Minute, new TimeSpan(0, 5, 0), 30f);

            MoveThrough(session, 0f, 330f);

            Assert.AreEqual(new TimeSpan(23, 55, 0), session.TimeOfDay);
        }

        [Test]
        public void SnapsToWholeMinutes()
        {
            var session = new HandDragSession(ClockHand.Minute, new TimeSpan(10, 0, 0), 0f);

            var changedByTwoDegrees = session.MoveTo(2f);
            var changedByFourDegrees = session.MoveTo(4f);

            Assert.IsFalse(changedByTwoDegrees);
            Assert.IsTrue(changedByFourDegrees);
            Assert.AreEqual(new TimeSpan(10, 1, 0), session.TimeOfDay);
        }

        [Test]
        public void DropsSecondsOfStartTime()
        {
            var session = new HandDragSession(ClockHand.Minute, new TimeSpan(10, 15, 40), 94f);

            Assert.AreEqual(new TimeSpan(10, 15, 0), session.TimeOfDay);
        }

        [Test]
        public void RebaseIgnoresPointerJump()
        {
            var session = new HandDragSession(ClockHand.Minute, new TimeSpan(10, 0, 0), 0f);

            session.Rebase(180f);
            session.MoveTo(186f);

            Assert.AreEqual(new TimeSpan(10, 1, 0), session.TimeOfDay);
        }

        private static void MoveThrough(HandDragSession session, params float[] angles)
        {
            foreach (var angle in angles)
            {
                session.MoveTo(angle);
            }
        }
    }
}
