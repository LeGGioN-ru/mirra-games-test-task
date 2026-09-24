using System;
using ClockApp.Core.Editing;
using NUnit.Framework;

namespace ClockApp.Tests.Editing
{
    public sealed class TimeInputParserTests
    {
        [TestCase("9:05", 9, 5, 0)]
        [TestCase("09:05", 9, 5, 0)]
        [TestCase("23:59:59", 23, 59, 59)]
        [TestCase(" 7:30 ", 7, 30, 0)]
        [TestCase("0:00", 0, 0, 0)]
        [TestCase("12:00:07", 12, 0, 7)]
        [TestCase("0930", 9, 30, 0)]
        [TestCase("930", 9, 30, 0)]
        [TestCase("2359", 23, 59, 0)]
        [TestCase("093015", 9, 30, 15)]
        [TestCase("93015", 9, 30, 15)]
        public void ParsesValidTime(string text, int hours, int minutes, int seconds)
        {
            var parsed = TimeInputParser.TryParse(text, out var timeOfDay);

            Assert.IsTrue(parsed);
            Assert.AreEqual(new TimeSpan(hours, minutes, seconds), timeOfDay);
        }

        [TestCase("24:00")]
        [TestCase("7:5")]
        [TestCase("12:60")]
        [TestCase("1:02:3")]
        [TestCase("12:30:60")]
        [TestCase("123:00")]
        [TestCase("12-30")]
        [TestCase("7:30 PM")]
        [TestCase("07:30:00.5")]
        [TestCase("-1:30")]
        [TestCase("７:30")]
        [TestCase("2400")]
        [TestCase("1260")]
        [TestCase("93")]
        [TestCase("0930155")]
        [TestCase("abc")]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase(null)]
        public void RejectsInvalidTime(string text)
        {
            var parsed = TimeInputParser.TryParse(text, out var timeOfDay);

            Assert.IsFalse(parsed);
            Assert.AreEqual(TimeSpan.Zero, timeOfDay);
        }

        [TestCase('0', true)]
        [TestCase('9', true)]
        [TestCase(':', true)]
        [TestCase('a', false)]
        [TestCase(' ', false)]
        [TestCase('-', false)]
        public void FiltersCharacters(char character, bool expected)
        {
            Assert.AreEqual(expected, TimeInputParser.IsAllowedCharacter(character));
        }

        [Test]
        public void FormatsWithLeadingZeros()
        {
            Assert.AreEqual("09:05:07", TimeInputParser.Format(new TimeSpan(9, 5, 7)));
        }

        [Test]
        public void FormatDropsFractionalSeconds()
        {
            Assert.AreEqual("09:05:07", TimeInputParser.Format(new TimeSpan(0, 9, 5, 7, 999)));
        }
    }
}
