using System;
using System.Threading;
using ClockApp.Core.Platform;
using ClockApp.Core.Synchronization;
using Cysharp.Threading.Tasks;

namespace ClockApp.Infrastructure.TimeSources
{
    public sealed class DeviceTimeSource : ITimeSource
    {
        private readonly IWallClock _wallClock;

        public DeviceTimeSource(IWallClock wallClock)
        {
            _wallClock = wallClock ?? throw new ArgumentNullException(nameof(wallClock));
        }

        public string Name => "Device clock";

        public bool IsTrusted => false;

        public UniTask<DateTime> GetUtcTimeAsync(CancellationToken cancellationToken)
        {
            return UniTask.FromResult(_wallClock.UtcNow);
        }
    }
}
