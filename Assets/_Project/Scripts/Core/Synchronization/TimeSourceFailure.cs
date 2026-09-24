namespace ClockApp.Core.Synchronization
{
    public sealed class TimeSourceFailure
    {
        public TimeSourceFailure(string sourceName, string reason)
        {
            SourceName = sourceName;
            Reason = reason;
        }

        public string SourceName { get; }

        public string Reason { get; }

        public override string ToString()
        {
            return SourceName + ": " + Reason;
        }
    }
}
