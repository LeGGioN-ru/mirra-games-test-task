using ClockApp.Core.Editing;
using NUnit.Framework;

namespace ClockApp.Tests.Editing
{
    public sealed class DialMathTests
    {
        private const float Tolerance = 1e-3f;

        [TestCase(0f, 1f, 0f)]
        [TestCase(1f, 0f, 90f)]
        [TestCase(0f, -1f, 180f)]
        [TestCase(-1f, 0f, 270f)]
        [TestCase(1f, 1f, 45f)]
        [TestCase(-1f, 1f, 315f)]
        public void PointerAngleIsClockwiseFromTwelve(float x, float y, float expected)
        {
            Assert.AreEqual(expected, DialMath.PointerAngle(x, y), Tolerance);
        }

        [TestCase(350f, 10f, 20f)]
        [TestCase(10f, 350f, -20f)]
        [TestCase(90f, 90f, 0f)]
        [TestCase(0f, 270f, -90f)]
        [TestCase(0f, 180f, 180f)]
        [TestCase(720f, 30f, 30f)]
        public void DeltaAngleTakesShortestPath(float from, float to, float expected)
        {
            Assert.AreEqual(expected, DialMath.DeltaAngle(from, to), Tolerance);
        }
    }
}
