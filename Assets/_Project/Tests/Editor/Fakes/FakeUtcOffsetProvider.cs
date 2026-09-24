using System;
using ClockApp.Core.Platform;

namespace ClockApp.Tests.Fakes
{
    internal sealed class FakeUtcOffsetProvider : IUtcOffsetProvider
    {
        private readonly DateTime _transitionUtc;
        private readonly TimeSpan _offsetAfterTransition;

        public FakeUtcOffsetProvider(TimeSpan offset)
            : this(offset, DateTime.MaxValue, offset)
        {
        }

        public FakeUtcOffsetProvider(TimeSpan offset, DateTime transitionUtc, TimeSpan offsetAfterTransition)
        {
            Offset = offset;
            _transitionUtc = transitionUtc;
            _offsetAfterTransition = offsetAfterTransition;
        }

        public TimeSpan Offset { get; set; }

        public TimeSpan GetUtcOffset(DateTime utc)
        {
            return utc < _transitionUtc ? Offset : _offsetAfterTransition;
        }
    }
}
