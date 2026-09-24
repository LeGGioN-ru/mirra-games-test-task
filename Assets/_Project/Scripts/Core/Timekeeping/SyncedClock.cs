using System;
using ClockApp.Core.Platform;
using ClockApp.Core.Synchronization;

namespace ClockApp.Core.Timekeeping
{
    public sealed class SyncedClock : IClock
    {
        private const double SuspensionThresholdSeconds = 2d;

        private readonly IRealtimeClock _realtimeClock;
        private readonly IWallClock _wallClock;
        private readonly IUtcOffsetProvider _utcOffsetProvider;

        private DateTime _anchorUtc;
        private double _anchorRealtime;
        private DateTime _anchorWallUtc;
        private TimeSpan _manualOffset;

        public SyncedClock(IRealtimeClock realtimeClock, IWallClock wallClock, IUtcOffsetProvider utcOffsetProvider)
        {
            _realtimeClock = realtimeClock ?? throw new ArgumentNullException(nameof(realtimeClock));
            _wallClock = wallClock ?? throw new ArgumentNullException(nameof(wallClock));
            _utcOffsetProvider = utcOffsetProvider ?? throw new ArgumentNullException(nameof(utcOffsetProvider));

            var wallUtc = WallUtcNow;
            SetAnchor(wallUtc, _realtimeClock.SecondsSinceStartup, wallUtc);
        }

        public event Action Adjusted;

        public DateTime UtcNow => UtcAt(_realtimeClock.SecondsSinceStartup);

        public DateTime LocalNow => ToLocal(UtcNow) + _manualOffset;

        public bool IsManuallyAdjusted { get; private set; }

        private DateTime WallUtcNow => UtcDateTime.From(_wallClock.UtcNow);

        public void ApplySync(TimeSyncResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var elapsedSinceAnchor = _realtimeClock.SecondsSinceStartup - result.AnchorRealtime;
            var wallUtcAtAnchor = WallUtcNow.AddTicks(-ToTicks(elapsedSinceAnchor));

            SetAnchor(result.UtcAtAnchor, result.AnchorRealtime, wallUtcAtAnchor);
            ClearManualOffset();
            Adjusted?.Invoke();
        }

        public void SetLocalTimeOfDay(TimeSpan timeOfDay)
        {
            if (timeOfDay < TimeSpan.Zero || timeOfDay >= TimeSpan.FromDays(1d))
            {
                throw new ArgumentOutOfRangeException(nameof(timeOfDay), timeOfDay, "Time of day must be within a single day.");
            }

            var syncedLocal = ToLocal(UtcNow);
            _manualOffset = syncedLocal.Date + timeOfDay - syncedLocal;
            IsManuallyAdjusted = true;
            Adjusted?.Invoke();
        }

        public void ResetToSyncedTime()
        {
            if (!IsManuallyAdjusted)
            {
                return;
            }

            ClearManualOffset();
            Adjusted?.Invoke();
        }

        public bool CompensateSuspension()
        {
            var realtime = _realtimeClock.SecondsSinceStartup;
            var wallUtc = WallUtcNow;
            var lag = (wallUtc - _anchorWallUtc).TotalSeconds - (realtime - _anchorRealtime);
            var isSuspensionDetected = lag >= SuspensionThresholdSeconds;
            var utc = UtcAt(realtime);

            SetAnchor(isSuspensionDetected ? utc.AddTicks(ToTicks(lag)) : utc, realtime, wallUtc);

            if (isSuspensionDetected)
            {
                Adjusted?.Invoke();
            }

            return isSuspensionDetected;
        }

        private DateTime UtcAt(double realtime)
        {
            return _anchorUtc.AddTicks(ToTicks(realtime - _anchorRealtime));
        }

        private DateTime ToLocal(DateTime utc)
        {
            return DateTime.SpecifyKind(utc + _utcOffsetProvider.GetUtcOffset(utc), DateTimeKind.Unspecified);
        }

        private void SetAnchor(DateTime utc, double realtime, DateTime wallUtc)
        {
            _anchorUtc = UtcDateTime.From(utc);
            _anchorRealtime = realtime;
            _anchorWallUtc = wallUtc;
        }

        private void ClearManualOffset()
        {
            _manualOffset = TimeSpan.Zero;
            IsManuallyAdjusted = false;
        }

        private static long ToTicks(double seconds)
        {
            return (long)Math.Round(seconds * TimeSpan.TicksPerSecond);
        }
    }
}
