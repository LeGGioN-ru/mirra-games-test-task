using System.Collections.Generic;
using ClockApp.Core.Platform;
using ClockApp.Core.Synchronization;
using ClockApp.Infrastructure.Http;
using ClockApp.Infrastructure.Platform;
using UnityEngine;

namespace ClockApp.Infrastructure.TimeSources
{
    public static class TimeSourceChain
    {
        public static IReadOnlyList<ITimeSource> CreateForCurrentPlatform(IHttpClient httpClient, IWallClock wallClock)
        {
            return Create(httpClient, wallClock, HostPlatform.IsBrowser, Application.absoluteURL);
        }

        public static IReadOnlyList<ITimeSource> Create(IHttpClient httpClient, IWallClock wallClock, bool isBrowser, string pageUrl)
        {
            var sources = new List<ITimeSource>();

            if (isBrowser)
            {
                sources.Add(new TimeApiTimeSource(httpClient));
                sources.Add(new AkamaiTimeSource(httpClient));
                sources.Add(new PageDateHeaderTimeSource(httpClient, pageUrl));
            }
            else
            {
                sources.Add(new YandexTimeSource(httpClient));
                sources.Add(new TimeApiTimeSource(httpClient));
                sources.Add(new AkamaiTimeSource(httpClient));
            }

            sources.Add(new DeviceTimeSource(wallClock));
            return sources;
        }
    }
}
