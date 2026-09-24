using ClockApp.Presentation.Loading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace ClockApp.Bootstrap
{
    public sealed class ClockSceneEntryPoint : IInitializable
    {
        private readonly ILoadingScreen _loadingScreen;

        public ClockSceneEntryPoint(ILoadingScreen loadingScreen)
        {
            _loadingScreen = loadingScreen;
        }

        public void Initialize()
        {
            _loadingScreen.HideAsync(Application.exitCancellationToken).Forget();
        }
    }
}
