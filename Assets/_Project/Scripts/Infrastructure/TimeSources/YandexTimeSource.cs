using System;
using System.Threading;
using ClockApp.Core.Synchronization;
using ClockApp.Infrastructure.Http;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ClockApp.Infrastructure.TimeSources
{
    public sealed class YandexTimeSource : ITimeSource
    {
        private const string Url = "https://yandex.com/time/sync.json";

        private readonly IHttpClient _httpClient;

        public YandexTimeSource(IHttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public string Name => "yandex.com";

        public bool IsTrusted => true;

        public async UniTask<DateTime> GetUtcTimeAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(Url, cancellationToken);
            var payload = JsonUtility.FromJson<Payload>(response.Body);

            if (payload == null || payload.time <= 0L)
            {
                throw new FormatException("Unexpected yandex.com response.");
            }

            return DateTimeOffset.FromUnixTimeMilliseconds(payload.time).UtcDateTime;
        }

        [Serializable]
        private sealed class Payload
        {
            public long time;
        }
    }
}
