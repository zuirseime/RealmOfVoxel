using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour
{
    public EventHandler<SpellSetEventArgs> SpellSetChanged;

    private static Game _instance;
    public static Game Instance => _instance;

    public SpellManager SpellManager { get; private set; }

    [SerializeField] private Weapon[] _weaponSet;
    [SerializeField] private Charm[] _charmSet;
    [SerializeField] private Weapon[] _defaultWeapons;
 
    private Spell[] _activeSpells = new Spell[4];

    public Spell[] CurrentSpellSet
    {
        get => _activeSpells;
        private set
        {
            Array.Resize(ref value, 4);
            _activeSpells = value;

            SpellSetChanged?.Invoke(this, new SpellSetEventArgs(_activeSpells));
        }
    }
    public Weapon[] WeaponSet => _weaponSet;

    private void Awake()
    {
        _instance = this;

        SpellManager = GetComponent<SpellManager>();
    }

    private void Start()
    {
        CurrentSpellSet = SpellManager.GetActive();

        foreach (Spell spell in CurrentSpellSet)
        {
            spell.Initialize();
        }

        Save();
    }

    public Weapon GetStartWeapon()
    {
        return _defaultWeapons.OrderBy(w => UnityEngine.Random.value).FirstOrDefault();
    }

    public Weapon GetWeapon(Weapon[] heldWeapons)
    {
        foreach (Weapon weapon in _weaponSet.OrderBy(w => UnityEngine.Random.value))
        {
            if (!heldWeapons.Any(w => w.Title == weapon.Title))
            {
                return weapon;
            }
        }

        return null;
    }

    public Collectable GetCollectable()
    {
        return GetCollectables(1).First();
    }

    public Collectable[] GetCollectables(int count)
    {
        List<Collectable> list = new();
        list.AddRange(_weaponSet);
        list.AddRange(_charmSet);

        return list.OrderBy(c => UnityEngine.Random.value).Take(count).ToArray();
    }

    public void ReplaceSpell(string spellId, int indexToReplace)
    {
        indexToReplace = Mathf.Clamp(indexToReplace, 0, CurrentSpellSet.Length);

        if (CurrentSpellSet[indexToReplace] != null)
            SpellManager.SetSpellActive(CurrentSpellSet[indexToReplace].ID, false);
        SpellManager.SetSpellActive(spellId, true);

        CurrentSpellSet[indexToReplace] = SpellManager.GetSpell(spellId);
    }

    public void AcquireSpell(string spellId)
    {
        SpellManager.SetSpellAcquired(spellId, true);
    }

    public void Save()
    {
        SpellManager.SaveSpellData();
    }
}

public class SpellSetEventArgs : EventArgs
{
    public Spell[] Spells { get; private set; }

    public SpellSetEventArgs(Spell[] spells) => Spells = spells;
}
