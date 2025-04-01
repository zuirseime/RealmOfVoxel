using System;
using UnityEngine;

[Serializable]
public class EntityModifier
{
    [SerializeField, Min(1)] private float _baseValue = 1;
    [field: SerializeField, ReadOnly] public float Value { get; set; }

    public float BaseValue => _baseValue;

    public void Reset()
    {
        Value = _baseValue;
    }
}
