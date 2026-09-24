using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace ClockApp.Infrastructure.Http
{
    public sealed class UnityWebRequestHttpClient : IHttpClient
    {
        private readonly int _timeoutSeconds;

        public UnityWebRequestHttpClient(TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), timeout, "Timeout must be positive.");
            }

            _timeoutSeconds = Math.Max(1, (int)Math.Ceiling(timeout.TotalSeconds));
        }

        public UniTask<HttpResponse> GetAsync(string url, CancellationToken cancellationToken)
        {
            return SendAsync(UnityWebRequest.Get(url), cancellationToken);
        }

        public UniTask<HttpResponse> HeadAsync(string url, CancellationToken cancellationToken)
        {
            return SendAsync(UnityWebRequest.Head(url), cancellationToken);
        }

        private async UniTask<HttpResponse> SendAsync(UnityWebRequest request, CancellationToken cancellationToken)
        {
            using (request)
            {
                request.timeout = _timeoutSeconds;
                await request.SendWebRequest().WithCancellation(cancellationToken);

                return new HttpResponse(request.responseCode, request.downloadHandler?.text, request.GetResponseHeaders());
            }
        }
    }
}
