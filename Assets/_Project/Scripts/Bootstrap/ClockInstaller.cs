using ClockApp.Presentation.Clock;
using ClockApp.Presentation.Editing;
using UnityEngine;
using Zenject;

namespace ClockApp.Bootstrap
{
    public sealed class ClockInstaller : MonoInstaller
    {
        [SerializeField] private AnalogClockView _analogClockView;
        [SerializeField] private DigitalClockView _digitalClockView;
        [SerializeField] private EditPanelView _editPanelView;
        [SerializeField] private DialDragInput _dialDragInput;

        public override void InstallBindings()
        {
            Container.BindInstance(_analogClockView);
            Container.BindInstance(_digitalClockView);
            Container.BindInstance(_editPanelView);
            Container.BindInstance(_dialDragInput);
            Container.Bind<ClockEditModel>().AsSingle();
            Container.BindInterfacesTo<ClockPresenter>().AsSingle();
            Container.BindInterfacesTo<ClockEditPresenter>().AsSingle();
            Container.BindInterfacesTo<ClockSceneEntryPoint>().AsSingle();
        }
    }
}
