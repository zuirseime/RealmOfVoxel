using UnityEngine;

public abstract class AreaSpell : Spell
{
    [SerializeField] protected float _maxExpansion;
    [SerializeField] protected float _tickRate;
}
