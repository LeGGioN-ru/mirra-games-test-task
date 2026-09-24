using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace ClockApp.Infrastructure.Scenes
{
    public interface ISceneLoader
    {
        UniTask InitializeAsync(CancellationToken cancellationToken);

        UniTask LoadSceneAsync(string address, IProgress<float> progress, CancellationToken cancellationToken);
    }
}
