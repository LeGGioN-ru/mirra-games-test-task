using System;
using TMPro;
using UnityEngine;

namespace ClockApp.Presentation.Clock
{
    public sealed class DigitalClockView : MonoBehaviour
    {
        private const string TimeFormat = "{0:00}:{1:00}:{2:00}";

        [SerializeField] private TMP_Text _label;

        private long _shownWholeSecond = long.MinValue;

        public void Show(double secondsOfDay)
        {
            var wholeSecond = (long)Math.Floor(secondsOfDay);

            if (wholeSecond == _shownWholeSecond)
            {
                return;
            }

            _shownWholeSecond = wholeSecond;
            _label.SetText(TimeFormat, wholeSecond / 3600, wholeSecond / 60 % 60, wholeSecond % 60);
        }
    }
}
