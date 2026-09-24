using System.Collections.Generic;
using ClockApp.Infrastructure.Http;
using NUnit.Framework;

namespace ClockApp.Tests.Infrastructure
{
    public sealed class HttpResponseTests
    {
        [Test]
        public void HeaderLookupIgnoresCase()
        {
            var response = new HttpResponse("body", new Dictionary<string, string> { { "date", "value" } });

            Assert.AreEqual("value", response.GetHeader("Date"));
            Assert.AreEqual("body", response.Body);
        }

        [Test]
        public void MissingHeadersAndBodyAreSafe()
        {
            var response = new HttpResponse(null, null);

            Assert.IsNull(response.GetHeader("Date"));
            Assert.AreEqual(string.Empty, response.Body);
        }
    }
}
