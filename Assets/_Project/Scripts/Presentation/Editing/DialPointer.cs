namespace ClockApp.Presentation.Editing
{
    public readonly struct DialPointer
    {
        public DialPointer(float angle, float normalizedRadius)
        {
            Angle = angle;
            NormalizedRadius = normalizedRadius;
        }

        public float Angle { get; }

        public float NormalizedRadius { get; }
    }
}
