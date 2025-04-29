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
    private int _money;

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
        foreach (Spell spell in SpellManager.GetAll())
        {
            spell.Initialize();
        }

        CurrentSpellSet = SpellManager.GetActive();

        Save();
    }

    public void AddMoney(int amount)
    {
        _money += amount;
    }

    public bool SpendMoney(int amount)
    {
        if (_money >= amount)
        {
            _money -= amount;
            return true;
        }
        return false;
    }

    public Weapon GetStartWeapon()
    {
        return _defaultWeapons?.OrderBy(w => UnityEngine.Random.value).FirstOrDefault();
    }

    public Weapon GetWeapon(Weapon[] heldWeapons)
    {
        List<Weapon> weapons = _weaponSet?.OrderBy(w => UnityEngine.Random.value).ToList();

        foreach (Weapon heldWeapon in heldWeapons)
        {
            if (heldWeapon == null)
                continue;

            foreach (Weapon weapon in weapons)
            {
                if (heldWeapon.Title == weapon.Title)
                    continue; 
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
        SpellManager.SetSpellActive(spellId, true, indexToReplace);

        CurrentSpellSet[indexToReplace] = SpellManager.GetSpell(spellId);
    }

    public void SwapActiveSpells(int indexA, int indexB)
    {
        if (indexA < 0 || indexA >= CurrentSpellSet.Length || 
            indexB < 0 || indexB >= CurrentSpellSet.Length || 
            indexA == indexB)
        {
            Debug.LogWarning($"Invalid indices for SwapActiveSpells: {indexA}, {indexB}");
            return;
        }

        Spell[] tempSpell = new Spell[CurrentSpellSet.Length];
        Array.Copy(CurrentSpellSet, tempSpell, CurrentSpellSet.Length);

        (tempSpell[indexA], tempSpell[indexB]) = (tempSpell[indexB], tempSpell[indexA]);

        CurrentSpellSet = tempSpell;
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
