using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

[Serializable]
public class SpellData
{
    [field: SerializeField] public string SpellId { get; set; }
    [field: SerializeField] public bool Acquired { get; set; } = false;
    [field: SerializeField] public bool Active { get; set; } = false;
    [field: SerializeField] public int Slot { get; set; } = -1;
}

[Preserve]
public abstract class Spell : ScriptableObject, IDisplayable
{
    public event EventHandler<SpellEventArgs> SpellUsed;
    public event EventHandler<SpellEventArgs> SpellSelected;
    public event EventHandler<SpellEventArgs> SpellDeselected;

    [field: SerializeField] public string ID { get; set; }

    [SerializeField, ReadOnly] protected float _nextCastTime = 0;

    [Header("Information")]
    [SerializeField] protected string _spellName;
    [SerializeField, TextArea] protected string _description;
    [SerializeField] protected float _price;

    [Header("Visualisation")]
    [SerializeField] protected Sprite _sprite;
    [SerializeField] protected SpellEffect _effectPrefab;

    [Header("Stats")]
    [SerializeField] protected float _range;
    [SerializeField] protected float _cooldown;
    [SerializeField] protected float _manaCost;
    [SerializeField] protected float _duration;

    [Header("Audo Settings")]
    [SerializeField] private AudioClip _castSound;

    public AudioClip CastSound => _castSound;
    public float Range => _range;
    public float Cooldown => _cooldown;
    public string Title
    {
        get => _spellName;
        set => _spellName = value;
    }
    public string Description
    {
        get => _description;
        set => _description = value;
    }
    public Sprite Sprite
    {
        get => _sprite;
        set => _sprite = value;
    }
    public float ManaCost => _manaCost;
    public float Duration => _duration;
    public float Price => _price;

    public Dictionary<string, string> Stats { get; set; } = new();

    public void CastAt(Entity owner, Vector3 targetPosition)
    {
        if (!CanCast())
        {
            Deselect();
            return;
        }

        owner.DrainMana(ManaCost);
        ApplyEffect(owner, targetPosition);

        SpellDeselected?.Invoke(this, new SpellEventArgs(this));
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
            string valueFormatted = $"{Math.Round(value, 1)}{units}";
            if (!Stats.ContainsKey(key))
                Stats.Add(key, valueFormatted);
            else
                Stats[key] = valueFormatted;
        }
    }

    private bool CanCast() => Time.time >= _nextCastTime;

    protected abstract void ApplyEffect(Entity owner, Vector3 targetPosition);

    public override string ToString()
    {
        return $"{Title}";
    }
}

public class SpellEventArgs : EventArgs
{
    public Spell Spell { get; }

    public SpellEventArgs(Spell spell) => Spell = spell;
}