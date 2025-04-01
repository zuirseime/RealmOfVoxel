using System;
using UnityEngine;

public class SpellCaster : MonoBehaviour
{
    public event EventHandler<Spell[]> SpellsChanged;

    [SerializeField, ReadOnly] private Spell[] _spells;
    private Spell _selected;

    public Spell[] Spells => _spells;

    public void SetSpells(Spell[] spells)
    {
        _spells = spells;
        SpellsChanged?.Invoke(this, _spells);

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
        if (index < 0 || index >= _spells.Length)
            return;

        if (_selected != null && _spells[index].Title == _selected.Title)
        {
            _selected.Deselect();
            return;
        }

        GetComponent<Player>().ChangeState(new PlayerCastingState(GetComponent<Player>(), _spells[index]));
    }
}
