using System.Threading;
using Cysharp.Threading.Tasks;

namespace ClockApp.Core.Synchronization
{
    public interface ITimeSyncService
    {
        UniTask<TimeSyncResult> SynchronizeAsync(CancellationToken cancellationToken);
    }
}
