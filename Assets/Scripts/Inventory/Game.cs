using System;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    private static Game _instance;
    public static Game Instance => _instance;

    [SerializeField] private Spell[] _spellSet;
    private Spell[] _acquiredSpells;
    private Spell[] _activeSpells = new Spell[4];

    public Spell[] CurrentSpellSet => _activeSpells;

    private void Awake()
    {
        _instance = this;
        _acquiredSpells = _spellSet.Where(s => s.Acquired).ToArray();
        _activeSpells = _acquiredSpells.Where(s => s.Active).Take(4).ToArray();
        foreach (Spell spell in _activeSpells)
        {
            spell.Initialize();
        }
        Array.Resize(ref _activeSpells, 4);
    }

    public void ReplaceSpell(Spell spell, int indexToReplace)
    {
        indexToReplace = Mathf.Clamp(indexToReplace, 0, CurrentSpellSet.Length);

        CurrentSpellSet[indexToReplace].Active = false;
        spell.Active = true;

        CurrentSpellSet[indexToReplace] = spell;
    }
}
