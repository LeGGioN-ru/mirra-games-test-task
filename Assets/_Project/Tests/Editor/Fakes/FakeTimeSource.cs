using System;
using System.Collections.Generic;
using System.Threading;
using ClockApp.Core.Synchronization;
using Cysharp.Threading.Tasks;

namespace ClockApp.Tests.Fakes
{
    internal sealed class FakeTimeSource : ITimeSource
    {
        private readonly FakeRealtimeClock _realtimeClock;
        private readonly IReadOnlyList<FakeTimeResponse> _responses;

        public FakeTimeSource(string name, bool isTrusted, FakeRealtimeClock realtimeClock, params FakeTimeResponse[] responses)
        {
            Name = name;
            IsTrusted = isTrusted;
            _realtimeClock = realtimeClock;
            _responses = responses;
        }

        public string Name { get; }

        public bool IsTrusted { get; }

        public int CallCount { get; private set; }

        public UniTask<DateTime> GetUtcTimeAsync(CancellationToken cancellationToken)
        {
            var response = _responses[Math.Min(CallCount, _responses.Count - 1)];
            CallCount++;
            _realtimeClock.Advance(response.RoundTripSeconds);
            response.CallerCancellation?.Cancel();

            return response.Error != null
                ? UniTask.FromException<DateTime>(response.Error)
                : UniTask.FromResult(response.ServerUtc);
        }
    }
}
