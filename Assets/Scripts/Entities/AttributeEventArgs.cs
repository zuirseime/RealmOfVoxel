using System;

public class AttributeEventArgs : EventArgs
{
    public float MaxValue { get; private set; }
    public float CurrentValue { get; private set; }
    public float PreviousValue { get; private set; }

    public AttributeEventArgs(float value, float previous, float max)
    {
        CurrentValue = value;
        PreviousValue = previous;
        MaxValue = max;
    }

    public AttributeEventArgs(float value, float max)
    {
        CurrentValue = value;
        MaxValue = max;
    }
}
