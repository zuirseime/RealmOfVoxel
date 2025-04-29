using System;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public event EventHandler<SpellSetEventArgs> SpellsChanged;

    [SerializeField, ReadOnly] private Spell[] _spells;
    private Spell _selected;

    public Spell[] Spells => _spells;

    public void SetSpells(Spell[] spells)
    {
        _spells = spells;
        SpellsChanged?.Invoke(this, new SpellSetEventArgs(Spells));

        foreach (Spell spell in _spells)
        {
            if (spell == null)
                continue;

            spell.SpellSelected += (sender, args) => _selected = args.Spell;
            spell.SpellDeselected += (sender, args) => _selected = null;
        }
    }

    public void UseSpell(int index)
    {
        if (index < 0 || index >= _spells.Length || _spells[index] == null)
            return;

        if (_selected != null && _spells[index].Title == _selected.Title)
        {
            _selected.Deselect();
            return;
        }

        var player = GetComponent<Player>();
        player.SetActiveSpell(_spells[index]);
        player.ChangeState<PlayerCastingState>();
    }
}
