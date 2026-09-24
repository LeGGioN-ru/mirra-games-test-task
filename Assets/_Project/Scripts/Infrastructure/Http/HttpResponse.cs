using System;
using System.Collections.Generic;

namespace ClockApp.Infrastructure.Http
{
    public sealed class HttpResponse
    {
        private readonly Dictionary<string, string> _headers;

        public HttpResponse(long statusCode, string body, IDictionary<string, string> headers)
        {
            StatusCode = statusCode;
            Body = body ?? string.Empty;
            _headers = headers == null
                ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                : new Dictionary<string, string>(headers, StringComparer.OrdinalIgnoreCase);
        }

        public long StatusCode { get; }

        public string Body { get; }

        public string GetHeader(string name)
        {
            return _headers.TryGetValue(name, out var value) ? value : null;
        }
    }
}
