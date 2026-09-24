using System;
using UnityEngine;

namespace ClockApp.Infrastructure.Platform
{
    public static class HostPlatform
    {
        public static bool IsBrowser => Application.platform == RuntimePlatform.WebGLPlayer;

        public static bool RealtimePausesDuringSleep => !SystemInfo.operatingSystem.StartsWith("Windows", StringComparison.OrdinalIgnoreCase);
    }
}
