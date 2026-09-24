using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using ClockApp.Core.Synchronization;
using ClockApp.Infrastructure.Http;
using Cysharp.Threading.Tasks;

namespace ClockApp.Infrastructure.TimeSources
{
    public sealed class PageDateHeaderTimeSource : ITimeSource
    {
        private const string DateHeader = "Date";
        private const string CacheBustingParameter = "?nocache=";

        private static readonly TimeSpan s_headerResolutionCompensation = TimeSpan.FromMilliseconds(500d);

        private readonly IHttpClient _httpClient;
        private readonly Uri _pageUri;

        public PageDateHeaderTimeSource(IHttpClient httpClient, string pageUrl)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            if (!Uri.TryCreate(pageUrl, UriKind.Absolute, out _pageUri))
            {
                throw new ArgumentException("Page URL must be absolute.", nameof(pageUrl));
            }
        }

        public string Name => _pageUri.Host;

        public bool IsTrusted => true;

        public async UniTask<DateTime> GetUtcTimeAsync(CancellationToken cancellationToken)
        {
            var url = new Uri(_pageUri, CacheBustingParameter + Stopwatch.GetTimestamp().ToString(CultureInfo.InvariantCulture)).AbsoluteUri;
            var response = await _httpClient.HeadAsync(url, cancellationToken);
            var header = response.GetHeader(DateHeader);

            if (string.IsNullOrEmpty(header)
                || !DateTime.TryParseExact(
                    header,
                    "r",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                    out var serverUtc))
            {
                throw new FormatException("Missing or invalid Date header.");
            }

            return serverUtc + s_headerResolutionCompensation;
        }
    }
}
