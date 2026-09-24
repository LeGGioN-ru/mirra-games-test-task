using System.Threading;
using Cysharp.Threading.Tasks;

namespace ClockApp.Infrastructure.Http
{
    public interface IHttpClient
    {
        UniTask<HttpResponse> GetAsync(string url, CancellationToken cancellationToken);

        UniTask<HttpResponse> HeadAsync(string url, CancellationToken cancellationToken);
    }
}
