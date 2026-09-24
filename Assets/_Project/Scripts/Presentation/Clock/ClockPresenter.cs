using ClockApp.Core.Timekeeping;
using ClockApp.Presentation.Editing;
using Zenject;

namespace ClockApp.Presentation.Clock
{
    public sealed class ClockPresenter : ITickable
    {
        private readonly IClock _clock;
        private readonly ClockEditModel _editModel;
        private readonly AnalogClockView _analogView;
        private readonly DigitalClockView _digitalView;

        public ClockPresenter(IClock clock, ClockEditModel editModel, AnalogClockView analogView, DigitalClockView digitalView)
        {
            _clock = clock;
            _editModel = editModel;
            _analogView = analogView;
            _digitalView = digitalView;
        }

        public void Tick()
        {
            if (_editModel.IsEditing)
            {
                _analogView.Show(_editModel.Draft.TotalSeconds, false);
                return;
            }

            var secondsOfDay = _clock.LocalNow.TimeOfDay.TotalSeconds;
            _analogView.Show(secondsOfDay, true);
            _digitalView.Show(secondsOfDay);
        }
    }
}
