using System;

public class SpellSetEventArgs : EventArgs
{
    public Spell[] Spells { get; private set; }

    public SpellSetEventArgs(Spell[] spells) => Spells = spells;
}
