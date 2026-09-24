using System;
using ClockApp.Core.Synchronization;

namespace ClockApp.Core.Timekeeping
{
    public interface IClock
    {
        event Action Adjusted;

        DateTime UtcNow { get; }

        DateTime LocalNow { get; }

        TimeSyncResult LastSync { get; }

        bool IsSynchronized { get; }

        bool IsManuallyAdjusted { get; }

        void ApplySync(TimeSyncResult result);

        void SetLocalTimeOfDay(TimeSpan timeOfDay);

        void ResetToSyncedTime();

        bool CompensateSuspension();
    }
}
