using System;

public class WeaponEventArgs : EventArgs
{
    public Weapon Primary { get; private set; }
    public Weapon Secondary { get; private set; }

    public WeaponEventArgs(Weapon primary, Weapon secondary)
    {
        Primary = primary;
        Secondary = secondary;
    }
}
