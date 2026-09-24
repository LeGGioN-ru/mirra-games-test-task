namespace ClockApp.Core.Platform
{
    public interface IRealtimeClock
    {
        double SecondsSinceStartup { get; }
    }
}
