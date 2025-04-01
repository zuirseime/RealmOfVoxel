using System;

public class CoinsEventArgs : EventArgs
{
    public float Last { get; private set; }
    public float Current { get; private set; }

    public CoinsEventArgs(float last, float current)
    {
        Last = last;
        Current = current;
    }
}
