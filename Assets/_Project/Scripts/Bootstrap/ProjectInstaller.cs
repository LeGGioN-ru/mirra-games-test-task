using System;
using ClockApp.Core.Platform;
using ClockApp.Core.Synchronization;
using ClockApp.Core.Timekeeping;
using ClockApp.Infrastructure.Http;
using ClockApp.Infrastructure.Platform;
using ClockApp.Infrastructure.Scenes;
using ClockApp.Infrastructure.TimeSources;
using ClockApp.Presentation.Loading;
using UnityEngine;
using Zenject;

namespace ClockApp.Bootstrap
{
    public sealed class ProjectInstaller : MonoInstaller
    {
        private const double RequestTimeoutSeconds = 5d;

        [SerializeField] private LoadingCurtain _loadingCurtain;

        public override void InstallBindings()
        {
            Container.Bind<IRealtimeClock>().To<UnityRealtimeClock>().AsSingle();
            Container.Bind<IWallClock>().To<SystemWallClock>().AsSingle();
            Container.Bind<IUtcOffsetProvider>().To<LocalUtcOffsetProvider>().AsSingle();
            Container.Bind<IHttpClient>().FromInstance(new UnityWebRequestHttpClient(TimeSpan.FromSeconds(RequestTimeoutSeconds))).AsSingle();
            Container.Bind<ITimeSyncService>().FromMethod(CreateTimeSyncService).AsSingle();
            Container.Bind<IClock>().To<SyncedClock>().AsSingle();
            Container.Bind<ISceneLoader>().To<AddressablesSceneLoader>().AsSingle();
            Container.Bind<ILoadingScreen>().FromInstance(_loadingCurtain).AsSingle();

            if (HostPlatform.RealtimePausesDuringSleep)
            {
                Container.BindInterfacesTo<SuspensionMonitor>().AsSingle();
            }
        }

        private static ITimeSyncService CreateTimeSyncService(InjectContext context)
        {
            var container = context.Container;
            var sources = TimeSourceChain.CreateForCurrentPlatform(container.Resolve<IHttpClient>(), container.Resolve<IWallClock>());

            return new TimeSyncService(sources, container.Resolve<IRealtimeClock>());
        }
    }
}
