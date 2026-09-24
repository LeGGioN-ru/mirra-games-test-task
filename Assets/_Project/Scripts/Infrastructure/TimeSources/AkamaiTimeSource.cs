using System;
using System.Globalization;
using System.Threading;
using ClockApp.Core.Synchronization;
using ClockApp.Infrastructure.Http;
using Cysharp.Threading.Tasks;

namespace ClockApp.Infrastructure.TimeSources
{
    public sealed class AkamaiTimeSource : ITimeSource
    {
        private const string Url = "https://time.akamai.com/?ms";

        private readonly IHttpClient _httpClient;

        public AkamaiTimeSource(IHttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public string Name => "time.akamai.com";

        public bool IsTrusted => true;

        public async UniTask<DateTime> GetUtcTimeAsync(CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(Url, cancellationToken);

            if (!double.TryParse(response.Body.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var unixSeconds)
                || unixSeconds <= 0d)
            {
                throw new FormatException("Unexpected time.akamai.com response.");
            }

            return DateTimeOffset.FromUnixTimeMilliseconds((long)Math.Round(unixSeconds * 1000d)).UtcDateTime;
        }
    }
}
