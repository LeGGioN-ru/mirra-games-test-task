using System;
using System.Collections.Generic;
using System.Threading;
using ClockApp.Infrastructure.Http;
using Cysharp.Threading.Tasks;

namespace ClockApp.Tests.Fakes
{
    internal sealed class FakeHttpClient : IHttpClient
    {
        private readonly HttpResponse _response;
        private readonly Exception _error;

        private FakeHttpClient(HttpResponse response, Exception error)
        {
            _response = response;
            _error = error;
        }

        public string LastMethod { get; private set; }

        public string LastUrl { get; private set; }

        public static FakeHttpClient WithBody(string body)
        {
            return new FakeHttpClient(new HttpResponse(200L, body, null), null);
        }

        public static FakeHttpClient WithHeader(string name, string value)
        {
            return new FakeHttpClient(new HttpResponse(200L, null, new Dictionary<string, string> { { name, value } }), null);
        }

        public static FakeHttpClient WithError(Exception error)
        {
            return new FakeHttpClient(null, error);
        }

        public UniTask<HttpResponse> GetAsync(string url, CancellationToken cancellationToken)
        {
            return Respond("GET", url);
        }

        public UniTask<HttpResponse> HeadAsync(string url, CancellationToken cancellationToken)
        {
            return Respond("HEAD", url);
        }

        private UniTask<HttpResponse> Respond(string method, string url)
        {
            LastMethod = method;
            LastUrl = url;

            return _error != null
                ? UniTask.FromException<HttpResponse>(_error)
                : UniTask.FromResult(_response);
        }
    }
}
