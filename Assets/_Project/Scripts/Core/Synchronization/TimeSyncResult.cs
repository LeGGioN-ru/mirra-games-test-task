using System;
using System.Collections.Generic;
using ClockApp.Core.Platform;

namespace ClockApp.Core.Synchronization
{
    public sealed class TimeSyncResult
    {
        public TimeSyncResult(
            DateTime utcAtAnchor,
            double anchorRealtime,
            string sourceName,
            bool isTrusted,
            TimeSpan roundTrip,
            IReadOnlyList<TimeSourceFailure> failures)
        {
            UtcAtAnchor = UtcDateTime.From(utcAtAnchor);
            AnchorRealtime = anchorRealtime;
            SourceName = sourceName;
            IsTrusted = isTrusted;
            RoundTrip = roundTrip;
            Failures = failures ?? Array.Empty<TimeSourceFailure>();
        }

        public DateTime UtcAtAnchor { get; }

        public double AnchorRealtime { get; }

        public string SourceName { get; }

        public bool IsTrusted { get; }

        public TimeSpan RoundTrip { get; }

        public IReadOnlyList<TimeSourceFailure> Failures { get; }
    }
}
