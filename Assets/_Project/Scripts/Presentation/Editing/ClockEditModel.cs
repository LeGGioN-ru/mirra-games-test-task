using System;

namespace ClockApp.Presentation.Editing
{
    public sealed class ClockEditModel
    {
        public bool IsEditing { get; private set; }

        public TimeSpan Draft { get; private set; }

        public void Begin(TimeSpan draft)
        {
            IsEditing = true;
            Draft = draft;
        }

        public void SetDraft(TimeSpan draft)
        {
            Draft = draft;
        }

        public void End()
        {
            IsEditing = false;
        }
    }
}
