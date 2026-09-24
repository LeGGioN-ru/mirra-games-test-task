using System;
using ClockApp.Core.Timekeeping;
using NUnit.Framework;

namespace ClockApp.Tests.Timekeeping
{
    public sealed class SecondHandMotionTests
    {
        private const float Tolerance = 1e-3f;
        private const float TickDuration = 0.2f;

        private static readonly Func<float, float> s_linear = progress => progress;

        [Test]
        public void TickHoldsWholeSecondAfterTick()
        {
            Assert.AreEqual(90f, SecondHandMotion.TickAngle(15.5d, TickDuration, s_linear), Tolerance);
        }

        [Test]
        public void TickEasesFromPreviousSecond()
        {
            Assert.AreEqual(87f, SecondHandMotion.TickAngle(15.1d, TickDuration, s_linear), Tolerance);
        }

        [Test]
        public void TickStartsExactlyAtWholeSecond()
        {
            Assert.AreEqual(84f, SecondHandMotion.TickAngle(14.999d, TickDuration, s_linear), Tolerance);
            Assert.AreEqual(84f, SecondHandMotion.TickAngle(15d, TickDuration, s_linear), Tolerance);
        }

        [Test]
        public void TickAcrossMinuteStaysWithinTurn()
        {
            Assert.AreEqual(357f, SecondHandMotion.TickAngle(120.1d, TickDuration, s_linear), Tolerance);
        }

        [Test]
        public void TickSupportsOvershootingEase()
        {
            Assert.AreEqual(181.2f, SecondHandMotion.TickAngle(30.05d, TickDuration, _ => 1.2f), Tolerance);
        }

        [Test]
        public void ZeroTickDurationSnapsToWholeSecond()
        {
            Assert.AreEqual(90f, SecondHandMotion.TickAngle(15.01d, 0f, s_linear), Tolerance);
        }

        [Test]
        public void TickRequiresEase()
        {
            Assert.Throws<ArgumentNullException>(() => SecondHandMotion.TickAngle(1d, TickDuration, null));
        }
    }
}
