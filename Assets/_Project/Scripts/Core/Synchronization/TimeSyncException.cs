using System;
using System.Collections.Generic;

namespace ClockApp.Core.Synchronization
{
    public sealed class TimeSyncException : Exception
    {
        public TimeSyncException(IReadOnlyList<TimeSourceFailure> failures)
            : base("No time source responded: " + string.Join("; ", failures))
        {
            Failures = failures;
        }

        public IReadOnlyList<TimeSourceFailure> Failures { get; }
    }
}
