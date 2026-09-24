using ClockApp.Core.Timekeeping;
using Zenject;

namespace ClockApp.Presentation.Clock
{
    public sealed class ClockPresenter : ITickable
    {
        private readonly IClock _clock;
        private readonly AnalogClockView _analogView;
        private readonly DigitalClockView _digitalView;

        public ClockPresenter(IClock clock, AnalogClockView analogView, DigitalClockView digitalView)
        {
            _clock = clock;
            _analogView = analogView;
            _digitalView = digitalView;
        }

        public void Tick()
        {
            var secondsOfDay = _clock.LocalNow.TimeOfDay.TotalSeconds;
            _analogView.Show(secondsOfDay);
            _digitalView.Show(secondsOfDay);
        }
    }
}
