using System;
using System.Collections.Generic;
using UnityEngine;

//public abstract class SpellData : ScriptableObject

public abstract class Spell : ScriptableObject
{
    public event EventHandler<SpellEventArgs> SpellUsed;
    public event EventHandler<SpellEventArgs> SpellSelected;
    public event EventHandler<SpellEventArgs> SpellDeselected;

    [SerializeField, ReadOnly] protected float _nextCastTime = 0;

    [Header("Information")]
    [SerializeField] protected string _spellName;
    [SerializeField, TextArea] protected string _description;

    [Header("Status")]
    [SerializeField] protected float _price;
    [field: SerializeField] public bool Acquired { get; set; }
    [field: SerializeField] public bool Active { get; set; }

    [Header("Visualisation")]
    [SerializeField] protected Sprite _sprite;
    [SerializeField] protected SpellEffect _effectPrefab;

    [Header("Stats")]
    [SerializeField] protected float _range;
    [SerializeField] protected float _cooldown;
    [SerializeField] protected float _manaCost;
    [SerializeField] protected float _duration;

    public float Range => _range;
    public float Cooldown => _cooldown;
    public string SpellName => _spellName;
    public string Description => _description;
    public Sprite Sprite => _sprite;
    public float ManaCost => _manaCost;
    public float Duration => _duration;

    public Dictionary<string, string> Stats { get; private set; } = new();

    public void CastAt(Entity owner, Vector3 targetPosition)
    {
        if (!CanCast())
        {
            Deselect();
            return;
        }

        owner.DrainMana(ManaCost);
        ApplyEffect(owner, targetPosition);

        SpellUsed?.Invoke(this, new SpellEventArgs(this));
        _nextCastTime = Time.time + Cooldown;
    }

    public void Select()
    {
        if (CanCast())
            SpellSelected?.Invoke(this, new SpellEventArgs(this));
    }

    public void Deselect()
    {
        SpellDeselected?.Invoke(this, new SpellEventArgs(this));
    }

    public virtual void Initialize()
    {
        _nextCastTime = 0;
    }

    protected void AddToStats(string key, float value, char units)
        => AddToStats(key, value, units.ToString());

    protected void AddToStats(string key, float value, string units = "")
    {
        if (value != 0)
        {
            Stats.Add(key, $"{Math.Round(value, 1)}{units}");
        }
    }

    private bool CanCast() => Time.time >= _nextCastTime;

    protected abstract void ApplyEffect(Entity owner, Vector3 targetPosition);

    public override string ToString()
    {
        return $"{SpellName}";
    }
}

public class SpellEventArgs : EventArgs
{
    public Spell Spell { get; }

    public SpellEventArgs(Spell spell) => Spell = spell;
}