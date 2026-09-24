using Zenject;

namespace ClockApp.Bootstrap
{
    public sealed class ClockInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<ClockSceneEntryPoint>().AsSingle();
        }
    }
}
