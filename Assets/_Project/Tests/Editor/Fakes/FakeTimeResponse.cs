using System;
using System.Threading;

namespace ClockApp.Tests.Fakes
{
    internal readonly struct FakeTimeResponse
    {
        private FakeTimeResponse(DateTime serverUtc, double roundTripSeconds, Exception error, CancellationTokenSource callerCancellation)
        {
            ServerUtc = serverUtc;
            RoundTripSeconds = roundTripSeconds;
            Error = error;
            CallerCancellation = callerCancellation;
        }

        public DateTime ServerUtc { get; }

        public double RoundTripSeconds { get; }

        public Exception Error { get; }

        public CancellationTokenSource CallerCancellation { get; }

        public static FakeTimeResponse Success(DateTime serverUtc, double roundTripSeconds)
        {
            return new FakeTimeResponse(serverUtc, roundTripSeconds, null, null);
        }

        public static FakeTimeResponse Failure(Exception error, double roundTripSeconds = 0d)
        {
            return new FakeTimeResponse(default, roundTripSeconds, error, null);
        }

        public static FakeTimeResponse FailureAfterCallerCancel(CancellationTokenSource callerCancellation, Exception error)
        {
            return new FakeTimeResponse(default, 0d, error, callerCancellation);
        }
    }
}
