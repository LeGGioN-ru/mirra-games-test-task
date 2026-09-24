using ClockApp.Core.Platform;

namespace ClockApp.Tests.Fakes
{
    internal sealed class FakeRealtimeClock : IRealtimeClock
    {
        public FakeRealtimeClock(double secondsSinceStartup = 0d)
        {
            SecondsSinceStartup = secondsSinceStartup;
        }

        public double SecondsSinceStartup { get; set; }

        public void Advance(double seconds)
        {
            SecondsSinceStartup += seconds;
        }
    }
}
