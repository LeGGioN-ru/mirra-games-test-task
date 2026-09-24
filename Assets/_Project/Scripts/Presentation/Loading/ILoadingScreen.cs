using System.Threading;
using Cysharp.Threading.Tasks;

namespace ClockApp.Presentation.Loading
{
    public interface ILoadingScreen
    {
        void Show();

        void SetStage(LoadingStage stage);

        void SetProgress(float progress);

        UniTask HideAsync(CancellationToken cancellationToken);
    }
}
