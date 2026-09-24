using System;
using System.Threading;
using ClockApp.Core.Synchronization;
using ClockApp.Infrastructure.Http;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ClockApp.Infrastructure.TimeSources
{
    public sealed class TimeApiTimeSource : ITimeSource
    {
        private const string Url = "https://timeapi.io/api/Time/current/zone?timeZone=UTC";

        private readonly IHttpClient _httpClient;

        public TimeApiTimeSource(IHttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public string Name => "timeapi.io";

        public bool IsTrusted => true;

        public async UniTask<DateTime> GetUtcTimeAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(Url, cancellationToken);
            var payload = JsonUtility.FromJson<Payload>(response.Body);

            if (payload == null || payload.year <= 0)
            {
                throw new FormatException("Unexpected timeapi.io response.");
            }

            return new DateTime(
                payload.year,
                payload.month,
                payload.day,
                payload.hour,
                payload.minute,
                payload.seconds,
                payload.milliSeconds,
                DateTimeKind.Utc);
        }

        [Serializable]
        private sealed class Payload
        {
            public int year;
            public int month;
            public int day;
            public int hour;
            public int minute;
            public int seconds;
            public int milliSeconds;
        }
    }
}
