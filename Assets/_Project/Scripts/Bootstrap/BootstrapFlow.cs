using System;
using System.Threading;
using ClockApp.Core.Synchronization;
using ClockApp.Core.Timekeeping;
using ClockApp.Infrastructure.Scenes;
using ClockApp.Presentation.Loading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace ClockApp.Bootstrap
{
    public sealed class BootstrapFlow : IInitializable
    {
        private const string ClockSceneAddress = "Clock";
        private const float StepCount = 3f;

        private readonly ISceneLoader _sceneLoader;
        private readonly ITimeSyncService _timeSyncService;
        private readonly IClock _clock;
        private readonly ILoadingScreen _loadingScreen;

        private int _completedSteps;

        public BootstrapFlow(ISceneLoader sceneLoader, ITimeSyncService timeSyncService, IClock clock, ILoadingScreen loadingScreen)
        {
            _sceneLoader = sceneLoader;
            _timeSyncService = timeSyncService;
            _clock = clock;
            _loadingScreen = loadingScreen;
        }

        public void Initialize()
        {
            RunAsync(Application.exitCancellationToken).Forget();
        }

        private async UniTaskVoid RunAsync(CancellationToken cancellationToken)
        {
            _loadingScreen.Show();
            _loadingScreen.SetStage(LoadingStage.SynchronizingTime);

            try
            {
                await UniTask.WhenAll(InitializeContentAsync(cancellationToken), SynchronizeClockAsync(cancellationToken));

                _loadingScreen.SetStage(LoadingStage.LoadingScene);
                var sceneProgress = Progress.Create<float>(value => _loadingScreen.SetProgress((_completedSteps + value) / StepCount));
                await _sceneLoader.LoadSceneAsync(ClockSceneAddress, sceneProgress, cancellationToken);
            }
            catch (Exception exception) when (exception is not OperationCanceledException)
            {
                Debug.LogException(exception);
                _loadingScreen.SetStage(LoadingStage.Failed);
            }
        }

        private async UniTask InitializeContentAsync(CancellationToken cancellationToken)
        {
            await _sceneLoader.InitializeAsync(cancellationToken);
            CompleteStep();
        }

        private async UniTask SynchronizeClockAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _timeSyncService.SynchronizeAsync(cancellationToken);
                _clock.ApplySync(result);
                LogSync(result);
            }
            catch (TimeSyncException exception)
            {
                Debug.LogWarning(exception.Message);
            }

            CompleteStep();
        }

        private void CompleteStep()
        {
            _completedSteps++;
            _loadingScreen.SetProgress(_completedSteps / StepCount);
        }

        private static void LogSync(TimeSyncResult result)
        {
            foreach (var failure in result.Failures)
            {
                Debug.LogWarning($"Time source failed: {failure}");
            }

            var message = $"Clock synchronized with {result.SourceName}, round trip {result.RoundTrip.TotalMilliseconds:F0} ms.";

            if (result.IsTrusted)
            {
                Debug.Log(message);
            }
            else
            {
                Debug.LogWarning(message);
            }
        }
    }
}
