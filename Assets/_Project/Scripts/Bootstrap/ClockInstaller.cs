using ClockApp.Presentation.Clock;
using UnityEngine;
using Zenject;

namespace ClockApp.Bootstrap
{
    public sealed class ClockInstaller : MonoInstaller
    {
        [SerializeField] private AnalogClockView _analogClockView;
        [SerializeField] private DigitalClockView _digitalClockView;

        public override void InstallBindings()
        {
            Container.BindInstance(_analogClockView);
            Container.BindInstance(_digitalClockView);
            Container.BindInterfacesTo<ClockPresenter>().AsSingle();
            Container.BindInterfacesTo<ClockSceneEntryPoint>().AsSingle();
        }
    }
}
