using Zenject;

namespace ClockApp.Bootstrap
{
    public sealed class BootstrapInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesTo<BootstrapFlow>().AsSingle();
        }
    }
}
