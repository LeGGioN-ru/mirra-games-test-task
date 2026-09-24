using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ClockApp.Core.Platform;
using Cysharp.Threading.Tasks;

namespace ClockApp.Core.Synchronization
{
    public sealed class TimeSyncService : ITimeSyncService
    {
        private readonly IReadOnlyList<ITimeSource> _sources;
        private readonly IRealtimeClock _realtimeClock;

        public TimeSyncService(IEnumerable<ITimeSource> sources, IRealtimeClock realtimeClock)
        {
            _sources = sources?.ToArray() ?? throw new ArgumentNullException(nameof(sources));
            _realtimeClock = realtimeClock ?? throw new ArgumentNullException(nameof(realtimeClock));

            if (_sources.Count == 0)
            {
                throw new ArgumentException("At least one time source is required.", nameof(sources));
            }
        }

        public async UniTask<TimeSyncResult> SynchronizeAsync(CancellationToken cancellationToken)
        {
            var failures = new List<TimeSourceFailure>();

            foreach (var source in _sources)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var requestStarted = _realtimeClock.SecondsSinceStartup;

                try
                {
                    var serverUtc = await source.GetUtcTimeAsync(cancellationToken);
                    var responseReceived = _realtimeClock.SecondsSinceStartup;
                    var roundTrip = TimeSpan.FromTicks(ToTicks(Math.Max(0d, responseReceived - requestStarted)));

                    return new TimeSyncResult(serverUtc, responseReceived, source.Name, source.IsTrusted, roundTrip, failures);
                }
                catch (Exception exception)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    failures.Add(new TimeSourceFailure(source.Name, DescribeFailure(exception)));
                }
            }

            throw new TimeSyncException(failures);
        }

        private static string DescribeFailure(Exception exception)
        {
            return exception is OperationCanceledException ? "Timed out" : exception.Message;
        }

        private static long ToTicks(double seconds)
        {
            return (long)Math.Round(seconds * TimeSpan.TicksPerSecond);
        }
    }
}
