using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

namespace ClockApp.Infrastructure.Scenes
{
    public sealed class AddressablesSceneLoader : ISceneLoader
    {
        public async UniTask InitializeAsync(CancellationToken cancellationToken)
        {
            var handle = Addressables.InitializeAsync(false);

            try
            {
                await handle.WithCancellation(cancellationToken);
            }
            finally
            {
                Addressables.Release(handle);
            }
        }

        public async UniTask LoadSceneAsync(string address, IProgress<float> progress, CancellationToken cancellationToken)
        {
            var handle = Addressables.LoadSceneAsync(address, LoadSceneMode.Single);

            while (!handle.IsDone)
            {
                progress?.Report(handle.PercentComplete);
                await UniTask.Yield(cancellationToken);
            }

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                throw new InvalidOperationException($"Failed to load scene '{address}'.", handle.OperationException);
            }

            progress?.Report(1f);
        }
    }
}
