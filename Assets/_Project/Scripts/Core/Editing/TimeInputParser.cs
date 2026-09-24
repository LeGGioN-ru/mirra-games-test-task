using System;
using System.Globalization;

namespace ClockApp.Core.Editing
{
    public static class TimeInputParser
    {
        private static readonly string[] s_formats = { "H':'mm", "H':'mm':'ss" };

        public static bool TryParse(string text, out TimeSpan timeOfDay)
        {
            timeOfDay = TimeSpan.Zero;

            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            if (!DateTime.TryParseExact(text.Trim(), s_formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            {
                return false;
            }

            timeOfDay = parsed.TimeOfDay;
            return true;
        }

        public static bool IsAllowedCharacter(char character)
        {
            return character >= '0' && character <= '9' || character == ':';
        }

        public static string Format(TimeSpan timeOfDay)
        {
            return timeOfDay.ToString("hh':'mm':'ss", CultureInfo.InvariantCulture);
        }
    }
}
