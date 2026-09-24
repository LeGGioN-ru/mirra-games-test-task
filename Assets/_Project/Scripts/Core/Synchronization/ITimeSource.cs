using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace ClockApp.Core.Synchronization
{
    public interface ITimeSource
    {
        string Name { get; }

        bool IsTrusted { get; }

        UniTask<DateTime> GetUtcTimeAsync(CancellationToken cancellationToken);
    }
}
