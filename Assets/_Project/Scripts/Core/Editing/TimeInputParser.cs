using System;
using System.Globalization;

namespace ClockApp.Core.Editing
{
    public static class TimeInputParser
    {
        private static readonly string[] s_formats = { "H':'mm", "H':'mm':'ss", "HHmm", "HHmmss" };

        public static bool TryParse(string text, out TimeSpan timeOfDay)
        {
            timeOfDay = TimeSpan.Zero;

            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            var candidate = text.Trim();

            if (candidate.Length is 3 or 5 && IsDigitsOnly(candidate))
            {
                candidate = "0" + candidate;
            }

            if (!DateTime.TryParseExact(candidate, s_formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            {
                return false;
            }

            timeOfDay = parsed.TimeOfDay;
            return true;
        }

        public static bool IsAllowedCharacter(char character)
        {
            return IsAsciiDigit(character) || character == ':';
        }

        public static string Format(TimeSpan timeOfDay)
        {
            return timeOfDay.ToString("hh':'mm':'ss", CultureInfo.InvariantCulture);
        }

        private static bool IsDigitsOnly(string text)
        {
            foreach (var character in text)
            {
                if (!IsAsciiDigit(character))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool IsAsciiDigit(char character)
        {
            return character >= '0' && character <= '9';
        }
    }
}
